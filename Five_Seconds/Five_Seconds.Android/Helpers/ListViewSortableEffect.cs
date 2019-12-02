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
using Five_Seconds.Droid.Helpers;
using Five_Seconds.Helpers;
using Five_Seconds.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Beside")]
[assembly: ExportEffect(typeof(ListViewSortableEffect), nameof(ListViewSortableEffect))]
namespace Five_Seconds.Droid.Helpers
{
    public class ListViewSortableEffect : PlatformEffect
    {
        private DragListAdapter _dragListAdapter = null;
        private Xamarin.Forms.ListView element = null;

        protected override void OnAttached()
        {
            element = Element as Xamarin.Forms.ListView;

            if (Control is Android.Widget.ListView listView)
            {
                _dragListAdapter = new DragListAdapter(listView, element);
                listView.Adapter = _dragListAdapter;
                listView.SetOnDragListener(_dragListAdapter);
                listView.OnItemLongClickListener = _dragListAdapter;
            }
        }

        protected override void OnDetached()
        {
            if (Control is Android.Widget.ListView listView)
            {
                listView.Adapter = _dragListAdapter.WrappedAdapter;

                // TODO: Remove the attached listeners
            }
        }
        private void ItemLong_OnClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            if (element.BindingContext is AlarmsViewModel viewModel)
            {
                var alarmsFromVM = viewModel.Alarms;

                var alarm = alarmsFromVM[e.Position - 1];

                viewModel.IsSelectedMode = true;

                viewModel.ChangeIsSelectedOfAlarm(alarm);
            }
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == Sorting.IsSortableProperty.PropertyName)
            {
                _dragListAdapter.DragDropEnabled = Sorting.GetIsSortable(Element);
            }
        }
    }
}