using Bare10.Content;
using Bare10.Services;
using Bare10.Services.Interfaces;
using Bare10.Utils;
using Bare10.ViewModels.Base;
using Bare10.ViewModels.ViewCellsModels;

namespace Bare10.ViewModels.Accessibility
{
    public class AchievementsViewModel : ViewModelBase
    {
        private readonly IAchievementService achievementService;

        #region Properties
        public ExplicitObservableCollection<AchievementViewModel> Achievements { get; } = 
            new ExplicitObservableCollection<AchievementViewModel>(AchievementExtensions.Achievements);
        #endregion

        public AchievementsViewModel()
        {
            achievementService = AchievementService.Current;

            //foreach(var a in achievementService.GetAchievements())
            //{
            //    var achievement = Achievements.First(vma => vma.Achievement == (Achievement)a.Id);
            //    achievement.UpdateFromStorageModel(a);
            //}
            //achievementService.OnAchievementUnlocked += HandleAchievementUnlocked;
        }

        private void HandleAchievementUnlocked(Achievement achievement)
        {
            //var vma = Achievements.First(a => a.Achievement == achievement);

            ////Match up storage model with viewModel
            //var sma = achievementService.GetAchievements().Find(a => (Achievement)a.Id == achievement);
            //vma.UpdateFromStorageModel(sma);

            //Achievements.RaisePropertyChanged();
        }
    }
}
