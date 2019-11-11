using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Five_Seconds.Droid.Services;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.Repository;
using Five_Seconds.Services;
using Plugin.CurrentActivity;
using Xamarin.Essentials;
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

        private LinearLayout countLayout;
        private RelativeLayout notCountLayout;

        private Button startButton;
        private Button laterButton;
        private ImageView tellmeView;
        public TextView countTextView;
        private TextView timeTextView;
        public TextView timeOutTextView;
        private TextView alarmTextView;
        private TextView finishAlarmText;
        private TextView pleaseSayText;
        private EditText alarmEditText;
        private CountDown countDownForFailed;
        private CountDown countDownForFiveSeconds;

        private Dialog reviewDialog;
        private TextView reviewTitle;
        private TextView reviewText1;
        private TextView reviewText2;
        private LinearLayout feedbackButtonLayout;
        private LinearLayout reviewButtonLayout;

        private Dialog resultDialog;
        private LinearLayout buttonLayout;


        private Dialog laterDialog;
        private NumberPicker laterNumberPicker;

        private SpeechRecognizer mSpeechRecognizer;
        private Intent mSpeechRecognizerIntent;
        const int MY_PERMISSIONS_RECORD_AUDIO = 2357;

        AdView adViewForResult;
        AdView adViewForLater;

        private int id;
        private string name;
        private string toneName;
        private bool IsAlarmOn;
        private bool IsVibrateOn;
        private bool IsRepeating;
        private bool IsVoiceRecognition;
        private int alarmVolume;

        public bool CanShowReview;
        private DateTime AlarmTimeNow;

        public bool IsSuccess = false;
        public bool IsFinished = false;

        private Alarm alarm;
        private IAlarmService alarmService;
        private IAlarmsRepository alarmsRepo;
        private IAlarmToneRepository tonesRepo;

        private AdRequest requestBuilder;

        public Handler Handler => new Handler();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AlarmTimeNow = DateTime.Now;

            GetAlarmServiceAndRepository();

            Bundle bundle = Intent.Extras;
            id = (int)bundle.Get("id");
            GetDataFromBundle(bundle);

            if (id == -100000)
            {
                CheckOnlyCountActivityAndRefreshAlarmManager();
                return;
            }

            SettingForAlarmActivity(savedInstanceState);
            CreatingForAlarmActivity();

            if (bundle == null) return;
        }

        private void GetAlarmServiceAndRepository()
        {
            alarmService = HelperAndroid.GetAlarmService();
            alarmsRepo = alarmService.Repository;
            tonesRepo = HelperAndroid.GetAlarmToneRepository();
        }

        private void GetDataFromBundle(Bundle bundle)
        {
            name = (string)bundle.Get("name");
            toneName = (string)bundle.Get("toneName");
            IsAlarmOn = (bool)bundle.Get("IsAlarmOn");
            IsVibrateOn = (bool)bundle.Get("IsVibrateOn");
            IsRepeating = (bool)bundle.Get("IsRepeating");
            IsVoiceRecognition = (bool)bundle.Get("IsVoiceRecognition");
            alarmVolume = (int)bundle.Get("alarmVolume");
        }

        private void CheckOnlyCountActivityAndRefreshAlarmManager()
        {
            // Refresh AlarmManager
            Alarm.IsInitFinished = false;
            var allAlarms = alarmService.GetAllAlarms();
            Alarm.IsInitFinished = true;
            AlarmHelper.RefreshAlarmByManager100(allAlarms);

            OnlyCountDown();
        }

        private void OnlyCountDown()
        {
            SetContentView(Resource.Layout.AlarmActivity);
            SetControlsForCountActivity();
            ShowCountActivity();
        }

        private void SetControlsForCountActivity()
        {
            notCountLayout = FindViewById<RelativeLayout>(Resource.Id.notCountLayout);
            countLayout = FindViewById<LinearLayout>(Resource.Id.countLayout);
            finishAlarmText = FindViewById<TextView>(Resource.Id.finishAlarmText);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            countTextView.Text = "5.0";
            finishAlarmText.Visibility = ViewStates.Gone;
        }

        private void ShowCountActivity()
        {
            var toastService = new ToastServiceAndroid();

            toastService.Show("이제 5초를 셉니다!");

            HideAllViewExceptForCountText();

            _soundService?.PlayCountAudio();

            SetCountDown();
        }
        private void HideAllViewExceptForCountText()
        {
            notCountLayout.Visibility = ViewStates.Invisible;
            countLayout.Visibility = ViewStates.Visible;
            finishAlarmText.Text = $"{name}";
        }

        private async void SetCountDown()
        {
            await Task.Delay(330);
            countDownForFiveSeconds = new CountDown(5000, 10, this, true);
            countDownForFiveSeconds.Start();
        }

        private void SettingForAlarmActivity(Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            MobileAds.Initialize(ApplicationContext, GetString(Resource.String.admob_app_id));

            SetContentView(Resource.Layout.AlarmActivity);
            GetAlarmFromDB();
            SetAndFindViewById();

            AddWindowManagerFlags();

            SetMediaPlayerAndVibrator();
        }

        private void GetAlarmFromDB()
        {
            alarm = alarmsRepo?.GetAlarm(id);
            alarm.Days = alarmsRepo?.GetDaysOfWeek(alarm.DaysId);

            if (alarm == null)
            {
                FinishAndRemoveTask();
            }
        }

        private void SetAndFindViewById()
        {
            notCountLayout = FindViewById<RelativeLayout>(Resource.Id.notCountLayout);
            countLayout = FindViewById<LinearLayout>(Resource.Id.countLayout);

            startButton = FindViewById<Button>(Resource.Id.startButton);
            laterButton = FindViewById<Button>(Resource.Id.laterButton);
            tellmeView = FindViewById<ImageView>(Resource.Id.tellmeView);
            countTextView = FindViewById<TextView>(Resource.Id.countTextView);
            timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            alarmTextView = FindViewById<TextView>(Resource.Id.alarmTextView);
            finishAlarmText = FindViewById<TextView>(Resource.Id.finishAlarmText);
            pleaseSayText = FindViewById<TextView>(Resource.Id.pleaseSayText);
            alarmEditText = FindViewById<EditText>(Resource.Id.alarmEditText);
            timeOutTextView = FindViewById<TextView>(Resource.Id.timeOutTextView);

            timeOutTextView.Text = "60초 이내";
            countTextView.Text = "5.0";
            alarmTextView.Text = name;
            finishAlarmText.Text = name;
            timeTextView.Text = AlarmTimeNow.ToShortTimeString();

            tellmeView.Click += StartListening_Click;
            startButton.Click += StartButton_Click;
            laterButton.Click += LaterButton_Click; ;
        }

        private void StartListening_Click(object sender, EventArgs e)
        {
            RequestRecordAudioPermission();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (IsVoiceRecognition)
            {
                HandleVoiceRecognitionResult();
            }
            else
            {
                //TestReviewDialog();

                SuccessAlarm();
            }
        }

        private void HandleVoiceRecognitionResult()
        {
            var editText = alarmEditText.Text.Replace(" ", "");
            var textView = alarmTextView.Text.Replace(" ", "");

            if (editText == textView)
            {
                SuccessAlarm();
            }
            else
            {
                SetMediaPlayerAndVibrator();

                FailedAlarm();
            }
        }
        private void FailedAlarm()
        {
            pleaseSayText.Text = "라고 적어주세요";
            SetVisibleOfStartButton();
            alarmEditText.RequestFocus();
            InputMethodManager imm = GetSystemService(InputMethodService) as InputMethodManager;
            imm.ShowSoftInput(alarmEditText, ShowFlags.Implicit);
            alarmEditText.SetSelection(alarmEditText.Text.Length);
            ShowToastDoNotMatchText();
        }
        private void ShowToastDoNotMatchText()
        {
            Toast.MakeText(ApplicationContext, "알람 이름이 일치하지 않습니다. 알람 이름을 정확히 기입해주세요", ToastLength.Long).Show();
        }

        [Conditional("DEBUG")]
        private void TestReviewDialog()
        {
            //ReviewDialog Test:: 
            CreateReviewDialog();
            ShowReviewDialog();
        }

        private void SuccessAlarm()
        {
            TurnOffSoundAndVibration();

            countDownForFailed.Cancel();

            IsSuccess = true;

            var successStack = Preferences.Get("SuccessStack", 0);

            Preferences.Set("SuccessStack", successStack + 1);

            ShowResultDialog();
        }

        public void ShowResultDialog()
        {
            CreateRecord();

            IsFinished = true;

            SetResultToDialogResult();

            resultDialog.Show();
        }

        private void CreateRecord()
        {
            alarm.Time = new TimeSpan(AlarmTimeNow.Hour, AlarmTimeNow.Minute, 0);
            var record = new Record(alarm, IsSuccess);
            alarmsRepo.SaveRecord(record);
        }

        private void SetResultToDialogResult()
        {
            var Records = alarmsRepo.RecordFromDB;
            var alarmRecords = Records.FindAll(a => a.Name == alarm.Name);

            TextView titleText = resultDialog.FindViewById<TextView>(Resource.Id.titleText);
            TextView messageText = resultDialog.FindViewById<TextView>(Resource.Id.messageText);
            LinearLayout buttonLayout = resultDialog.FindViewById<LinearLayout>(Resource.Id.buttonLayout);
            Button confirmBtn = resultDialog.FindViewById<Button>(Resource.Id.confirmBtn);
            Button countButton = resultDialog.FindViewById<Button>(Resource.Id.countButton);
            Button failedBtn = resultDialog.FindViewById<Button>(Resource.Id.failedBtn);

            if (IsSuccess)
            {
                titleText.Text = "알람 성공!";

                confirmBtn.Click += ResultDialog_ConfirmBtn_Click;
                countButton.Click += ResultDialog_CountBtn_Click;
            }
            else
            {
                titleText.Text = "알람 실패 ㅠ^ㅠ";
                titleText.SetTextColor(Resources.GetColor(Resource.Color.failedTitleColor, Theme));
                messageText.SetTextColor(Resources.GetColor(Resource.Color.failedMessageColor, Theme));
                buttonLayout.Visibility = ViewStates.Gone;
                failedBtn.Visibility = ViewStates.Visible;

                failedBtn.Click += ResultDialog_ConfirmBtn_Click;
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
                    bool HasShownReview = Preferences.Get("HasShownReview", false);
                    var successStack = Preferences.Get("SuccessStack", 0);
                    CanShowReview = alarmRecords.Count > 4 && IsSuccess && !HasShownReview && successStack > 9;

                    if (CanShowReview)
                    {
                        CreateReviewDialog();
                    }
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

        private void ResultDialog_ConfirmBtn_Click(object sender, EventArgs e)
        {
            resultDialog.Dismiss();

            if (CanShowReview)
            {
                ShowReviewDialog();
            }
            else
            {
                FinishAndRemoveTask();
            }
        }

        private void ResultDialog_CountBtn_Click(object sender, EventArgs e)
        {
            resultDialog.Dismiss();
            ShowCountActivity();
        }

        private void LaterButton_Click(object sender, EventArgs e)
        {
            laterDialog.Show();
        }
        private void AddWindowManagerFlags()
        {
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);
        }

        private void SetMediaPlayerAndVibrator()
        {
            SetMediaPlayer();
            SetVibrator();
        }
        private void SetMediaPlayer()
        {
            if (IsAlarmOn)
            {
                AlarmTone alarmTone = tonesRepo.Tones.Find(a => a.Name == toneName);
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

        private void CreatingForAlarmActivity()
        {
            CreateNextAlarm();

            CreateSpeechRecognizer();

            CreateFailedCountDown();

            CreateStartBtnByIsVoiceRecognition();

            CreateLaterDialog();

            CreateResultDialog();

            CreateAdView();
        }

        private void CreateNextAlarm()
        {
            if (alarm.IsLaterAlarm)
            {
                alarm.IsLaterAlarm = false;
            }

            if (!IsRepeating)
            {
                Alarm.ChangeIsActive(alarm, false);
            }
            else
            {
                AlarmHelper.SetRepeatAlarm(alarm);
            }

            alarmService.SaveAlarmAtLocal(alarm);
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

        private void CreateFailedCountDown()
        {
            countDownForFailed = new CountDown(60000, 1000, this, false);
            countDownForFailed.Start();
        }


        private void CreateStartBtnByIsVoiceRecognition()
        {
            if (!IsVoiceRecognition)
            {
                SetVisibleOfStartButton();
                alarmEditText.Visibility = ViewStates.Gone;
            }
        }

        private void SetVisibleOfStartButton()
        {
            tellmeView.Visibility = ViewStates.Gone;
            alarmEditText.Visibility = ViewStates.Visible;
            startButton.Visibility = ViewStates.Visible;
        }

        private void CreateLaterDialog()
        {
            SetLaterDialog();
            FindAndSetLaterDialogById();
        }

        private void SetLaterDialog()
        {
            laterDialog = new Dialog(this);
            laterDialog.SetContentView(Resource.Layout.LaterAlarmDialog);
            laterDialog.SetTitle($"{AlarmTimeNow.ToShortTimeString()}에서");

            laterDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            laterNumberPicker = laterDialog.FindViewById<NumberPicker>(Resource.Id.laterNumberPicker);
            laterNumberPicker.MaxValue = 119;
            laterNumberPicker.MinValue = 1;
            laterNumberPicker.Value = 10;
        }

        private void FindAndSetLaterDialogById()
        {
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

            AlarmHelper.SetLaterAlarm(alarm);
            //AlarmController.SetLaterAlarmByManager(alarm, (long)diffTimeSpan.TotalMilliseconds);

            alarmService.SaveAlarmAtLocal(alarm);

            countDownForFailed.Cancel();

            Toast.MakeText(ApplicationContext, CreateDateString.CreateTimeRemainingString(alarmTime), ToastLength.Long).Show();

            AlarmNotificationAndroid.NotifyLaterAlarm(alarm, Intent);
        }

        private DateTime SetLaterAlarmAndGetLaterAlamrTime(int minutes)
        {
            var alarmTime = AlarmTimeNow.AddMinutes(minutes);

            Alarm.ChangeIsActive(alarm, true);

            alarm.LaterAlarmTime = alarmTime;

            return alarmTime;
        }

        private void CreateResultDialog()
        {
            resultDialog = new Dialog(this);
            resultDialog.SetContentView(Resource.Layout.AlarmResultDialog);
            resultDialog.SetCancelable(false);
            resultDialog.SetCanceledOnTouchOutside(false);

            resultDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            adViewForResult = resultDialog.FindViewById<AdView>(Resource.Id.adView);
        }

        private void CreateAdView()
        {
            requestBuilder = new AdRequest.Builder().Build();

            CreateRequestBuilderWhenTest();

            adViewForResult.LoadAd(requestBuilder);
            adViewForLater.LoadAd(requestBuilder);
        }

        [Conditional("DEBUG")]
        private void CreateRequestBuilderWhenTest()
        {
            requestBuilder.Dispose();
            requestBuilder = new AdRequest.Builder().AddTestDevice("FA3E0133F649B126EB4B86A6DA3E60D2").Build();
        }

        private void TurnOffSoundAndVibration()
        {
            _soundService?.StopAudio();
            _vibrator?.Cancel();
        }

        // 뒤로가기, 홈버튼, 액티비티 종료

        public override void OnBackPressed()
        {
        }

        protected override void OnUserLeaveHint()
        {
            if (!IsFinished)
            {
                SetLaterAlarm(5);
            }
            FinishAndRemoveTask();

            base.OnUserLeaveHint();
        }

        public override void FinishAndRemoveTask()
        {
            TurnOffSoundAndVibration();
            base.FinishAndRemoveTask();
        }

        // 마이크 권한

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

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

        // Review Dialog

        private void CreateReviewDialog()
        {
            reviewDialog = new Dialog(this);
            reviewDialog.SetContentView(Resource.Layout.ReviewDialog);
            reviewDialog.SetCancelable(false);
            reviewDialog.SetCanceledOnTouchOutside(false);

            reviewDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
        }

        public void ShowReviewDialog()
        {
            SetReviewToDialogResult();

            reviewDialog.Show();
        }

        private void SetReviewToDialogResult()
        {
            var Records = alarmsRepo.RecordFromDB;
            var alarmRecords = Records.FindAll(a => a.Name == alarm.Name);

            reviewTitle = reviewDialog.FindViewById<TextView>(Resource.Id.reviewTitle);
            reviewText1 = reviewDialog.FindViewById<TextView>(Resource.Id.reviewText1);
            reviewText2 = reviewDialog.FindViewById<TextView>(Resource.Id.reviewText2);

            buttonLayout = reviewDialog.FindViewById<LinearLayout>(Resource.Id.buttonLayout);
            feedbackButtonLayout = reviewDialog.FindViewById<LinearLayout>(Resource.Id.feedbackButtonLayout);
            reviewButtonLayout = reviewDialog.FindViewById<LinearLayout>(Resource.Id.reviewButtonLayout);

            SetClickEventForReviewDialog();
        }

        private void SetClickEventForReviewDialog()
        {
            Button confirmBtn1 = reviewDialog.FindViewById<Button>(Resource.Id.confirmBtn1);
            Button confirmBtn2 = reviewDialog.FindViewById<Button>(Resource.Id.confirmBtn2);
            Button confirmBtn3 = reviewDialog.FindViewById<Button>(Resource.Id.confirmBtn3);
            Button cancelBtn1 = reviewDialog.FindViewById<Button>(Resource.Id.cancelBtn1);
            Button cancelBtn2 = reviewDialog.FindViewById<Button>(Resource.Id.cancelBtn2);
            Button cancelBtn3 = reviewDialog.FindViewById<Button>(Resource.Id.cancelBtn3);
            Button laterBtn3 = reviewDialog.FindViewById<Button>(Resource.Id.laterBtn3);

            confirmBtn1.Click += ConfirmBtn1_Click;
            cancelBtn1.Click += CancelBtn1_Click;

            confirmBtn2.Click += ConfirmBtn2_Click;
            cancelBtn2.Click += CancelBtn2_Click;

            confirmBtn3.Click += ConfirmBtn3_Click;
            cancelBtn3.Click += CancelBtn3_Click;
            laterBtn3.Click += LaterBtn3_Click;
        }

        private void ConfirmBtn1_Click(object sender, EventArgs e)
        {
            buttonLayout.Visibility = ViewStates.Gone;
            reviewButtonLayout.Visibility = ViewStates.Visible;

            reviewTitle.Text = "도움이 되서 기뻐요!";
            reviewText1.Text = "계속 도움이 되는 5초의 알람이 될게요!";
            reviewText2.Text = "5초의 알람 리뷰를 통해 추천해주시겠어요?";
        }

        private void CancelBtn1_Click(object sender, EventArgs e)
        {
            buttonLayout.Visibility = ViewStates.Gone;
            feedbackButtonLayout.Visibility = ViewStates.Visible;

            reviewTitle.Text = "도움드리지 못 해 죄송해요";
            reviewText1.Text = "앱을 더욱 보완해서 보답드릴게요!";
            reviewText2.Text = "불편, 건의 사항이 있다면 말씀해주시겠어요?";
        }

        private void ConfirmBtn2_Click(object sender, EventArgs e)
        {
            // 이메일 인텐트
            var intent = new Intent(Intent.ActionSend);
            intent.SetType("message/rfc822");
            intent.PutExtra(Intent.ExtraEmail, "save_us_222@naver.com");
            intent.PutExtra(Intent.ExtraSubject, $"{AlarmTimeNow.ToShortDateString()} 5초의 알람 오류 보고");

            try
            {
                StartActivity(Intent.CreateChooser(intent, "이메일 보내기.."));
            }
            catch (ActivityNotFoundException)
            {
                Toast.MakeText(this, "There are no email clients installed.", ToastLength.Short).Show();
            }

            Preferences.Set("SuccessStack", 0);
            FinishAndRemoveTask();
        }

        private void CancelBtn2_Click(object sender, EventArgs e)
        {
            Preferences.Set("SuccessStack", 0);
            FinishAndRemoveTask();
        }

        private void ConfirmBtn3_Click(object sender, EventArgs e)
        {
            Intent googleAppStoreIntent;
            try
            {
                googleAppStoreIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=" + PackageName));

            }
            catch (ActivityNotFoundException)
            {
                googleAppStoreIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=" + PackageName));

            }
            googleAppStoreIntent.AddFlags(ActivityFlags.NewTask);
            StartActivity(googleAppStoreIntent);

            Preferences.Set("HasShownReview", true);

            FinishAndRemoveTask();
        }

        private void CancelBtn3_Click(object sender, EventArgs e)
        {
            // 앱 리뷰 다신 안 하기

            Preferences.Set("HasShownReview", true);
            FinishAndRemoveTask();
        }

        private void LaterBtn3_Click(object sender, EventArgs e)
        {
            // 리뷰 알람 카운트 다시 카운트 하기
            Preferences.Set("SuccessStack", 0);
            FinishAndRemoveTask();
        }

        // Voice Recognition
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

            SetMediaPlayerAndVibrator();
        }

        public void OnEvent(int eventType, Bundle @params)
        {
        }

        public void OnPartialResults(Bundle partialResults)
        {
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
    }
}