using System;
using System.ComponentModel;
using Android.Content;
using Android.Views;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedViewCell), typeof(RoundedViewCellRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class RoundedViewCellRenderer : ViewCellRenderer
    {
        Android.Views.View _nativeCell;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            _nativeCell = base.GetCellCore(item, convertView, parent, context);

            SetSelected();

            return _nativeCell;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == RoundedViewCell.IsSelectedProperty.PropertyName)
            {
                try
                {
                    SetSelected();
                }
                catch (ObjectDisposedException error) 
                {
                    Console.WriteLine(error.Message);
                }
            }
        }

        private void SetSelected()
        {
            if (!(Cell is RoundedViewCell formsCell))
                return;

            int backgroundResource;

            if (formsCell.IsSelected)
            {
                backgroundResource = Resource.Drawable.ripple_rounded_viewcell_selected;
            }
            else
            {
                backgroundResource = Resource.Drawable.ripple_rounded_viewcell;
            }

            _nativeCell?.SetBackgroundResource(backgroundResource);
        }
    }
}