using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.CustomControls
{
    public interface ISearchPage
    {
        //void OnSearchBarTextChanged(string text);
        void OnSearchBarTextSubmited(string text);
        //event EventHandler<string> SearchBarTextChanged;
        event EventHandler<string> SearchBarTextSubmited;
    }
}
