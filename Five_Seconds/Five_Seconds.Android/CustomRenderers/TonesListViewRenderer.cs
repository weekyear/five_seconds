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
using Five_Seconds.Models;
using Five_Seconds.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ListView = Xamarin.Forms.ListView;

[assembly: ExportRenderer(typeof(TonesListView), typeof(TonesListViewRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class TonesListViewRenderer : ListViewRenderer
    {
        private ListView listView;
        public TonesListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
            Control.SetSelector(Resource.Layout.no_selector);

            listView = e.NewElement;

            Control.ItemLongClick += new EventHandler<AdapterView.ItemLongClickEventArgs>(ItemLong_OnClick);
        }

        private void ItemLong_OnClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var settingToneViewModel = listView.BindingContext as SettingToneViewModel;

            var allAlarmTones = settingToneViewModel.AllAlarmTones;

            var settingTone = allAlarmTones[e.Position - 1];

            settingToneViewModel.ConfirmDeleteTone(settingTone);
        }
    }
}