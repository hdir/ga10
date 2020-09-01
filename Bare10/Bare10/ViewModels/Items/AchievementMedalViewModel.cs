using Bare10.Models;
using Bare10.Resources;
using Bare10.ViewModels.Base;
using FFImageLoading.Svg.Forms;
using System;
using Bare10.Content;
using Xamarin.Forms;

namespace Bare10.ViewModels.Items
{
    public class AchievementTierProgressViewModel : ViewModelBase
    {
        #region Properties

        public TieredAchievement Achievement { get; }
        public Tier Tier { get; }
        public string Title { get; }
        public string Description { get; set; }
        public string CompletedDescription { get; set; }
        public string UnlockedText { get; }
        public int ProgressGoal { get; }

        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                SetProperty(ref _progress, value);
                RaisePropertyChanged(() => ProgressNormalized);
                RaisePropertyChanged(() => IsCompleted);
            }
        }

        private bool _isLocked;
        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        public double ProgressNormalized => Progress / (double)ProgressGoal;

        public bool IsCompleted => Progress >= ProgressGoal;

        private ImageSource IconComplete { get; }
        private ImageSource IconIncomplete { get; }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get => _icon;
            private set => SetProperty(ref _icon, value);
        }

        private int _subProgress;
        public int SubProgress
        {
            get => _subProgress;
            private set => SetProperty(ref _subProgress, value);
        }

        private DateTime _dateCompleted;
        public DateTime DateCompleted
        {
            get => _dateCompleted;
            private set
            {
                SetProperty(ref _dateCompleted, value);
                RaisePropertyChanged(() => DateCompletedString);
            }
        }

        public string DateCompletedString => $"MOTATT {DateCompleted:dd.MM.yyyy}";

        #endregion


        public AchievementTierProgressViewModel(TieredAchievement achievement, Tier tier)
        {
            Achievement = achievement;
            Tier = tier;
            Title = tier.ToTierName();
            ProgressGoal = achievement.GetProgressiveGoal(tier);
            UnlockedText = achievement.GetUnlockedText(tier);

            IconComplete = SvgImageSource.FromResource(Images.TieredAchievementCompleteIcon(achievement));
            IconIncomplete = SvgImageSource.FromResource(Images.TieredAchievementIncompleteIcon(achievement));
        }

        public void UpdateFromStorageModel(AchievementTierProgressModel progress, bool isLocked)
        {
            if (!isLocked)
            {
                Progress = Achievement.GetProgressiveGoal(Tier - 1) + progress.Progress;
                // NOTE: Make sure that when entering a new tier the subprogress is cleared (as subprogress would be full otherwise)
                SubProgress = (progress.Progress == 0 && progress.SubProgress == Achievement.GetSubGoal())
                    ? 0 : progress.SubProgress;
            }

            DateCompleted = progress.DateTierAchieved;
            IsLocked = isLocked;

            Icon = IsCompleted ? IconComplete : IconIncomplete;
        }
    }
}
