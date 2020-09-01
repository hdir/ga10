using SQLite;
using System;
using Bare10.Content;

namespace Bare10.Models
{
    public class AchievementTierProgressModel
    {
        /// <summary>
        /// Must <b>always</b> be created with CreatePrimaryHashKey
        /// </summary>
        [PrimaryKey]
        public int Id { get; set; }

        [Indexed] // reference to achievement in DB
        public int AchievementId { get; set; }
        public Tier Tier { get; set; }
        public int Progress { get; set; }
        //NOTE: Means something specific for different types of TieredAchievement
        [Ignore]
        public int SubProgress { get; set; }
        public DateTime TimeProgressAchieved { get; set; } = DateTime.MinValue;
        public DateTime DateTierAchieved { get; set; } = DateTime.MinValue;

        public AchievementTierProgressModel() { }
        public AchievementTierProgressModel(int achievementId, Tier tier)
        {
            //NOTE: Achievements and tier Ids should not exceed ‭65535‬
            Id = achievementId ^ ((int)tier << 15);
            Tier = tier;
            AchievementId = achievementId;
        }
    }
}
