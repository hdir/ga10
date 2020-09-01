using Acr.UserDialogs;
using Bare10.Localization;
using Bare10.Resources;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.ViewModels.Base;
using Bare10.ViewModels.ViewCellsModels;
using Microsoft.AppCenter.Analytics;
using MvvmCross.Commands;
using System;
using System.Linq;
using Bare10.Content;
using Bare10.Utils;

namespace Bare10.ViewModels.Accessibility
{
    public class GoalsViewModel : ViewModelBase
    {
        private readonly IGoalService goalService;

        #region Commands
        public IMvxAsyncCommand SetGoal { get; private set; }
        #endregion

        #region Properties
        private string currentGoal = "";
        public string CurrentGoal
        {
            get => currentGoal;
            set => SetProperty(ref currentGoal, value);
        }

        public ExplicitObservableCollection<GoalViewModel> Goals { get; } =
            new ExplicitObservableCollection<GoalViewModel>(GoalExtensions.GetAll()
                .Select(goal => new GoalViewModel(goal)));
        #endregion

        public GoalsViewModel()
        {
            goalService = GoalService.Current;
            
            foreach (var goalViewModel in Goals)
                goalViewModel.IsChosen += SetNewGoal;
        }

        public void SetNewGoal(Goal goal)
        {
            CurrentGoal = goal.Details().Title;

            Settings.CurrentGoal = goal;

            foreach (var g in Goals)
                g.IsSelected = g.Goal == Settings.CurrentGoal;

            Analytics.TrackEvent(TrackingEvents.GoalChanged, new TrackingEvents.GoalChangedArgs(Enum.GetName(typeof(Goal), goal)));

            string description = string.Format(AppText.accessibility_set_goal_description, CurrentGoal);
            UserDialogs.Instance.Alert(description, AppText.accessibility_set_goal_title);

            
            //NOTE: Special case as this is an achievement
            //if (goal != Content.Goal.TenMinutes)
            //{
            //    Task.Run(AchievementService.Current.UserChangedGoal);
            //}

        }
    }
}
