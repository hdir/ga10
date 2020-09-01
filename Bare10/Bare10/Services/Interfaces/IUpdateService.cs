using System;
using System.Threading.Tasks;

namespace Bare10.Services.Interfaces
{
    public enum UpdateMode
    {
        Foreground = 1,
        Background = 2
    }

    public interface IUpdateService
    {
        /// <summary>
        /// Event fired when update mode has changed
        /// </summary>
        event Action<UpdateMode> UpdateModeChanged;
        /// <summary>
        /// Event fired when an update cycle has been completed
        /// </summary>
        event Action UpdateCompleted;
        /// <summary>
        /// Updates Walking data and then achievements
        /// </summary>
        /// <returns></returns>
        Task UpdateAllDataServices();
        /// <summary>
        /// Sets whether or not we're polling for data when app is active or not
        /// </summary>
        /// <param name="mode">The mode to enter</param>
        void SetUpdateMode(UpdateMode mode);

        UpdateMode GetUpdateMode();
    }
}
