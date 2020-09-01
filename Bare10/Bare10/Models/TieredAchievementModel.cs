using Bare10.Content;
using Newtonsoft.Json;
using SQLite;

namespace Bare10.Models
{
    public class TieredAchievementModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        /// <summary>
        /// The tier we are currently working _towards_.
        /// </summary>
        public Tier CurrentTier { get; set; }

        [JsonIgnore]
        public TieredAchievement Type => (TieredAchievement) Id;

        // Only for Deserialization
        public TieredAchievementModel()
        {
        }

        public TieredAchievementModel(TieredAchievement achievement)
        {
            Id = (int) achievement;
        }
    }
}
