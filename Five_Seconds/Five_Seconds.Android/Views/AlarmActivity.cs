using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
using Android.Views.Animations;
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
using Animation = Android.Views.Animations.Animation;
using Button = Android.Widget.Button;
using RelativeLayout = Android.Widget.RelativeLayout;

namespace Five_Seconds.Droid
{
    [Activity(Label = "@string/FiveSecondAlarm", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : Activity, IRecognitionListener
    {
        readonly IPlaySoundService _soundService = new PlaySoundServiceAndroid();
        Vibrator _vibrator;

        private LinearLayout countLayout;
        private RelativeLayout notCountLayout;
        private FrameLayout tellmeLayout;

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
        private LinearLayout likeButtonLayout;


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
        public string WakeUpText;
        private bool IsAlarmOn;
        private bool IsVibrateOn;
        private bool IsRepeating;
        private bool IsVoiceRecognition;
        private bool IsNotDelayAlarm;
        public bool IsFiveCount;
        public bool HasWakeUpText;
        private int alarmVolume;

        private bool IsFirstFeedbackNotification = false;
        private bool CanShowFeedbackNotification;
        private double PreviousRate;
        private double RecentRate;

        public bool CanShowReview;
        public DateTime AlarmTimeNow;

        public bool CanOpenOtherApp = false;
        public bool IsLinkOtherApp;
        public string _PackageName;

        public bool IsSuccess = false;
        public bool IsFinished = false;

        public Alarm alarm;
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
                IsFinished = true;
                CheckOnlyCountActivityAndRefreshAlarmManager();
            }
            else
            {
                SettingForAlarmActivity(savedInstanceState);
                CreatingForAlarmActivity();
            }
        }

        private void GetAlarmServiceAndRepository()
        {
            alarmService = App.AlarmService;
            alarmsRepo = alarmService.Repository;
            tonesRepo = App.AlarmToneRepo;
        }

        private void GetDataFromBundle(Bundle bundle)
        {
            name = (string)bundle.Get("name");
            toneName = (string)bundle.Get("toneName");
            IsAlarmOn = (bool)bundle.Get("IsAlarmOn");
            IsVibrateOn = (bool)bundle.Get("IsVibrateOn");
            IsRepeating = (bool)bundle.Get("IsRepeating");
            IsVoiceRecognition = (bool)bundle.Get("IsVoiceRecognition");
            IsNotDelayAlarm = (bool)bundle.Get("IsNotDelayAlarm");
            IsFiveCount = (bool)bundle.Get("IsFiveCount");
            HasWakeUpText = (bool)bundle.Get("HasWakeUpText");
            WakeUpText = (string)bundle.Get("WakeUpText");
            IsLinkOtherApp = (bool)bundle.Get("IsLinkOtherApp");
            _PackageName = (string)bundle.Get("PackageName");
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

            toastService.Show(GetString(Resource.String.IllCount));

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
            countDownForFiveSeconds = new CountDown(5000, 50, this, true);
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
            Console.WriteLine($"alarm.Name : {alarm.Name}");
            Console.WriteLine($"alarm.DaysId : {alarm.DaysId}");
            alarm.Days = alarmsRepo?.GetDaysOfWeek(alarm.DaysId);

            for (int i = 0; i < alarm.Days.AllDays.Length; i++)
            {
                Console.WriteLine($"alarm.Days[{i}] : {alarm.Days.AllDays[i]}");
            }

            if (alarm == null)
            {
                FinishAndRemoveTask();
            }
        }

        private void SetAndFindViewById()
        {
            notCountLayout = FindViewById<RelativeLayout>(Resource.Id.notCountLayout);
            countLayout = FindViewById<LinearLayout>(Resource.Id.countLayout);
            tellmeLayout = FindViewById<FrameLayout>(Resource.Id.tellmeLayout);

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

            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ko-KR":
                    timeOutTextView.Text = "60초 이내";
                    break;
                case "en-US":
                    timeOutTextView.Text = "Within 60 sec";
                    break;
                default:
                    timeOutTextView.Text = "Within 60 sec";
                    break;
            }
            countTextView.Text = "5.0";
            alarmTextView.Text = name;
            finishAlarmText.Text = name;
            timeTextView.Text = AlarmTimeNow.ToShortTimeString();

            if (IsNotDelayAlarm)
            {
                laterButton.Visibility = ViewStates.Gone;
            }

            SetAnimToTellmeView();
            tellmeView.Click += StartListening_Click;
            startButton.Click += StartButton_Click;
            laterButton.Click += LaterButton_Click; ;
        }

