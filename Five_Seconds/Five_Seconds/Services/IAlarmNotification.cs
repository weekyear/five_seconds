using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IAlarmNotification
    {
        void UpdateNotification();

        void CancelAllNotification();
    }
}
