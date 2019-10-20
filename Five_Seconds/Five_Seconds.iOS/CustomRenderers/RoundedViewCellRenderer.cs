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

[assembly: ExportRenderer(typeof(RoundedViewCell), typeof(RoundedViewCellRenderer))]
namespace Five_Seconds.iOS.CustomRenderers
{
    public class RoundedViewCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            UITableViewCell viewCell = base.GetCell(item, reusableCell, tv);
            if (viewCell != null)
            {
                if (item is RoundedViewCell roundedCell)
                {
                    UIView custom = new UIView();
                    viewCell.ContentView.Layer.BackgroundColor = roundedCell.BackgroundColor.ToCGColor();
                    viewCell.ContentView.Layer.CornerRadius = new nfloat(roundedCell.CornerRadius);
                }
            }
            return viewCell;
        }
    }
}