using System;

namespace Bare10.Services
{
    /// <summary>
    /// Empty because all implementation is in base class,
    /// but we need this class name for lazy cross platform load
    /// 
    /// Sorry
    /// </summary>
    public class UpdateService : UpdateServiceBase
    {
        private static Lazy<UpdateServiceBase> current = new Lazy<UpdateServiceBase>(() => new UpdateService());
        public static UpdateServiceBase Current => current.Value;

    }
}