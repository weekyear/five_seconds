using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Five_Seconds.CustomControls;
using Five_Seconds.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace Five_Seconds.iOS.CustomRenderers
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            //var view = item as CustomViewCell;
            //cell.SelectedBackgroundView = new UIView
            //{
            //    BackgroundColor = view.SelectedBackgroundColor.ToUIColor(),
            //};
            return cell;
        }
    }
}