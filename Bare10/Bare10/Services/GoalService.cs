using Bare10.Models;
using Bare10.Models.Walking;
using Bare10.Resources;
using Bare10.Services.Interfaces;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bare10.Content;
using static Bare10.TrackingEvents;

namespace Bare10.Services
{
    public class GoalService : IGoalService
    {
        private readonly IStorageService storageService;

        private readonly List<GoalModel> goals;

        private Task checkTask = null;

        private static GoalService current = null;
        public static GoalService Current
        {
            get
            {
                if(current == null)
                {
                    current = new GoalService();
                }
                return current;
            }
        }

        public GoalService()
        {
            storageService = StorageService.Current;

            goals = new List<GoalModel>
            {
                new GoalModel { Id = (int)Goal.TenMinutes, MinutesToReach = 10 },
                new GoalModel { Id = (int)Goal.TwentyMinutes, MinutesToReach = 20 },
                new GoalModel { Id = (int)Goal.ThirtyMinutes, MinutesToReach = 30 },
            };
        }

        public uint GetMinutesRequiredForCurrentGoal()
        {
            try
            {
                return goals.Find(g => (Goal)g.Id == Settings.CurrentGoal).MinutesToReach;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            return 0;
        }

        public async Task<bool> CheckIfGoalCompleted(TodaysWalkingModel todaysWalking)
        {
            if(checkTask == null || checkTask.IsCompleted)
            {
                try
                {
                    var goal = goals.Find(g => (Goal)g.Id == Settings.CurrentGoal);

                    var goalResult = new GoalStorageModel
                    {
                        Day = DateTime.Today,
                        Id = goal.Id,
                        HasBeenReached = false,
                        MinutesToReach = goal.MinutesToReach
                    };

                    // check if goal is complete
                    if (todaysWalking.minutesBriskWalkToday >= goal.MinutesToReach)
                    {
                        var previousToday = await storageService.GetTodaysGoal();

                        // check if already achieved goal today or if goal has changed to a greater goal
                        if (previousToday == null || previousToday.MinutesToReach < goal.MinutesToReach)
                        {
                            goalResult.HasBeenReached = true;
                            await storageService.StoreDailyGoalResult(goalResult);

                            // Return true (new goal achieved today)
                            Analytics.TrackEvent(GoalAccomplished,
                                    new GoalAccomplishedArgs(Enum.GetName(typeof(Goal), 
                                    (Goal)goal.Id)));
                            return true;
                        }
                    }

                } catch(Exception e)
                {
                    Crashes.TrackError(e);
                }
            }

            return false;
        }
    }
}
