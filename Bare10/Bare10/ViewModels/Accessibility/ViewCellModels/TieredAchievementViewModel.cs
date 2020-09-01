using Bare10.Content;
using Bare10.Models;
using Bare10.Services;
using Bare10.ViewModels.Accessibility.Items;
using Bare10.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Bare10.ViewModels.Accessibility.ViewCellModels
{
    public class TieredAchievementViewModel : ViewModelBase
    {
        private readonly TieredAchievementModel model;

        private string title;
        private string description;

        #region Properties

        public TieredAchievement Achievement { get; }
        public ObservableCollection<AchievementMedalViewModel> Tiers { get; }

        public string Text
        {
            get => $"{title}\n{description}";
        }

        #endregion

        public TieredAchievementViewModel(TieredAchievement achievement, string title, string description)
        {
            Achievement = achievement;
            this.title = title;
            this.description = description;
            model = new TieredAchievementModel();

            Tiers = new ObservableCollection<AchievementMedalViewModel>(
                TierExtensions.GetAll().Select(t => new AchievementMedalViewModel(achievement, t)));
            
        }
        public void UpdateFromStorageModel(TieredAchievementModel model)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.model.CurrentTier = model.CurrentTier;

                //Add in previous and current tier
                foreach (Tier tier in Enum.GetValues(typeof(Tier)))
                {
                    var isLocked = (int) tier > (int) model.CurrentTier;
                    var progress = AchievementService.Current.GetProgressForTier(model.Id, tier);
                    Tiers.FirstOrDefault(t => t.Tier == tier)?.UpdateFromStorageModel(progress, isLocked);
                }
            });
        }
    }
}
