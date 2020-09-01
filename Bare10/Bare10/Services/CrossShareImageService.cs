using Bare10.Services.Interfaces;
using System;
using System.Threading;

namespace Bare10.Services
{
    public static class CrossImageShareService
    {
        private static readonly Lazy<IShareImageService> Impl = new Lazy<IShareImageService>(CreateServiceImplementation, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the current platform specific IShareImageService implementation.
        /// </summary>
        public static IShareImageService Current
        {
            get
            {
                var val = Impl.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static IShareImageService CreateServiceImplementation()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new ShareImageService();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException();
        }
    }

}
