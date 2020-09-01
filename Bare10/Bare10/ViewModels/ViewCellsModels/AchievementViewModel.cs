using Bare10.Models;
using Bare10.Resources;
using Bare10.ViewModels.Base;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace Bare10.ViewModels.ViewCellsModels
{
    public class AchievementViewModel : ViewModelBase
    {
        #region Properties
        public Content.Achievement Achievement { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string UnlockedDescription { get; set; }
        public string HealthDescription { get; set; }
        public string ShareSocialMedia { get; set; }

        public AchievementModel StorageModel { get; private set; }

        public string TimeAchieved => StorageModel.TimeAchieved.ToString("dd.MM.yyyy");

        public bool HasBeenAchieved
        {
            get => StorageModel.HasBeenAchieved;
            set
            {
                StorageModel.HasBeenAchieved = value;
                RaisePropertyChanged(() => HasBeenAchieved);
                RaisePropertyChanged(() => AccessibilityText);
            }
        }

        public string IconCompleteSource { get; }
        public string IconIncompleteSource { get; }

        public ImageSource IconComplete { get; }
        public ImageSource IconIncomplete { get; }

        public string AccessibilityText
        {
            get
            {
                string achieved = HasBeenAchieved ? "" : "ikke ";
                string unlockedtext = HasBeenAchieved ? $"{UnlockedDescription}\n{HealthDescription}" : "";
                return $"{Title}\n {Description}\n Har {achieved}blitt fullført\n{unlockedtext}";
            }

        }

        #endregion

        public AchievementViewModel(Content.Achievement achievement)
        {
            Achievement = achievement;
            IconCompleteSource = Images.AchievementIcons[achievement];
            IconIncompleteSource = Images.AchievementIncompleteIcons[achievement];

            IconComplete = SvgImageSource.FromResource(IconCompleteSource);
            IconIncomplete = SvgImageSource.FromResource(IconIncompleteSource);

            StorageModel = new AchievementModel
            {
                Id = (int)Achievement,
            };
        }

        public void UpdateFromStorageModel(AchievementModel model)
        {
            HasBeenAchieved = model.HasBeenAchieved;
            //This is formatted with a string prefix, so manual property change notification
            StorageModel.TimeAchieved = model.TimeAchieved;
            RaisePropertyChanged(() => TimeAchieved);
            RaisePropertyChanged(() => AccessibilityText);
        }
    }
}
