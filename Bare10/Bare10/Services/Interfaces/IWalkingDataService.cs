using Bare10.Models.Walking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bare10.Services.Interfaces
{

    public interface IWalkingDataService
    {
        /// <summary>
        /// Fires whenever the status of the connection has changed
        /// </summary>
        event Action Connected;

        /// <summary>
        /// Fires whenever the Update completes
        /// </summary>
        event Action UpdateCompleted;

        /// <summary>
        /// Fires whenever there is new data available for the fitness history.
        /// </summary>
        event Action<TodaysWalkingModel> TodaysWalkingUpdated;

        /// <summary>
        /// Fires when the history for this week has been updated
        /// </summary>
        event Action<List<WalkingDayModel>> ThisWeekDaysUpdated;

        /// <summary>
        /// Fires when the history for last thirty days has been updated
        /// </summary>
        event Action<List<WalkingDayModel>> LastThirtyDaysUpdated;

        /// <summary>
        /// Is the walking data history current?
        /// </summary>
        bool HasCaughtUpToNow { get; }

        /// <summary>
        /// Connects to fitness data provider and asks for system authorization
        /// </summary>
        void ConnectToOSService(bool backgroundMode = false);

        bool GetIsConnectedToOSService();

        /// <summary>
        /// Updates the walking data available.
        /// </summary>
        /// <returns></returns>
        Task RequestUpdate();

        /// <summary>
        /// Gets todays walking history
        /// </summary>
        /// <returns>Current history</returns>
        TodaysWalkingModel GetTodaysHistory();

        /// <summary>
        /// Gets this weeks history
        /// </summary>
        /// <returns>A list of the days walked this week</returns>
        List<WalkingDayModel> GetWeekHistory();

        /// <summary>
        /// Gets the last 30 days history
        /// </summary>
        /// <returns>A list of the days walked last month</returns>
        List<WalkingDayModel> GetThirtyDaysHistory();
    }
}
