using Bare10.Content;
using Bare10.Resources;
using MvvmCross.ViewModels;

namespace Bare10.ViewModels.Items
{
    public class ProgressViewModel : MvxViewModel
    {
        private float _minutesBriskWalked;
        public float MinutesBriskWalked
        {
            get => _minutesBriskWalked;
            set => SetProperty(ref _minutesBriskWalked, value);
        }

        public void UpdateWithBriskWalkMinutes(float minutes)
        {
            if (GoalCompleted && minutes < Settings.CurrentGoal.Details().RequiredMinutes)
                GoalCompleted = false;

            MinutesBriskWalked = minutes;
        }

        private bool _goalCompleted;
        public bool GoalCompleted
        {
            get => _goalCompleted;
            set => SetProperty(ref _goalCompleted, value);
        }

        private bool _init;
        public float InitialValue
        {
            get
            {
                if (!_init)
                {
                    _init = true;
                    return 0;
                }
                return MinutesBriskWalked;
            }
        }
    }
}
