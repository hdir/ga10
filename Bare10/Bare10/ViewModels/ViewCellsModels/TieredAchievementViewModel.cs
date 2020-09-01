using Bare10.Models;
using Bare10.Services;
using Bare10.ViewModels.Base;
using Bare10.ViewModels.Items;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Bare10.Content;
using Xamarin.Forms;

namespace Bare10.ViewModels.ViewCellsModels
{
    public class TieredAchievementViewModel : ViewModelBase
    {
        private readonly TieredAchievementModel _model;

        #region Properties

        public string Title { get; set; }

        public TieredAchievement Achievement { get; }

        public ObservableCollection<AchievementTierProgressViewModel> Tiers { get; }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                foreach (var t in Tiers)
                    t.Description = value + (t.ProgressGoal > 1 ? $" {t.ProgressGoal} ganger" : "");
            }
        }

        public string CompletedDescription
        {
            set
            {
                foreach (var t in Tiers)
                    t.CompletedDescription = value + (t.ProgressGoal > 1 ? $" {t.ProgressGoal} ganger" : "");
            }
        }

        #endregion

        #region Commands

        private ICommand _tierSelectedCommand;
        public ICommand TierSelectedCommand
        {
            get => _tierSelectedCommand;
            set => SetProperty(ref _tierSelectedCommand, value);
        }

        #endregion

        public TieredAchievementViewModel(TieredAchievement achievement)
        {
            Achievement = achievement;

            _model = new TieredAchievementModel(Achievement);

            Tiers = new ObservableCollection<AchievementTierProgressViewModel>(TierExtensions.GetAll().Select(tier =>
                new AchievementTierProgressViewModel(Achievement, tier)));
        }

        public void UpdateFromStorageModel(TieredAchievementModel model)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _model.CurrentTier = model.CurrentTier;

                //Add in previous and current tier
                foreach (var tier in TierExtensions.GetAll())
                {
                    var isLocked = (int) tier > (int) model.CurrentTier;
                    var progress = AchievementService.Current.GetProgressForTier(_model.Id, tier);
                    Tiers.FirstOrDefault(t => t.Tier == tier)?.UpdateFromStorageModel(progress, isLocked);
                }
            });
        }
    }
}
