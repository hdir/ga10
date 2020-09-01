using Bare10.Services.Interfaces;
using System;
using System.Threading;

namespace Bare10.Services
{
    public static class CrossScreenshotService
    {
        private static readonly Lazy<IScreenshotService> Impl = new Lazy<IScreenshotService>(CreateServiceImplementation, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the current platform specific ILocalNotifications implementation.
        /// </summary>
        public static IScreenshotService Current
        {
            get
            {
                var val = Impl.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static IScreenshotService CreateServiceImplementation()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new ScreenshotService();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException();
        }
    }
}
