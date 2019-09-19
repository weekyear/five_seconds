using System;

namespace Five_Seconds.Services
{
    public interface IFileLocator
    {
        void OpenFileLocator();

        event Action<string> FileChosen;
    }
}
