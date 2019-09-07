using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.CustomControls
{
    public class PlayButton : Button
    {
        public static readonly BindableProperty IsPlayingProperty =
            BindableProperty.Create(nameof(IsPlaying),
                typeof(bool),
                typeof(PlayButton),
                false,
                BindingMode.TwoWay,
                propertyChanged: OnIsPlayingChanged);

        public bool IsPlaying
        {
            get
            {
                return (bool)GetValue(IsPlayingProperty);
            }
            set
            {
                SetValue(IsPlayingProperty, value);
            }
        }

        public static readonly BindableProperty SourceIsPlayingOnProperty =
            BindableProperty.Create(nameof(SourceIsPlaying),
                typeof(string),
                typeof(PlayButton),
                "");

        public string SourceIsPlaying
        {
            get { return (string)GetValue(SourceIsPlayingOnProperty); }
            set { SetValue(SourceIsPlayingOnProperty, value); }
        }

        public static readonly BindableProperty SourceIsPauseProperty =
            BindableProperty.Create(nameof(SourceIsPause),
                typeof(string),
                typeof(PlayButton),
                "");

        public string SourceIsPause
        {
            get { return (string)GetValue(SourceIsPauseProperty); }
            set { SetValue(SourceIsPauseProperty, value); }
        }

        public event EventHandler IsPlayingChanged;

        static void OnIsPlayingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var button = (PlayButton)bindable;
            button.IsPlayingChanged?.Invoke(button, null);
        }
    }
}
