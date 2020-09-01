using Bare10.Services.Interfaces;
using System;
using System.Threading;

namespace Bare10.Services
{
    public static class CrossWalkingDataService
    {
        private static readonly Lazy<IWalkingDataService> Impl = new Lazy<IWalkingDataService>(CreateServiceImplementation, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the current platform specific ILocalNotifications implementation.
        /// </summary>
        public static IWalkingDataService Current
        {
            get
            {
                var val = Impl.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static IWalkingDataService CreateServiceImplementation()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new WalkingDataService();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException();
        }
    }
}

