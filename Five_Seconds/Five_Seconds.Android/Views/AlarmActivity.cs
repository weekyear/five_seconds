using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Five_Seconds.Droid.Interface;
using Five_Seconds.Droid.Services;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Button = Android.Widget.Button;
using RelativeLayout = Android.Widget.RelativeLayout;

namespace Five_Seconds.Droid
{
    [Activity(Label = "5초의 알람", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : Activity, IRecognitionListener
    {
        readonly IPlaySoundService _soundService = new PlaySoundServiceAndroid();
        Vibrator _vibrator;

        private Dialog feedbackDialog;
        private Dialog laterDialog;
        private NumberPicker laterNumberPicker;

        private SpeechRecognizer mSpeechRecognizer;
        private Intent mSpeechRecognizerIntent;
        const int MY_PERMISSIONS_RECORD_AUDIO = 2357;
        private RelativeLayout alarmTextLayout;
        private LinearLayout tellAlarmLayout;
        private LinearLayout buttonLayout;
        private Button startButton;
        private Button laterButton;
        private ImageView tellmeView;
        public TextView countTextView;
        private TextView timeTextView;
        public TextView timeOutTextView;
        private TextView alarmTextView;
        private TextView pleaseSayText;
        private EditText alarmEditText;
        private CountDown countDownForFailed;
        private CountDown countDownForFiveSeconds;

        AdView adViewForResult;
        AdView adViewForLater;

        private int id;
        private string name;
        private string toneName;
        private bool IsAlarmOn;
        private bool IsVibrateOn;
        private bool IsRepeating;
        private int alarmVolume;

        private DateTime AlarmTimeNow;
        public bool IsSuccess = false;

        public bool IsFinished = false;

        private bool IsPausePassed;

        Alarm alarm;
        IAlarmsRepository alarmsRepo;

        public Handler Handler => new Handler();

        public AlarmActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AlarmTimeNow = DateTime.Now;

            Bundle bundle = Intent.Extras;

            id = (int)bundle.Get("id");

            if (id == -100000)
            {
                OnlyCountDown();
                return;
            }

            GetDataFromBundle(bundle);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            SetMediaPlayer();
            SetVibrator();

            SetMobileAds();

            SetContentView(Resource.Layout.AlarmActivity);
            SetAndFindViewById();

            AddWindowManagerFlags();

            SetAlarmAfterCalled();

            CreateSpeechRecognizer();

            SetIsFailedCountDown();


            SetLaterDialog();

            SetFeedbackDialog();

            SetAdView();

            if (bundle == null) return;
        }

        private void SetIsFailedCountDown()
        {
            countDownForFailed = new CountDown(60000, 1000, this, false);
            countDownForFailed.Start();
        }

        private void OnlyCountDown()
        {
            SetContentView(Resource.Layout.AlarmActivity);
            SetControlsForCountActivity();
            ShowCountActivity();
        }

        private void SetAlarmAfterCalled()
        {
            alarmsRepo = App.AlarmsRepo;

            alarm = App.AlarmsRepo?.GetAlarm(id);
            alarm.Days = App.AlarmsRepo?.GetDaysOfWeek(alarm.DaysId);

            alarm.IsLaterAlarm = false;

            if (!IsRepeating)
            {
                alarm.IsActive = false;
                App.Service.SaveAlarmAtLocal(alarm);
            }
            else
            {
                AlarmController.SetNextAlarm(alarm);
            }
        }

        private void GetDataFromBundle(Bundle bundle)
        {
            name = (string)bundle.Get("name");
            toneName = (string)bundle.Get("toneName");
            IsAlarmOn = (bool)bundle.Get("IsAlarmOn");
            IsVibrateOn = (bool)bundle.Get("IsVibrateOn");
            IsRepeating = (bool)bundle.Get("IsRepeating");
            alarmVolume = (int)bundle.Get("alarmVolume");
        }


        private void SetAndFindViewById()
        {
            alarmTextLayout = FindViewById<RelativeLayout>(Resource.Id.alarmTextLayout);
            tellAlarmLayout = FindViewById<LinearLayout>(Resource.Id.tellAlarmLayout);
            buttonLayout = FindViewById<LinearLayout>(Resource.Id.buttonLayout);
            startButton = FindViewById<Button>(Resource.Id.startButton);
            laterButton = FindViewById<Button>(Resource.Id.laterButton);
            tellmeView = FindViewById<ImageView>(Resource.Id.tellmeView);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            alarmTextView = FindViewById<TextView>(Resource.Id.alarmTextView);
            pleaseSayText = FindViewById<TextView>(Resource.Id.pleaseSayText);
            alarmEditText = FindViewById<EditText>(Resource.Id.alarmEditText);
            timeOutTextView = FindViewById<TextView>(Resource.Id.timeOutTextView);

            timeOutTextView.Text = "60초 이내";
            countTextView.Text = "5.00";
            alarmTextView.Text = name;
            timeTextView.Text = AlarmTimeNow.ToShortTimeString();

            tellmeView.Click += StartListening_Click;
            startButton.Click += StartButton_Click;
            laterButton.Click += ShowLaterDialog;
        }

        private void SetControlsForCountActivity()
        {
            alarmTextLayout = FindViewById<RelativeLayout>(Resource.Id.alarmTextLayout);
            tellAlarmLayout = FindViewById<LinearLayout>(Resource.Id.tellAlarmLayout);
            buttonLayout = FindViewById<LinearLayout>(Resource.Id.buttonLayout);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            countTextView.Text = "5.00";
        }

        private void AddWindowManagerFlags()
        {
            // add flags to turn screen on and appear over lock screen
            var pm = GetSystemService(PowerService) as PowerManager;

            if (pm.IsInteractive)
            {
                IsPausePassed = true;
            }

            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);
        }

        private void CreateSpeechRecognizer()
        {
            mSpeechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
            mSpeechRecognizerIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.PackageName);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 2000);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 2000);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 1500);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            mSpeechRecognizerIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

            mSpeechRecognizer.SetRecognitionListener(this);
        }

        private void StartListening_Click(object sender, EventArgs e)
        {
            RequestRecordAudioPermission();
            //SendBroadcast(new Intent(Intent.ActionBootCompleted));
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            HandleVoiceRecognitionResult();
        }

        private void HandleVoiceRecognitionResult()
        {
            startButton.Text = "시작!";

            var editText = alarmEditText.Text.Replace(" ", "");
            var textView = alarmTextView.Text.Replace(" ", "");

            if (editText == textView)
            {
                countDownForFailed.Cancel();

                IsSuccess = true;

                ShowFeedbackDialog();
            }
            else
            {
                pleaseSayText.Text = "라고 적어주세요";
                SetVisibilityOfControls();
                alarmEditText.RequestFocus();
                InputMethodManager imm = GetSystemService(InputMethodService) as InputMethodManager;
                imm.ShowSoftInput(alarmEditText, ShowFlags.Implicit);
                alarmEditText.SetSelection(alarmEditText.Text.Length);
                ShowToastDoNotMatchText();
            }
        }

        private void ShowCountActivity()
        {
            var toastService = new ToastServiceAndroid();

            toastService.Show("이제 5초를 셉니다!");

            HideAllViewExceptForCountText();

            _soundService?.PlayCountAudio();

            SetCountDown();
        }

        private void SetVisibilityOfControls()
        {
            tellmeView.Visibility = ViewStates.Gone;
            alarmEditText.Visibility = ViewStates.Visible;
            startButton.Visibility = ViewStates.Visible;
        }

        private void ShowToastDoNotMatchText()
        {
            Toast.MakeText(ApplicationContext, "알람 이름이 일치하지 않습니다. 알람 이름을 정확히 기입해주세요", ToastLength.Long).Show();
        }

        private void SetMediaPlayer()
        {
            if (IsAlarmOn)
            {
                AlarmTone alarmTone = App.Tones.Find(a => a.Name == toneName);
                _soundService?.PlayAudio(alarmTone, true, alarmVolume);
            }
        }

        private void SetVibrator()
        {
            if (IsVibrateOn)
            {
                _vibrator = Vibrator.FromContext(this);
                long[] mVibratePattern = new long[] { 0, 400, 1000, 600, 1000, 800, 1000, 1000 };
                VibrationEffect effect = VibrationEffect.CreateWaveform(mVibratePattern, 0);
                _vibrator?.Vibrate(effect);
            }
        }

        private void HideAllViewExceptForCountText()
        {
            alarmTextLayout.Visibility = ViewStates.Invisible;
            tellAlarmLayout.Visibility = ViewStates.Invisible;
            buttonLayout.Visibility = ViewStates.Invisible;
            countTextView.Visibility = ViewStates.Visible;
        }

        private async void SetCountDown()
        {
            await Task.Delay(330);
            countDownForFiveSeconds = new CountDown(5000, 10, this, true);
            countDownForFiveSeconds.Start();
        }

        public override void OnBackPressed()
        {
        }

        private void CreateRecord()
        {
            alarm.Time = new TimeSpan(AlarmTimeNow.Hour, AlarmTimeNow.Minute, 0);
            var record = new Record(alarm, IsSuccess);
            alarmsRepo.SaveRecord(record);
        }

        public void OnBeginningOfSpeech()
        {
        }

        public void OnBufferReceived(byte[] buffer)
        {
        }

        public void OnEndOfSpeech()
        {
            mSpeechRecognizer?.StopListening();

            SetTellmeBtnDefault();
        }

        public void OnError(SpeechRecognizerError error)
        {
            //string message;
            switch (error)
            {

                case SpeechRecognizerError.Audio:
                    //message = "오디오 에러입니다.";
                    break;

                case SpeechRecognizerError.Client:
                    //message = "클라이언트 에러입니다.";
                    break;

                case SpeechRecognizerError.InsufficientPermissions:
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, "오디오 녹음 권한을 허용해주세요", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.Network:
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, "네트워크 에러입니다. 네트워크 연결 확인을 해주세요.", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.NetworkTimeout:
                    //message = "네트워크 시간초과입니다.";
                    break;

                case SpeechRecognizerError.NoMatch:
                    //message = "해당 음성 녹음 결과가 없습니다."; ;
                    break;

                case SpeechRecognizerError.RecognizerBusy:
                    //message = "다시 시도해주세요.";
                    break;

                case SpeechRecognizerError.Server:
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, "서버에 문제가 있습니다. 텍스트 창에 해야할 일을 적어주세요.", ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.SpeechTimeout:
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, "음성 녹음 시간이 초과되었습니다. 다시 시도해주세요.", ToastLength.Long).Show();
                    break;

                default:
                    //message = "알수없음";
                    break;
            }
        }

        public void OnEvent(int eventType, Bundle @params)
        {
        }

        public void OnPartialResults(Bundle partialResults)
        {
            //IList<string> matches = partialResults.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
        }

        public void OnReadyForSpeech(Bundle @params)
        {
        }

        public void OnResults(Bundle results)
        {
            IList<string> matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
            alarmEditText.Text = matches[0];
            HandleVoiceRecognitionResult();
        }

        public void OnRmsChanged(float rmsdB)
        {
        }

        private void StartVoiceRecognition()
        {
            TurnOffSoundAndVibration();
            SetTellmeBtnRecording();
            mSpeechRecognizer?.StartListening(mSpeechRecognizerIntent);
        }

        public void ShowFeedbackDialog()
        {
            CreateRecord();

            SetResultToDialogResult();

            IsFinished = true;

            feedbackDialog.Show();
        }

        private void SetFeedbackDialog()
        {
            feedbackDialog = new Dialog(this);
            feedbackDialog.SetContentView(Resource.Layout.AlarmFeedbackDialog);
            feedbackDialog.SetCancelable(false);
            feedbackDialog.SetCanceledOnTouchOutside(false);

            feedbackDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            adViewForResult = feedbackDialog.FindViewById<AdView>(Resource.Id.adView);
        }

        private void SetResultToDialogResult()
        {
            var Records = App.AlarmsRepo.RecordFromDB;
            var alarmRecords = Records.FindAll(a => a.Name == alarm.Name);

            TextView titleText = feedbackDialog.FindViewById<TextView>(Resource.Id.titleText);
            TextView messageText = feedbackDialog.FindViewById<TextView>(Resource.Id.messageText);
            LinearLayout buttonLayout = feedbackDialog.FindViewById<LinearLayout>(Resource.Id.buttonLayout);
            Button confirmBtn = feedbackDialog.FindViewById<Button>(Resource.Id.confirmBtn);
            Button countButton = feedbackDialog.FindViewById<Button>(Resource.Id.countButton);
            Button failedBtn = feedbackDialog.FindViewById<Button>(Resource.Id.failedBtn);

            if (IsSuccess)
            {
                titleText.Text = "알람 성공!";

                confirmBtn.Click += FeedbackDialog_ConfirmBtn_Click;
                countButton.Click += FeedbackDialog_CountBtn_Click;
            }
            else
            {
                titleText.Text = "알람 실패 ㅠ^ㅠ";
                titleText.SetTextColor(Resources.GetColor(Resource.Color.failedTitleColor, Theme));
                messageText.SetTextColor(Resources.GetColor(Resource.Color.failedMessageColor, Theme));
                buttonLayout.Visibility = ViewStates.Gone;
                failedBtn.Visibility = ViewStates.Visible;

                failedBtn.Click += FeedbackDialog_ConfirmBtn_Click;
            }

            if (alarmRecords.Count == 1)
            {
                if (IsSuccess)
                {
                    messageText.Text = $"첫 시작이 아주 좋네요! 5초 카운트가 끝나기 전에 '{alarm.Name}' 바로 시작해봐요!";
                }
                else
                {
                    messageText.Text = $"첫 단추를 잘못 끼웠다고 상심 말아요. 다시 끼우면 되잖아요! 다음번엔 잘 해봐요!";
                }
            }
            else
            {
                var successRecords = alarmRecords.FindAll(a => a.IsSuccess == true);
                double successRate = (double)successRecords.Count / alarmRecords.Count;

                if (successRate > 0.7)
                {
                }
                else if (successRate > 0.5)
                {
                }
                else if (successRate == -1)
                {
                }
                else
                {
                }

                messageText.Text = $"'{alarm.Name}' 알람의 전체 성공률은 {successRate * 100:0.##}% 입니다.";
            }
        }

        private void FeedbackDialog_ConfirmBtn_Click(object sender, EventArgs e)
        {
            feedbackDialog.Dismiss();
            FinishAndRemoveTask();
        }

        private void FeedbackDialog_CountBtn_Click(object sender, EventArgs e)
        {
            feedbackDialog.Dismiss();
            ShowCountActivity();
        }

        private void SetAdView()
        {
            //var requestbuilder = new AdRequest.Builder().AddTestDevice("FA3E0133F649B126EB4B86A6DA3E60D2").Build();
            //adViewForResult.LoadAd(requestbuilder);
            //adViewForLater.LoadAd(requestbuilder);

            var requestbuilder = new AdRequest.Builder().Build();
            adViewForResult.LoadAd(requestbuilder);
            adViewForLater.LoadAd(requestbuilder);
        }

        private void SetMobileAds()
        {
            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_app_id));
        }

        public void ShowLaterDialog(object s, EventArgs e)
        {
            //SetLaterDialog();

            //SetAdViewForLater();

            laterDialog.Show();
        }

        private void SetLaterDialog()
        {
            laterDialog = new Dialog(this);
            laterDialog.SetContentView(Resource.Layout.LaterAlarmDialog);
            laterDialog.SetTitle($"{AlarmTimeNow.ToShortTimeString()}에서");

            laterDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            laterNumberPicker = laterDialog.FindViewById<NumberPicker>(Resource.Id.laterNumberPicker);
            laterNumberPicker.MaxValue = 120;
            laterNumberPicker.MinValue = 1;
            laterNumberPicker.Value = 5;

            Button confirmBtn = laterDialog.FindViewById<Button>(Resource.Id.confirmBtn);
            Button cancelBtn = laterDialog.FindViewById<Button>(Resource.Id.cancelBtn);
            adViewForLater = laterDialog.FindViewById<AdView>(Resource.Id.adView);

            confirmBtn.Click += LaterDialog_ConfirmBtn_Click;
            cancelBtn.Click += LaterDialog_CancelBtn_Click;
        }

        private void LaterDialog_ConfirmBtn_Click(object sender, EventArgs e)
        {
            var number = laterNumberPicker.Value;

            SetLaterAlarm(number);

            IsFinished = true;
            laterDialog.Dismiss();
            FinishAndRemoveTask();
        }

        private void LaterDialog_CancelBtn_Click(object sender, EventArgs e)
        {
            laterDialog.Dismiss();
        }

        private void SetLaterAlarm(int minutes)
        {
            var alarmTime = SetLaterAlarmAndGetLaterAlamrTime(minutes);

            var diffTimeSpan = alarmTime.Subtract(DateTime.Now);

            AlarmController.SetLaterAlarmByManager(alarm, (long)diffTimeSpan.TotalMilliseconds);

            App.Service.SaveAlarmAtLocal(alarm);

            countDownForFailed.Cancel();

            Toast.MakeText(ApplicationContext, CreateDateString.CreateTimeRemainingString(alarmTime), ToastLength.Long).Show();

            AlarmNotificationAndroid.NotifyLaterAlarm(alarm, Intent);
        }

        private DateTime SetLaterAlarmAndGetLaterAlamrTime(int minutes)
        {
            var alarmTime = AlarmTimeNow.AddMinutes(minutes);

            alarm.IsActive = true;
            alarm.LaterAlarmTime = alarmTime;

            return alarmTime;
        }

        private void SetTellmeBtnDefault()
        {
            tellmeView.SetImageResource(Resource.Drawable.ic_mic);
            tellmeView.SetBackgroundResource(Resource.Drawable.rounded_button);
        }

        private void SetTellmeBtnRecording()
        {
            tellmeView.SetImageResource(Resource.Drawable.ic_settings_voice);
            tellmeView.SetBackgroundResource(Resource.Drawable.background_tellmebtn_recording);
        }

        private void TurnOffSoundAndVibration()
        {
            _soundService?.StopAudio();
            _vibrator?.Cancel();
        }

        protected override void OnUserLeaveHint()
        {
            base.OnUserLeaveHint();
        }

        protected override void OnPause()
        {
            if (IsPausePassed)
            {
                // 여기에 다시 울림 설정
                TurnOffSoundAndVibration();
                if (!IsFinished)
                {
                    SetLaterAlarm(5);
                    FinishAndRemoveTask();
                }
            }
            else
            {
                IsPausePassed = true;
            }
            base.OnPause();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == MY_PERMISSIONS_RECORD_AUDIO)
            {
                if (grantResults[0] != Permission.Granted)
                {
                    Toast.MakeText(ApplicationContext,
                            "알람을 해제하려면 음성 녹음 권한이 필요합니다.", ToastLength.Short).Show();
                }
                else
                {
                    StartVoiceRecognition();
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void RequestRecordAudioPermission()
        {
            if (CheckCallingOrSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                IsPausePassed = false;
                // Should we show an explanation?
                if (ShouldShowRequestPermissionRationale(Manifest.Permission.RecordAudio))
                {
                    // Explain to the user why we need to read the contacts
                    Toast.MakeText(this, "알람을 해제하려면 음성 녹음 권한이 필요합니다.", ToastLength.Long).Show();
                }

                RequestPermissions(new string[] { Manifest.Permission.RecordAudio }, MY_PERMISSIONS_RECORD_AUDIO);

                // MY_PERMISSIONS_REQUEST_READ_EXTERNAL_STORAGE is an
                // app-defined int constant that should be quite unique

                return;
            }
            else if (CheckCallingOrSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted)
            {
                StartVoiceRecognition();
            }
        }
    }
}