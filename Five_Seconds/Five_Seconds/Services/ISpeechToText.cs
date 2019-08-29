using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Five_Seconds.Services
{
    public interface ISpeechToText
    {
        Task<string> SpeechToTextAsync();
    }
}
