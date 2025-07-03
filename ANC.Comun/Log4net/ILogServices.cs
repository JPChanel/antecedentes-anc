using System;
using log4net;

namespace ANC.Comun.Log4net
{
    public interface ILogServices : IDisposable
    {
        ILog Logger();
    }
}   
