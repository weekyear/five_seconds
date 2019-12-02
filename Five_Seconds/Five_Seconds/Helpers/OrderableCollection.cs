using Five_Seconds.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Five_Seconds.Helpers
{
    public class OrderableCollection<T> : ObservableCollection<T>, IOrderable
    {
        public OrderableCollection() : base()
        {
        }

        public event EventHandler OrderChanged;

        public void ChangeOrdinal(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex) return;

            var priorIndex = oldIndex;
            var latterIndex = newIndex;

            var changedItem = Items[oldIndex];
            if (newIndex < oldIndex)
            {
                // add one to where we delete, because we're increasing the index by inserting
                priorIndex += 1;
            }
            else
            {
                // add one to where we insert, because we haven't deleted the original yet
                latterIndex += 1;
            }

            Items.Insert(latterIndex, changedItem);
            Items.RemoveAt(priorIndex);

            OrderChanged?.Invoke(this, EventArgs.Empty);

            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Move,
                    changedItem,
                    newIndex,
                    oldIndex));

            if (Items is IEnumerable<Alarm> alarms)
            {
                if (alarms != null)
                {
                    int i = 0;
                    foreach (var alarm in alarms)
                    {
                        alarm.Index = i++;
                    }
                }

                App.Service.SaveAlarmsAtLocal(alarms);
            }
        }
    }
}
