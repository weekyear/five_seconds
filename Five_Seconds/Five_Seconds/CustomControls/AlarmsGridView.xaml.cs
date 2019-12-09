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
        public AlarmsGridView()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}