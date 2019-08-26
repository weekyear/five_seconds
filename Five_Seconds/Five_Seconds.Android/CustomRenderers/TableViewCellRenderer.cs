using System;
using System.Collections.Generic;
using System.ComponentModel;
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

[assembly: ExportRenderer(typeof(TableViewCell), typeof(TableViewCellRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class TableViewCellRenderer : ViewCellRenderer
    {
        Android.Views.View _nativeCell;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            _nativeCell = base.GetCellCore(item, convertView, parent, context);
            setStyle();

            return _nativeCell;
        }

        private void setStyle()
        {
            var formsCell = Cell as TableViewCell;
            if (formsCell == null)
                return;

            _nativeCell.Clickable = !formsCell.AllowHighlight;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == TableViewCell.AllowHighlightProperty.PropertyName)
            {
                setStyle();
            }
        }
    }
}