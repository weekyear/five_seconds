using System;
using System.Collections.Generic;
using System.Text;

namespace Five_Seconds.Services
{
    public interface IDependencyService
    {
        T Get<T>() where T : class;
    }
}
