using System;
using log4net;

namespace ANC.Comun.Log4net
{
    public class LogServices : ILogServices
    {
        private ILog logger;
        private bool isConfigured = false;

        public LogServices()
        {
            if (!isConfigured)
            {
                logger = LogManager.GetLogger(typeof(LogServices));
                log4net.Config.XmlConfigurator.Configure();
            }
        }

        public ILog Logger()
        {
            return logger;
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            // Dispose of managed resources here.
            if (disposing)
            {
                logger = null;
            }

            // Dispose of any unmanaged resources not wrapped in safe handles.
            disposed = true;
        }

        ~LogServices()
        {
            Dispose(false);
        }

    }
}
