using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.CustomControls
{
    public interface ISearchPage
    {
        void OnSearchBarTextSubmited(string text);
        event EventHandler<string> SearchBarTextSubmited;
    }
}
