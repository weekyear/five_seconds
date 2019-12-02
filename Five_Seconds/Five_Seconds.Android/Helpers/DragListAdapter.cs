using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Five_Seconds.Helpers;
using Five_Seconds.Models;
using Five_Seconds.ViewModels;

namespace Five_Seconds.Droid.Helpers
{
    public class DragListAdapter : BaseAdapter, IWrapperListAdapter, AdapterView.IOnItemLongClickListener, View.IOnDragListener
    {
        private readonly ListView _listView;

        private readonly Xamarin.Forms.ListView _element;

        private readonly List<View> _translatedItems = new List<View>();

        public DragListAdapter(ListView listView, Xamarin.Forms.ListView element)
        {
            _listView = listView;
            // NOTE: careful, the listAdapter might not always be an IWrapperListAdapter
            //WrappedAdapter = (_listView.Adapter is IWrapperListAdapter) ? ((IWrapperListAdapter)_listView.Adapter).WrappedAdapter : _listView.Adapter;
            //_listAdapter = (_listView.Adapter is IWrapperListAdapter) ? (_listView.Adapter as IWrapperListAdapter).WrappedAdapter : _listView.Adapter;

            if (_listView.Adapter is HeaderViewListAdapter ad)
            {
                WrappedAdapter = ad.WrappedAdapter;
            }

            _element = element;
        }

        public bool DragDropEnabled { get; set; } = true;

        //... removed for brevity
        #region IWrapperListAdapter Members;

        public IListAdapter WrappedAdapter { get; }

        public override int Count => WrappedAdapter.Count;

        public override bool HasStableIds => WrappedAdapter.HasStableIds;

        public override bool IsEmpty => WrappedAdapter.IsEmpty;

        public override int ViewTypeCount => WrappedAdapter.ViewTypeCount;

        public IListAdapter ListAdapter => WrappedAdapter;

        public override bool AreAllItemsEnabled() => WrappedAdapter.AreAllItemsEnabled();

        public override Java.Lang.Object GetItem(int position)
        {
            return WrappedAdapter.GetItem(position);
        }

        public override long GetItemId(int position)
        {
            return WrappedAdapter.GetItemId(position);
        }

        public override int GetItemViewType(int position)
        {
            return WrappedAdapter.GetItemViewType(position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = WrappedAdapter.GetView(position, convertView, parent);
            view.SetOnDragListener(this);
            return view;
        }

        public override bool IsEnabled(int position)
        {
            return WrappedAdapter.IsEnabled(position);
        }

        public override void RegisterDataSetObserver(DataSetObserver observer)
        {
            base.RegisterDataSetObserver(observer);
            WrappedAdapter.RegisterDataSetObserver(observer);
        }

        public override void UnregisterDataSetObserver(DataSetObserver observer)
        {
            base.UnregisterDataSetObserver(observer);
            WrappedAdapter.UnregisterDataSetObserver(observer);
        }

        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            object selectedItem = ItemLongClickAndGetSelectedItem((int)id);

            // Creating drag state
            DragItem dragItem = new DragItem(NormalizeListPosition(position), view, selectedItem);

            // Creating a blank clip data object (we won't depend on this) 
            var data = ClipData.NewPlainText(string.Empty, string.Empty);

            // Creating the default drag shadow for the item (the translucent version of the view)
            // NOTE: Can create a custom view in order to change the dragged item view
            View.DragShadowBuilder shadowBuilder = new View.DragShadowBuilder(view);

            // Setting the original view cell to be invisible
            view.Visibility = ViewStates.Invisible;

            // NOTE: this method is introduced in Android 24, for earlier versions the StartDrag method should be used
            view.StartDragAndDrop(data, shadowBuilder, dragItem, 0);

            return true;
        }

        private object ItemLongClickAndGetSelectedItem(int id)
        {
            object selectedItem = null;

            if (_element.BindingContext is AlarmsViewModel)
            {
                selectedItem = ((IList)_element.ItemsSource)[id];

                Item_LongClick_GoalsMainVM(_element, selectedItem);
            }

            return selectedItem;
        }

