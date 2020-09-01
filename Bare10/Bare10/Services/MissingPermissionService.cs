using Bare10.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Bare10.Services
{
    public class MissingPermissionService : IMissingPermissionService
    {
        public event Action<MissingPermission, Action> MissingPermissionsAdded;
        public event Action<MissingPermission> MissingPermissionsResolved;

        private readonly Dictionary<MissingPermission, Action> _missingPermissionCallbacks = 
            new Dictionary<MissingPermission, Action>();

        public void ReportMissingPermission(MissingPermission permission, Action callback)
        {
            if(!_missingPermissionCallbacks.ContainsKey(permission))
            {
                _missingPermissionCallbacks.Add(permission, callback);
            }
            MissingPermissionsAdded?.Invoke(permission, callback);
        }

        public void ResolvedMissingPermission(MissingPermission permission)
        {
            if (_missingPermissionCallbacks.ContainsKey(permission))
            {
                _missingPermissionCallbacks.Remove(permission);
            }
            MissingPermissionsResolved?.Invoke(permission);
        }
    }
}
