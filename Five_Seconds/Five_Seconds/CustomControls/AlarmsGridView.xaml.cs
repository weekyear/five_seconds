using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Five_Seconds.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlarmsGridView : ContentView
    {
        private bool Execute { get; set; }

        public AlarmsGridView()
        {
            InitializeComponent();

            Execute = true;

            var name = AlarmNameLabel.Text;

            var nameLabelWidth = AlarmNameLabel.ScaleX;
            var gridLabelWidth = AlarmsGrid.ScaleX * 0.85;

            if (nameLabelWidth > gridLabelWidth)
            {
                //Device.StartTimer(TimeSpan.FromMilliseconds(25), () =>
                //{
                //    AlarmNameLabel.TranslationX -= 3f;

                //    if (AlarmNameLabel.TranslationX < -Width)
                //    {
                //        AlarmNameLabel.TranslationX = AlarmNameLabel.Width;
                //    }

                //    return Execute;
                //});
            }
        }
    }
}