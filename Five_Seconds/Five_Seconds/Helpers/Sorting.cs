using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Five_Seconds.Helpers
{
    public static class Sorting
    {
        public static readonly BindableProperty IsSortableProperty =
            BindableProperty.CreateAttached(
                            "IsSortabble", typeof(bool),
                            typeof(ListViewSortableEffect), false,
                            propertyChanged: OnIsSortabbleChanged);

        public static bool GetIsSortable(BindableObject view)
        {
            return (bool)view.GetValue(IsSortableProperty);
        }

        public static void SetIsSortable(BindableObject view, bool value)
        {
            view.SetValue(IsSortableProperty, value);
        }

        static void OnIsSortabbleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is ListView view))
            {
                return;
            }

            view.Effects.Clear();
            if ((bool)newValue)
            {
                view.Effects.Add(new ListViewSortableEffect());
            }
        }

        public class ListViewSortableEffect : RoutingEffect
        {
            public ListViewSortableEffect() : base($"Beside.{nameof(ListViewSortableEffect)}")
            {

            }
        }
    }
}
