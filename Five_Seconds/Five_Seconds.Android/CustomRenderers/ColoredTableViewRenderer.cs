using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.CustomControls;
using Five_Seconds.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ListView = Android.Widget.ListView;

[assembly: ExportRenderer(typeof(ColoredTableView), typeof(ColoredTableViewRenderer))]
namespace Five_Seconds.Droid.CustomRenderers
{
    public class ColoredTableViewRenderer : TableViewRenderer
    {
        public ColoredTableViewRenderer(Context context) : base(context)
        {
        }

        protected override TableViewModelRenderer GetModelRenderer(Android.Widget.ListView listView, TableView view)
        {
            return new CustomHeaderTableViewModelRenderer(Context, listView, view);
        }

        private class CustomHeaderTableViewModelRenderer : TableViewModelRenderer
        {
            private readonly ColoredTableView _coloredTableView;

            public CustomHeaderTableViewModelRenderer(Context context, Android.Widget.ListView listView, TableView view) : base(context, listView, view)
            {
                _coloredTableView = view as ColoredTableView;
            }

            public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
            {
                var view = base.GetView(position, convertView, parent);

                var element = GetCellForPosition(position);

                // section header will be a TextCell
                if (element.GetType() == typeof(TextCell))
                {
                    try
                    {
                        // Get the textView of the actual layout
                        var textView = ((((view as LinearLayout).GetChildAt(0) as LinearLayout).GetChildAt(1) as LinearLayout).GetChildAt(0) as TextView);

                        // get the divider below the header
                        var divider = (view as LinearLayout).GetChildAt(1);

                        // Set the color
                        textView.SetTextColor(_coloredTableView.GroupHeaderColor.ToAndroid());
                        divider.SetBackgroundColor(_coloredTableView.GroupHeaderColor.ToAndroid());
                    }
                    catch (Exception) { }
                }

                return view;
            }
        }
    }
}