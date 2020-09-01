using Bare10.Models.Walking;
using Bare10.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Bare10.Utils;
using Bare10.Localization;

namespace Bare10.ViewModels.Accessibility
{
    public class HistoryViewModel : ViewModelBase
    {
        #region Properties
        public ExplicitObservableCollection<string> HourlyBreakdown { get; } = new ExplicitObservableCollection<string>();
        public ExplicitObservableCollection<string> WeeklyBreakDown { get; } = new ExplicitObservableCollection<string>();
        public ExplicitObservableCollection<string> MonthlyBreakdown { get; } = new ExplicitObservableCollection<string>();
        #endregion

        public HistoryViewModel()
        {
            var walkingDataService = CrossServiceContainer.WalkingDataService;

            ////NOTE: Manually poll since HomeViewModel does the first poll
            UpdateDay(walkingDataService.GetTodaysHistory());
            UpdateWeek(walkingDataService.GetWeekHistory());
            UpdateMonth(walkingDataService.GetThirtyDaysHistory());

            walkingDataService.TodaysWalkingUpdated += UpdateDay;
            walkingDataService.ThisWeekDaysUpdated += UpdateWeek;
            walkingDataService.LastThirtyDaysUpdated += UpdateMonth;

        }

        private void UpdateDay(TodaysWalkingModel today)
        {
            HourlyBreakdown.Clear();
            foreach(var dp in today.todaysWalking)
            {
                HourlyBreakdown.Add(string.Format(
                    "Fra {0} til {1}: {2}.\n",
                    dp.Start.ToShortTimeString(),
                    dp.Stop.ToShortTimeString(),
                    dp.WasBrisk ? AppText.brisk_walk : AppText.regular_walk));
            }
            RaisePropertyChanged(() => HourlyBreakdown);
        }

        private void UpdateWeek(List<WalkingDayModel> walking)
        {
            var week = new WalkingDayModel[7];
            foreach (var walk in walking)
                week[((int)walk.Day.DayOfWeek + 6) % 7] = walk;

            WeeklyBreakDown.Clear();
            for(int i = 0; i < week.Length; ++i)
            {
                string entry = (week[i] != null) 
                    ?
                    string.Format("{0}: {1} {2} frisk gange.\n", week[i].Day.DayOfWeek.InNorwegian(), week[i].MinutesBriskWalking, week[i].MinutesBriskWalking == 1 ? "minutt" : "minutter")
                    :
                    string.Format("{0}: 0 minutter frisk gange.\n", ((DayOfWeek)((i + 1) % 7)).InNorwegian());
                WeeklyBreakDown.Add(entry);
            }
            RaisePropertyChanged(() => WeeklyBreakDown);
        }

        private void UpdateMonth(List<WalkingDayModel> walking)
        {
            var month = from d in walking.GetRange(0, 30)
                        orderby d.Day descending
                        select d;

            MonthlyBreakdown.Clear();
            foreach(var d in month)
            {
                MonthlyBreakdown.Add(string.Format("{0}: {1} {2} frisk gange.\n", d.Day.ToString("m", App.Culture), d.MinutesBriskWalking, d.MinutesBriskWalking == 1 ? "minutt" : "minutter"));
            }
            RaisePropertyChanged(() => MonthlyBreakdown);
        }
    }
}
