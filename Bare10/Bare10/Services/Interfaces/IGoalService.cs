using Bare10.Models.Walking;
using System.Threading.Tasks;

namespace Bare10.Services.Interfaces
{
    public interface IGoalService
    {
        /// <summary>
        /// Get the minutes required for the currently set goal
        /// </summary>
        /// <returns>The number of minutes required for the currently set goal</returns>
        uint GetMinutesRequiredForCurrentGoal();
        /// <summary>
        /// Check if the currently set goal has been completed
        /// </summary>
        /// <returns>True if todays goal is completed for the first time</returns>
        Task<bool> CheckIfGoalCompleted(TodaysWalkingModel todaysWalking);
        
    }
}