        private void Item_LongClick_GoalsMainVM(Xamarin.Forms.ListView element, object selectedItem)
        {
            if (element.BindingContext is AlarmsViewModel viewModel)
            {
                var alarm = selectedItem as Alarm;

                if (!viewModel.IsSelectedMode)
                {
                    viewModel.ChangeIsSelectedOfAlarm(alarm);
                }

                viewModel.IsSelectedMode = true;
            }
        }

        public bool OnDrag(View v, DragEvent e)
        {
            switch (e.Action)
            {
                case DragAction.Started:
                    break;
                case DragAction.Entered:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Entered from {v.GetType()}");

                    if (!(v is ListView))
                    {
                        var dragItem = (DragItem)e.LocalState;

                        var targetPosition = InsertOntoView(v, dragItem);

                        dragItem.Index = targetPosition;

                        // Keep a list of items that has translation so we can reset
                        // them once the drag'n'drop is finished.
                        _translatedItems.Add(v);
                        _listView.Invalidate();
                    }
                    break;
                case DragAction.Location:
                    break;
                case DragAction.Exited:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Entered from {v.GetType()}");
                    break;
                case DragAction.Drop:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Drop from {v.GetType()}");
                    break;
                case DragAction.Ended:
                    System.Diagnostics.Debug.WriteLine($"DragAction.Ended from {v.GetType()}");

                    if (!(v is ListView))
                    {
                        return false;
                    }

                    var mobileItem = e.LocalState as DragItem;

                    mobileItem.View.Visibility = ViewStates.Visible;

                    foreach (var view in _translatedItems)
                    {
                        view.TranslationY = 0;
                    }

                    _translatedItems.Clear();

                    if (_element.ItemsSource is IOrderable orderable)
                    {
                        orderable.ChangeOrdinal(mobileItem.OriginalIndex, mobileItem.Index);
                    }

                    break;
            }

            return true;
        }

        private int InsertOntoView(View view, DragItem item)
        {
            var positionEntered = GetListPositionForView(view);
            var correctedPosition = positionEntered;

            // If the view already has a translation, we need to adjust the position
            // If the view has a positive translation, that means that the current position
            // is actually one index down then where it started.
            // If the view has a negative translation, that means it actually moved
            // up previous now we will need to move it down.
            if (view.TranslationY > 0)
            {
                correctedPosition += 1;
            }
            else if (view.TranslationY < 0)
            {
                correctedPosition -= 1;
            }

            // If the current index of the dragging item is bigger than the target
            // That means the dragging item is moving up, and the target view should
            // move down, and vice-versa
            var translationCoef = item.Index > correctedPosition ? 1 : -1;

            // We translate the item as much as the height of the drag item (up or down)
            var translationTarget = view.TranslationY + (translationCoef * item.View.Height);

            ObjectAnimator anim = ObjectAnimator.OfFloat(view, "TranslationY", view.TranslationY, translationTarget);
            anim.SetDuration(100);
            anim.Start();

            return correctedPosition;
        }

        private int GetListPositionForView(View view)
        {
            return NormalizeListPosition(_listView.GetPositionForView(view));
        }

        private int NormalizeListPosition(int position)
        {
            // We do not want to count the headers into the item source index
            return position - _listView.HeaderViewsCount;
            //return position - _listView.HeaderViewsCount - 1;
        }

        #endregion

        public class DragItem : Java.Lang.Object
        {
            /// <summary>
            /// Initializes a new instance of the  class.
            /// </summary>
            /// 
            /// The initial index for the data item.
            /// 
            /// 
            /// The view element that is being dragged.
            /// 
            /// 
            /// The data item that is bound to the view.
            /// 
            public DragItem(int index, View view, object dataItem)
            {
                OriginalIndex = Index = index;
                View = view;
                Item = dataItem;
            }

            /// <summary>
            /// Gets or sets the current index for the data item.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Gets the original index for the data item
            /// </summary>
            public int OriginalIndex { get; }

            /// <summary>
            /// Gets the data item that is being dragged
            /// </summary>
            public object Item { get; }

            /// <summary>
            /// Gets the view that is being dragged
            /// </summary>
            public View View { get; }
        }
    }
}