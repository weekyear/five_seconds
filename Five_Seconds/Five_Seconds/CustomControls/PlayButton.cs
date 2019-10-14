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

        public static readonly BindableProperty ImageSourcePlayOnProperty =
            BindableProperty.Create(nameof(ImageSourcePlay),
                typeof(string),
                typeof(PlayButton),
                "");

        public string ImageSourcePlay
        {
            get { return (string)GetValue(ImageSourcePlayOnProperty); }
            set { SetValue(ImageSourcePlayOnProperty, value); }
        }

        public static readonly BindableProperty ImageSourcePauseProperty =
            BindableProperty.Create(nameof(ImageSourcePause),
                typeof(string),
                typeof(PlayButton),
                "");

        public string ImageSourcePause
        {
            get { return (string)GetValue(ImageSourcePauseProperty); }
            set { SetValue(ImageSourcePauseProperty, value); }
        }

        //public event EventHandler IsPlayingChanged;

        static void OnIsPlayingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var button = (PlayButton)bindable;

            if (button.IsPlaying)
            {
                button.ImageSource = button.ImageSourcePause;
            }
            else
            {
                button.ImageSource = button.ImageSourcePlay;
            }

            //button.IsPlayingChanged?.Invoke(button, null);
        }
    }
}
