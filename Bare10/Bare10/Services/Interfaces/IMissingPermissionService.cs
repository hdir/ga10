using System;
using Bare10.Content;

namespace Bare10.Services.Interfaces
{
    public enum MissingPermission
    {
        iOSLocationAlways,
        iOSMotionAndActivity,
        AndroidFineLocation,
        AndroidActivityRecognition,
        AndroidFitnessAccountLink,
        AndroidFitnessPermissions,
    }

    public interface IMissingPermissionService
    {
        event Action<MissingPermission, Action> MissingPermissionsAdded;
        event Action<MissingPermission> MissingPermissionsResolved;

        void ReportMissingPermission(MissingPermission permission, Action callback);
        void ResolvedMissingPermission(MissingPermission permission);
    }

    public class PermissionModel
    {
        public MissingPermission Type { get; }
        public string Description { get; }

        public PermissionModel(MissingPermission type, string description = "")
        {
            Type = type;
            Description = description;
        }
    }

    public static class MissingPermissionExtension
    {
        public static string ToDescription(this MissingPermission permission) => Permissions.GetDescription(permission);
    }
}
