using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ListView = Xamarin.Forms.ListView;

[assembly: ExportRenderer(typeof(NoRippleListView), typeof(NoRippleListViewRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class NoRippleListViewRenderer : ListViewRenderer
    {
        public NoRippleListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            Control.SetSelector(Resource.Layout.no_selector);
        }
    }
}