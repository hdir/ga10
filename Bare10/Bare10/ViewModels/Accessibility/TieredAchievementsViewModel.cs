using Bare10.Content;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.ViewModels.Accessibility.ViewCellModels;
using Bare10.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Bare10.ViewModels.Accessibility
{
    public class TieredAchievementsViewModel : ViewModelBase
    {
        private readonly IAchievementService achievementService;

        #region Properties
        public ObservableCollection<TieredAchievementViewModel> Achievements { get; } = new ObservableCollection<TieredAchievementViewModel>();
        #endregion

        public TieredAchievementsViewModel()
        {
            achievementService = AchievementService.Current;

            foreach (var tieredAchievement in TieredAchievementExtensions.GetAll())
            {
                var details = tieredAchievement.Details();
                var achievement = new TieredAchievementViewModel(tieredAchievement, details.Title, details.Description);

                Achievements.Add(achievement);
            }

            //Match up models with viewModels
            foreach(var a in achievementService.GetTieredAchievements())
            {
                var achievement = Achievements.First(vma => vma.Achievement == (TieredAchievement)a.Id);
                achievement.UpdateFromStorageModel(a);
            }

            achievementService.OnTierChanged += HandleTierChanged;
            achievementService.OnTierProgress += HandleTierProgress;

            CrossServiceContainer.UpdateService.UpdateModeChanged += mode =>
            {
                if (mode == UpdateMode.Background)
                {
                    achievementService.OnTierChanged -= HandleTierChanged;
                    achievementService.OnTierProgress -= HandleTierProgress;
                }
                else
                {
                    achievementService.OnTierChanged += HandleTierChanged;
                    achievementService.OnTierProgress += HandleTierProgress;
                }
            };
        }

        private void HandleTierChanged(TieredAchievement achievement, Tier tierAchieved)
        {
            var vma = Achievements.First(a => a.Achievement == achievement);
            var sma = achievementService.GetTieredAchievements().Find(a => (TieredAchievement)a.Id == achievement);

            vma.UpdateFromStorageModel(sma);

        }

        private void HandleTierProgress(TieredAchievement achievement)
        {
            var vma = Achievements.First(a => a.Achievement == achievement);
            var sma = achievementService.GetTieredAchievements().Find(a => (TieredAchievement)a.Id == achievement);

            vma.UpdateFromStorageModel(sma);
        }
    }
}
