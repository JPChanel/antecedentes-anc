using System;
using log4net;

namespace ANC.Comun.Log4net
{
    public class LogAntecedentes
    {
        private static ILog logger;
        private static bool isConfigured = false;

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
        ~LogAntecedentes()
        {
            Dispose(false);
        }

        public static ILog Logger()
        {
            if (!isConfigured)
            {
                logger = LogManager.GetLogger(typeof(LogAntecedentes));
                log4net.Config.XmlConfigurator.Configure();
            }

            return logger;
        }

    }
}
