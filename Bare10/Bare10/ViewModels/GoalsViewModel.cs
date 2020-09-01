using Bare10.Resources;
using Bare10.ViewModels.Base;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Bare10.Content;
using Bare10.Localization;
using Bare10.ViewModels.ViewCellsModels;
using MvvmCross.Commands;

namespace Bare10.ViewModels
{
    public class GoalsViewModel : ToolbarViewModelBase
    {
        #region Commands
        public IMvxCommand ItemTappedCommand { get; }
        #endregion

        #region Properties
        public ObservableCollection<GoalViewModel> Goals { get; } 
            = new ObservableCollection<GoalViewModel>(GoalExtensions.GetAll().Select(goal => new GoalViewModel(goal)));

        private GoalViewModel _selectedGoal;
        public GoalViewModel SelectedGoal
        {
            get => _selectedGoal;
            set
            {
                // unselect previous
                if (_selectedGoal != null)
                    _selectedGoal.IsSelected = false;

                // raise property changed
                SetProperty(ref _selectedGoal, value);
            }
        }

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            Title = AppText.navigation_goal_title,
        };

        #endregion

        public GoalsViewModel()
        {
            foreach (var goalViewModel in Goals)
                goalViewModel.IsChosen += SetNewGoal;

            SelectedGoal = Goals.First(g => g.Goal == Settings.CurrentGoal);

            ItemTappedCommand = new MvxCommand(() =>
            {
                if (SelectedGoal != null)
                {
                    SelectedGoal.IsSelected = !SelectedGoal.IsSelected;
                }
            });
        }

        public void SetNewGoal(Goal goal)
        {
            Settings.CurrentGoal = goal;

            foreach (var g in Goals)
                g.IsSelected = g.Goal == Settings.CurrentGoal;

            Analytics.TrackEvent(TrackingEvents.GoalChanged, new TrackingEvents.GoalChangedArgs(Enum.GetName(typeof(Goal), SelectedGoal.Goal)));

            //NOTE: Special case as this is an achievement
            //if (SelectedGoal.Goal != Content.Goal.TenMinutes)
            //{
            //    Task.Run(AchievementService.Current.UserChangedGoal);
            //}
        }
    }
}
