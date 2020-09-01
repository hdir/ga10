using SQLite;
using System;

namespace Bare10.Models
{
    public class AchievementModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public bool HasBeenAchieved { get; set; }
        public DateTime TimeAchieved { get; set; }

        public AchievementModel()
        {

        }
    }
}
