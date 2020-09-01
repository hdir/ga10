using System;
using Bare10.Content;
using Bare10.Localization;
using Bare10.Resources;
using Bare10.ViewModels.Base;
using MvvmCross.Commands;

namespace Bare10.ViewModels.ViewCellsModels
{
    public class GoalViewModel : ViewModelBase
    {
        public event Action<Goal> IsChosen;

        public IMvxCommand ChooseCommand { get; }

        public Goal Goal { get; }
        public string Title { get; }
        public string Description { get; }
        public uint MinutesRequiredToAchieve { get; }

        public string AccessibleText => Title + ".\n" + Description;

        public string AccessibleSelectButtonText => AppText.goals_page_confirm_goal + ": " + Title;

        public bool IsCurrentGoal => Goal == Settings.CurrentGoal;
        public bool CanChangeGoal => !IsCurrentGoal && IsSelected;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
                RaisePropertyChanged(() => IsCurrentGoal);
                RaisePropertyChanged(() => CanChangeGoal);
            }
        }

        public GoalViewModel(Goal goal)
        {
            var details = goal.Details();

            Goal = goal;
            Title = details.Title;
            Description = details.Description;
            MinutesRequiredToAchieve = details.RequiredMinutes;

            ChooseCommand = new MvxCommand(() => IsChosen?.Invoke(Goal));
        }
    }
}
