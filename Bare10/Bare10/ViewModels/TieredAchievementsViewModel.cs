using Bare10.Localization;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.ViewModels.Base;
using Bare10.ViewModels.ViewCellsModels;
using MvvmCross.Commands;
using System.Collections.ObjectModel;
using System.Linq;
using Bare10.Content;
using Bare10.ViewModels.Items;
using Xamarin.Forms;

namespace Bare10.ViewModels
{
    public class TieredAchievementsViewModel : ToolbarViewModelBase
    {
        private readonly IAchievementService achievementService;

        #region Commands
        public IMvxCommand TappedAchievement { get; private set; }
        #endregion

        #region Properties

        public ObservableCollection<TieredAchievementViewModel> Achievements { get; } = 
            new ObservableCollection<TieredAchievementViewModel>();

        public override ToolbarState ToolbarState { get; } = new ToolbarState()
        {
            Title = AppText.navigation_achievements_title,
        };

        private AchievementTierProgressViewModel _selected;
        public AchievementTierProgressViewModel Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        private bool _showAchievementDetails;
        public bool ShowAchievementDetails
        {
            get => _showAchievementDetails;
            set => SetProperty(ref _showAchievementDetails, value);
        }

        #endregion

        public TieredAchievementsViewModel()
        {
            achievementService = AchievementService.Current;

            foreach (var tieredAchievement in TieredAchievementExtensions.GetAll())
            {
                var details = tieredAchievement.Details();
                var achievement = new TieredAchievementViewModel(tieredAchievement)
                {
                    Title = details.Title,
                    Description = details.Description,
                    CompletedDescription = details.CompletedDescription,
                };
                achievement.TierSelectedCommand = new Command<AchievementTierProgressViewModel>(tier =>
                {
                    Toolbar.Instance.Push(new ToolbarState()
                    {
                        Title = achievement.Title,
                    }, () =>
                    {
                        ShowAchievementDetails = false;
                    });
                    Selected = tier;
                    ShowAchievementDetails = true;
                });
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

        public override void ViewAppearing()
        {
            var todaysWalking = CrossServiceContainer.WalkingDataService.GetTodaysHistory();
            achievementService.CheckTieredAchievementsProgress(todaysWalking);

            //Match up models with viewModels
            foreach (var a in achievementService.GetTieredAchievements())
            {
                var achievement = Achievements.First(vma => vma.Achievement == (TieredAchievement)a.Id);
                achievement.UpdateFromStorageModel(a);
            }

            RaisePropertyChanged(nameof(Achievements));
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
