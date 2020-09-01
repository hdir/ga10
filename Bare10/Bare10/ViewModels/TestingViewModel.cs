using Bare10.Models.Walking;
using Bare10.Resources;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.Utils.Enum;
using Bare10.ViewModels.Base;
using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bare10.ViewModels
{
    public class TestingViewModel : SettingViewModel
    {
        private readonly IWalkingDataService walkingDataService;
        private readonly IStorageService storageService;
        private readonly IGoalService goalService;

        #region Commands
        public IMvxAsyncCommand RequestWalkingData { get; private set; }
        public IMvxAsyncCommand<Table> GenerateTestData { get; private set; }
        public IMvxAsyncCommand<Table> DeleteStoredData { get; private set; }
        public IMvxAsyncCommand DeleteAllAndResetStartDate { get; private set; }
        public IMvxAsyncCommand CompleteTodaysGoal { get; private set; }
        #endregion

        
        #region Properties

        private Table selectedTables;
        public Table SelectedTables
        {
            get => selectedTables;
            set
            {
                selectedTables = value;
                RaisePropertyChanged(() => SelectedTables);
            }
        }

        private bool selectTodaysWalking;
        public bool SelectTodaysWalking
        {
            get => selectTodaysWalking;
            set
            {
                selectTodaysWalking = value;
                if(selectTodaysWalking)
                {
                    SelectedTables = SelectedTables.Set(Table.WalkingDataPoints);
                }
                else
                {
                    SelectedTables = SelectedTables.Unset(Table.WalkingDataPoints);
                }
                RaisePropertyChanged(() => SelectTodaysWalking);
            }
        }

        private bool selectDailyWalking;
        public bool SelectDailyWalking
        {
            get => selectDailyWalking;
            set
            {
                selectDailyWalking = value;
                if(selectDailyWalking)
                {
                    SelectedTables = SelectedTables.Set(Table.WalkingDays);
                }
                else
                {
                     SelectedTables = SelectedTables.Unset(Table.WalkingDays);
                }
                RaisePropertyChanged(() => SelectDailyWalking);
            }
        }

        private bool selectAchievements;
        public bool SelectAchievements
        {
            get => selectAchievements;
            set
            {
                selectAchievements = value;
                if(selectAchievements)
                {
                    SelectedTables = SelectedTables.Set(Table.Achievements);
                }
                else
                {
                    SelectedTables = SelectedTables.Unset(Table.Achievements);
                }
                RaisePropertyChanged(() => SelectAchievements);
            }
        }

        private bool selectGoals;
        public bool SelectGoals
        {
            get => selectGoals;
            set
            {
                selectGoals = value;
                if(selectGoals)
                {
                    SelectedTables = SelectedTables.Set(Table.Goals);
                }
                else
                {
                    SelectedTables = SelectedTables.Unset(Table.Goals);
                }
                RaisePropertyChanged(() => SelectGoals);
            }
        }

        public string Output => TodaysWalking + DailyWalking;

        private string todaysWalking = "Empty";
        public string TodaysWalking
        {
            get => todaysWalking;
            set
            {
                todaysWalking = value;
                RaisePropertyChanged(() => Output);
            }
        }

        private string dailyWalking =  "Empty";
        public string DailyWalking
        {
            get => dailyWalking;
            set
            {
                dailyWalking = value;
                RaisePropertyChanged(() => Output);
            }
        }

        private string log = string.Empty;
        public string Log
        {
            get => log;
            set
            {
                log = value;
                RaisePropertyChanged(() => Log);
            }
        }
        #endregion

        public TestingViewModel()
        {
            walkingDataService = CrossServiceContainer.WalkingDataService;
            storageService = StorageService.Current;
            goalService = GoalService.Current;

            walkingDataService.TodaysWalkingUpdated += h =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Last update was performed {0}\n", DateTime.Now.ToLongTimeString());
                foreach (var w in h.todaysWalking)
                {
                    sb.AppendLine(w.ToString());
                }
                TodaysWalking = sb.ToString();
            };

            walkingDataService.LastThirtyDaysUpdated += walking =>
            {
                StringBuilder sb = new StringBuilder();
                foreach (var d in walking)
                {
                    sb.AppendLine(d.ToString());
                }
                DailyWalking = sb.ToString();
            };

            RequestWalkingData = new MvxAsyncCommand(async () => await walkingDataService.RequestUpdate());
            GenerateTestData = new MvxAsyncCommand<Table>(async tables => await storageService.GenerateTestData(tables));
            DeleteStoredData = new MvxAsyncCommand<Table>(async tables => await storageService.DeleteDataBaseTables(tables));
            DeleteAllAndResetStartDate = new MvxAsyncCommand(async () => {
                await storageService.DeleteDataBaseTables(Table.All);
                Settings.StartDate = DateTime.Now;
                });

            CompleteTodaysGoal = new MvxAsyncCommand(async () =>
            {
                var today = walkingDataService.GetTodaysHistory();
                int minutesRemaining = (int)goalService.GetMinutesRequiredForCurrentGoal() - (int)today.minutesBriskWalkToday;
                if (minutesRemaining <= 0)
                    return;

                List<WalkingDataPointModel> dataPoints = new List<WalkingDataPointModel>();
                //find some gaps
                DateTime lastInsertedStop = DateTime.Today.AddHours(-1);
                //will poll current and next datapoint
                if(today.todaysWalking.Count == 0)
                {
                    var start = DateTime.Today;
                    dataPoints.Add(new WalkingDataPointModel(start, start.AddMinutes(minutesRemaining), Config.BriskPaceMetersPerSecond));
                }
                if (today.todaysWalking.Count == 1)
                {
                    var currentHour = 0;
                    while(currentHour < 24)
                    {
                        var dp = today.todaysWalking[0];
                        if(currentHour < dp.Start.Hour)
                        {
                            var start = DateTime.Today.AddHours(currentHour);
                            dataPoints.Add(new WalkingDataPointModel(start, start.AddMinutes(minutesRemaining), Config.BriskPaceMetersPerSecond));
                            break;
                        }
                        currentHour++;
                    }
                }

                for (int i = 0; i < today.todaysWalking.Count - 1; ++i)
                {
                    var dp = today.todaysWalking[i];
                    var nextDp = today.todaysWalking[i + 1];
                    //empty hour available
                    var upComingHour = (lastInsertedStop.Hour + 1) % 24;
                    if (upComingHour < dp.Start.Hour)
                    {
                        var minutesToAdd = minutesRemaining % 60;
                        var start = DateTime.Today.AddHours(upComingHour);
                        lastInsertedStop = start.AddMinutes(minutesToAdd);
                        dataPoints.Add(new WalkingDataPointModel(start, lastInsertedStop, Config.BriskPaceMetersPerSecond));
                        minutesRemaining -= minutesToAdd;
                        if(minutesRemaining > 0)
                        {
                            i--;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //before current dp
                        if(i == 0)
                        {
                            int availableMinutes = dp.Start.Minute - 1;
                            if (availableMinutes > 0)
                            {
                                var start = DateTime.Today.AddHours(dp.Start.Hour);
                                lastInsertedStop = start.AddMinutes(availableMinutes);
                                dataPoints.Add(new WalkingDataPointModel(start, lastInsertedStop, Config.BriskPaceMetersPerSecond));
                                minutesRemaining -= availableMinutes;
                            }
                        }
                        if (minutesRemaining > 0)
                        {
                            //after current dp
                            int minutesBetween = (int)Math.Floor((nextDp.Start - dp.Stop).TotalMinutes);
                            if(minutesBetween > 0)
                            {
                                lastInsertedStop = nextDp.Start;
                                dataPoints.Add(new WalkingDataPointModel(dp.Stop, lastInsertedStop, Config.BriskPaceMetersPerSecond));
                                minutesRemaining -= minutesBetween;
                            }
                        }
                    }
                }
                await storageService.StoreWalkingDataPoints(dataPoints);
            });

            LiveLogService.Current.LogUpdated += newLog => Log = newLog;
            Log = LiveLogService.Current.GetLog();
        }

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            Title = "Testing",
        };

        public override SettingType Type { get; } = SettingType.Testing;
    }
}