        private void SetAnimToTellmeView()
        {
            Animation myAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.bounce);

            tellmeLayout.StartAnimation(myAnim);
        }

        private void StartListening_Click(object sender, EventArgs e)
        {
            tellmeLayout.ClearAnimation();
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
            pleaseSayText.Text = GetString(Resource.String.PleaseWrite);
            SetVisibleOfStartButton();
            alarmEditText.RequestFocus();
            InputMethodManager imm = GetSystemService(InputMethodService) as InputMethodManager;
            imm.ShowSoftInput(alarmEditText, ShowFlags.Implicit);
            alarmEditText.SetSelection(alarmEditText.Text.Length);
            ShowToastDoNotMatchText();
        }
        private void ShowToastDoNotMatchText()
        {
            Toast.MakeText(ApplicationContext, GetString(Resource.String.DoNotMatchAlarmName), ToastLength.Long).Show();
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

            countDownForFailed?.Cancel();

            IsSuccess = true;

            var successStack = Preferences.Get("SuccessStack", 0);

            Preferences.Set("SuccessStack", successStack + 1);

            ShowResultDialog();
        }

        public void ShowResultDialog()
        {
            tellmeLayout.ClearAnimation();
            CreateRecord();

            IsFinished = true;

            if (IsLinkOtherApp)
            {
                CanOpenOtherApp = true;
            }

            SetResultToDialogResult();

            if (!IsFinishing)
            {
                resultDialog.Show();
            }
        }

        private void CreateRecord()
        {
            alarm.Time = new TimeSpan(AlarmTimeNow.Hour, AlarmTimeNow.Minute, 0);
            var record = new Record(alarm, IsSuccess);
            alarmsRepo.SaveRecord(record);
        }

        private void SetResultToDialogResult()
        {
            if (alarmsRepo == null) GetAlarmServiceAndRepository();

            var Records = alarmsRepo.RecordFromDB;
            var alarmRecords = Records.Where(a => a.Name == alarm.Name);

            CanShowFeedbackNotification = alarmRecords.Count() % 10 == 0;
            if (CanShowFeedbackNotification)
            {
                var RecentRecords = new List<Record>();

                var OrdererdRecords = alarmRecords.OrderByDescending(a => a.DateTime).ToList();
                for (int i = 0; i < 10; i++)
                {
                    var record = OrdererdRecords[i];

                    RecentRecords.Add(record);
                }

                for (int i = 0; i < 10; i++)
                {
                    OrdererdRecords.RemoveAt(0);
                }

                if (alarmRecords.Count() == 10)
                {
                    IsFirstFeedbackNotification = true;
                }
                else
                {
                    var successOrderedRecords = OrdererdRecords.FindAll(a => a.IsSuccess == true);
                    PreviousRate = (double)successOrderedRecords.Count / OrdererdRecords.Count;
                }

                var successRecentRecords = RecentRecords.FindAll(a => a.IsSuccess == true);
                RecentRate = (double)successRecentRecords.Count / RecentRecords.Count;

            }

            if (resultDialog == null) CreateResultDialog();

            TextView titleText = resultDialog.FindViewById<TextView>(Resource.Id.titleText);
            TextView messageText = resultDialog.FindViewById<TextView>(Resource.Id.messageText);
            Button confirmBtn = resultDialog.FindViewById<Button>(Resource.Id.confirmBtn);
            Button countButton = resultDialog.FindViewById<Button>(Resource.Id.countButton);
            Button failedBtn = resultDialog.FindViewById<Button>(Resource.Id.failedBtn);

            if (IsSuccess)
            {
                titleText.Text = GetString(Resource.String.Success);

                confirmBtn.Click += ResultDialog_ConfirmBtn_Click;
                countButton.Click += ResultDialog_CountBtn_Click;

                if (IsFiveCount)
                {
                    confirmBtn.Visibility = ViewStates.Gone;
                    countButton.Visibility = ViewStates.Visible;
                }
                else
                {
                    confirmBtn.Visibility = ViewStates.Visible;
                    countButton.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                titleText.Text = GetString(Resource.String.Failure);
                titleText.SetTextColor(Resources.GetColor(Resource.Color.failedTitleColor, Theme));
                messageText.SetTextColor(Resources.GetColor(Resource.Color.failedMessageColor, Theme));
                confirmBtn.Visibility = ViewStates.Gone;
                countButton.Visibility = ViewStates.Gone;
                failedBtn.Visibility = ViewStates.Visible;

                failedBtn.Click += ResultDialog_ConfirmBtn_Click;
            }

            if (alarmRecords.Count() == 1)
            {
                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        messageText.Text = CreateStringWhenFirstAlarm_ko_KR();
                        break;
                    case "en-US":
                        messageText.Text = CreateStringWhenFirstAlarm_en_US();
                        break;
                    default:
                        messageText.Text = CreateStringWhenFirstAlarm_en_US();
                        break;
                }
            }
            else
            {
                var successRecords = alarmRecords.Where(a => a.IsSuccess == true);
                double successRate = (double)successRecords.Count() / alarmRecords.Count();

                if (successRate > 0.7)
                {
                    bool HasShownReview = Preferences.Get("HasShownReview", false);
                    var successStack = Preferences.Get("SuccessStack", 0);
                    CanShowReview = alarmRecords.Count() > 4 && IsSuccess && !HasShownReview && successStack > 9;

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

                switch (CultureInfo.CurrentCulture.Name)
                {
                    case "ko-KR":
                        messageText.Text = $"'{alarm.Name}' 알람의 전체 성공률은 {successRate * 100:0.##}% 입니다.";
                        break;
                    case "en-US":
                        messageText.Text = $"The overall success rate of '{alarm.Name}' alarm is {successRate * 100:0.##}%.";
                        break;
                    default:
                        messageText.Text = $"The overall success rate of '{alarm.Name}' alarm is {successRate * 100:0.##}%.";
                        break;
                }
            }
        }

        private string CreateStringWhenFirstAlarm_ko_KR()
        {
            if (IsSuccess)
            {
                return $"첫 시작이 아주 좋네요! 5초 카운트가 끝나기 전에 '{alarm.Name}' 바로 시작해봐요!";
            }
            else
            {
                return $"첫 단추를 잘못 끼웠다고 상심 말아요. 다시 끼우면 되잖아요! 다음번엔 잘 해봐요!";
            }
        }
        private string CreateStringWhenFirstAlarm_en_US()
        {
            if (IsSuccess)
            {
                return $"Very good start! Let's start '{alarm.Name}' before the 5 second count is over!";
            }
            else
            {
                return $"Don't be bothered if you put the first button wrong. Just put it back! Do well next time!";
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
            if (!IsFinishing)
            {
                laterDialog.Show();
            }
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
                AlarmTone alarmTone = tonesRepo.Tones.FirstOrDefault(a => a.Name == toneName);

                if (alarmTone == null) { alarmTone = tonesRepo.Tones.First(); }

                _soundService?.PlayAudio(alarmTone, true, alarmVolume);
            }
        }

        private void SetVibrator()
        {
            if (IsVibrateOn)
            {
                _vibrator = Vibrator.FromContext(this);
                long[] mVibratePattern = new long[] { 0, 400, 1000, 600, 1000, 800, 1000, 1000 };
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var effect = VibrationEffect.CreateWaveform(mVibratePattern, 0);
                    _vibrator?.Vibrate(effect);
                }
                else
                {
                    _vibrator?.Vibrate(mVibratePattern, 0);
                }
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

            Console.WriteLine($"alarmName_AlarmActivity : {alarm.Name}");
            Console.WriteLine($"alarmId_AlarmActivity : {alarm.Id}");
            Console.WriteLine($"alarmDaysId_AlarmActivity : {alarm.DaysId}");
            Console.WriteLine($"alarmDays.Id_AlarmActivity : {alarm.Days.Id}");
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
            tellmeLayout.ClearAnimation();
            tellmeLayout.Visibility = ViewStates.Gone;
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

            NotificationAndroid.NotifyLaterAlarm(alarm, Intent);
        }

        private DateTime SetLaterAlarmAndGetLaterAlamrTime(int minutes)
        {
            var alarmTime = AlarmTimeNow.AddMinutes(minutes);

            Alarm.ChangeIsActive(alarm, true);

            alarm.LaterAlarmTime = alarmTime;

            return alarmTime;
        }

        private DateTime GetLaterAlarmTimeForNotDelayAlarm(int seconds)
        {
            var alarmTime = DateTime.Now.AddSeconds(seconds);
            Alarm.ChangeIsActive(alarm, true);
            alarm.LaterAlarmTime = alarmTime;
            return alarmTime;
        }

        private void CreateResultDialog()
        {
            resultDialog = new Dialog(this);
            resultDialog.SetContentView(Resource.Layout.ResultDialog);
            resultDialog.SetCancelable(false);
            resultDialog.SetCanceledOnTouchOutside(false);

            resultDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));

            adViewForResult = resultDialog.FindViewById<AdView>(Resource.Id.adView);
        }

        private void CreateAdView()
        {
            requestBuilder = new AdRequest.Builder().Build();

            //CreateRequestBuilderWhenTest();

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
            if (id == -100000)
            {
                FinishAndRemoveTask();
            }
        }

        private void AddWindowManagerFlags()
        {
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);
        }


        protected override void OnUserLeaveHint()
        {
            if (IsNotDelayAlarm)
            {
                var alarmTime = GetLaterAlarmTimeForNotDelayAlarm(3);
                AlarmHelper.SetLaterAlarm(alarm);
                alarmService.SaveAlarmAtLocal(alarm);
                Toast.MakeText(ApplicationContext, CreateDateString.CreateTimeRemainingString(alarmTime), ToastLength.Long).Show();
                FinishAndRemoveTask();
                return;
            }

            if (!IsFinished)
            {
                SetLaterAlarm(5);
                FinishAndRemoveTask();
            }

            base.OnUserLeaveHint();
        }

        public override void FinishAndRemoveTask()
        {
            TurnOffSoundAndVibration();
            countDownForFailed?.Cancel();
            ShowFeedbackNotification();
            SetAlarmComeback();

            base.FinishAndRemoveTask();

            if (CanOpenOtherApp)
            {
                OpenOtherApp();
            }
        }

        private void OpenOtherApp()
        {
            Intent launchIntent = PackageManager.GetLaunchIntentForPackage(_PackageName);
            if (launchIntent != null)
            {
                StartActivity(launchIntent);//null pointer check in case package name was not found
            }
        }

        #region MIC_PERMISSION

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == MY_PERMISSIONS_RECORD_AUDIO)
            {
                if (grantResults[0] != Permission.Granted)
                {
                    Toast.MakeText(ApplicationContext, GetString(Resource.String.RequestMicPermission), ToastLength.Short).Show();
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
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                StartVoiceRecognition();
                return;
            }

            if (CheckCallingOrSelfPermission(Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                // Should we show an explanation?
                if (ShouldShowRequestPermissionRationale(Manifest.Permission.RecordAudio))
                {
                    // Explain to the user why we need to read the contacts
                    Toast.MakeText(this, GetString(Resource.String.RequestMicPermission), ToastLength.Long).Show();
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

        #endregion

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

            if (!IsFinishing)
            {
                reviewDialog.Show();
            }
        }

        private void SetReviewToDialogResult()
        {
            var Records = alarmsRepo.RecordFromDB;
            var alarmRecords = Records.Where(a => a.Name == alarm.Name);

            reviewTitle = reviewDialog.FindViewById<TextView>(Resource.Id.reviewTitle);
            reviewText1 = reviewDialog.FindViewById<TextView>(Resource.Id.reviewText1);
            reviewText2 = reviewDialog.FindViewById<TextView>(Resource.Id.reviewText2);

            likeButtonLayout = reviewDialog.FindViewById<LinearLayout>(Resource.Id.likeButtonLayout);
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
            likeButtonLayout.Visibility = ViewStates.Gone;
            reviewButtonLayout.Visibility = ViewStates.Visible;

            reviewTitle.Text = GetString(Resource.String.GladToHelpYou);
            reviewText1.Text = GetString(Resource.String.WillKeepTrying);
            reviewText2.Text = GetString(Resource.String.WouldYouRecommend);
        }

        private void CancelBtn1_Click(object sender, EventArgs e)
        {
            likeButtonLayout.Visibility = ViewStates.Gone;
            feedbackButtonLayout.Visibility = ViewStates.Visible;

            reviewTitle.Text = GetString(Resource.String.ImSorry);
            reviewText1.Text = GetString(Resource.String.IllImprove);
            reviewText2.Text = GetString(Resource.String.CanYouTellMe);
        }

        private void ConfirmBtn2_Click(object sender, EventArgs e)
        {
            // 이메일 인텐트
            var intent = new Intent(Intent.ActionSend);
            intent.SetType("message/rfc822");
            intent.PutExtra(Intent.ExtraEmail, "save_us_222@naver.com");
            intent.PutExtra(Intent.ExtraSubject, $"{AlarmTimeNow.ToShortDateString()} {GetString(Resource.String.ErrorReport)}");

            try
            {
                StartActivity(Intent.CreateChooser(intent, "Email.."));
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
                googleAppStoreIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=" + _PackageName));

            }
            catch (ActivityNotFoundException)
            {
                googleAppStoreIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://play.google.com/store/apps/details?id=" + _PackageName));

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

        #region VOICE_RECOGNITION
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
                    Toast.MakeText(ApplicationContext, GetString(Resource.String.PleaseAllowAudioPermission), ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.Network:
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, GetString(Resource.String.NetworkError), ToastLength.Long).Show();
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
                    Toast.MakeText(ApplicationContext, GetString(Resource.String.ServerError), ToastLength.Long).Show();
                    break;

                case SpeechRecognizerError.SpeechTimeout:
                    SetTellmeBtnDefault();
                    Toast.MakeText(ApplicationContext, GetString(Resource.String.RecordingTimeOut), ToastLength.Long).Show();
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

        #endregion

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
            SetAnimToTellmeView();
        }

        private void SetTellmeBtnRecording()
        {
            tellmeView.SetImageResource(Resource.Drawable.ic_settings_voice);
            tellmeView.SetBackgroundResource(Resource.Drawable.background_tellmebtn_recording);
        }

        private void ShowFeedbackNotification()
        {
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ko-KR":
                    if (CanShowFeedbackNotification)
                    {
                        if (IsFirstFeedbackNotification)
                        {
                            NotificationAndroid.NotifyFirstFeedbackAlarm(alarm, RecentRate);
                        }
                        else
                        {
                            NotificationAndroid.NotifyFeedbackAlarm(alarm, PreviousRate, RecentRate);
                        }
                    }
                    break;
                case "en-US":
                    break;
                default:
                    break;
            }
        }

        private void SetAlarmComeback()
        {
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ko-KR":
                    AlarmHelper.SetComebackNotification(true);
                    break;
                case "en-US":
                    break;
                default:
                    break;
            }
        }
    }
}