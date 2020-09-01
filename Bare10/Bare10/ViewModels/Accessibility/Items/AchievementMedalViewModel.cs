using Bare10.Content;
using Bare10.Models;
using Bare10.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bare10.ViewModels.Accessibility.Items
{
    public class AchievementMedalViewModel : ViewModelBase
    {
        private readonly TieredAchievement achievement;
        private readonly int progressGoal;

        #region Properties

        public Tier Tier { get; }

        private string text;
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
        #endregion

        public AchievementMedalViewModel(TieredAchievement achievement, Tier tier)
        {
            this.achievement = achievement;
            Tier = tier;
            progressGoal = achievement.GetGoal(tier);

            //NOTE: Just to have some defaults
            CompileText(false, Tier != Tier.Bronze, DateTime.MinValue, 0, 0);
        }

        public void UpdateFromStorageModel(AchievementTierProgressModel progress, bool isLocked)
        {
            // NOTE: Make sure that when entering a new tier the subprogress is cleared (as subprogress would be full otherwise)
            int subProgress = (progress.Progress == 0 && progress.SubProgress == achievement.GetSubGoal())
                ? 0 : progress.SubProgress;

            CompileText(
                progress.Progress >= progressGoal,
                isLocked,
                progress.DateTierAchieved,
                progress.Progress,
                subProgress);


            RaisePropertyChanged(() => Text);
        }

        private void CompileText(bool isCompleted, bool isLocked, DateTime dateCompleted, int progress, int subProgress)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Tier.ToTierName());

            if(isCompleted)
            {
                sb.Append($"Mottatt: {dateCompleted:dd.MM.yyyy}");
            }
            else if(!isLocked)
            {
                switch(achievement)
                {
                    case TieredAchievement.ThirtyMinutes:
                        sb.AppendLine($"Du har gått {subProgress} av {achievement.GetSubGoal()} minutter i dag");
                        break;
                    case TieredAchievement.ReachedGoalFiveDaysInARow:
                        sb.AppendLine($"Du har gått {subProgress} av {achievement.GetSubGoal()} dager i strekk");
                        break;
                    default:
                        break;
                }

                sb.Append($"Du har klart oppgaven {progress} av {progressGoal} ganger så langt");
            }
            else
            {
                sb.Append("Ikke tilgjengelig før du har klart tidligere medaljer");
            }

            Text = sb.ToString();
        }
    }
}
