using Bare10.Services.Interfaces;
using System;
using System.Threading;

namespace Bare10.Services
{
    public static class CrossUpdateService
    {
        private static readonly Lazy<IUpdateService> Impl = new Lazy<IUpdateService>(CreateServiceImplementation, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the current platform specific ILocalNotifications implementation.
        /// </summary>
        public static IUpdateService Current
        {
            get
            {
                var val = Impl.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static IUpdateService CreateServiceImplementation()
        {
#if NETSTANDARD2_0
            return null;
#else
            return UpdateService.Current;
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException();
        }
    }
}
