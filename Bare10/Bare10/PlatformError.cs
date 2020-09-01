using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bare10
{
    public class PlatformError
    {
        public static void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            Exception e = new Exception("TaskSchedulerUnobservedTaskException", args.Exception);
            LogUnhandledException(e);
        }

        public static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = new Exception("CurrentDomainUnhandledException", args.ExceptionObject as Exception);
            LogUnhandledException(e);
        }

        private static void LogUnhandledException(Exception e)
        {
            Debug.WriteLine($"Unhandled Exception occured: {e.Message}");
            if (e.InnerException != null)
            {
                Debug.WriteLine($"Unhandled InnerException: {e.InnerException}");
            }
        }
    }
}
