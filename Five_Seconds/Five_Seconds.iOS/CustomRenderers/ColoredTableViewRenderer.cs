using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Five_Seconds.CustomControls;
using Five_Seconds.iOS.CustomRenderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ColoredTableView), typeof(ColoredTableViewRenderer))]
namespace Five_Seconds.iOS.CustomRenderers
{
    public class ColoredTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                return;

            var tableView = Control as UITableView;
            var coloredTableView = Element as ColoredTableView;
            tableView.WeakDelegate = new CustomHeaderTableModelRenderer(coloredTableView);
        }

        private class CustomHeaderTableModelRenderer : UnEvenTableViewModelRenderer
        {
            private readonly ColoredTableView _coloredTableView;
            public CustomHeaderTableModelRenderer(TableView model) : base(model)
            {
                _coloredTableView = model as ColoredTableView;
            }
            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                return new UILabel()
                {
                    Text = TitleForHeader(tableView, section),
                    TextColor = _coloredTableView.GroupHeaderColor.ToUIColor(),
                    TextAlignment = UITextAlignment.Center
                };
            }
        }
    }
}