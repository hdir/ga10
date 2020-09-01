using System;
using System.Linq;
using Bare10.Localization;
using Bare10.Resources;
using Xamarin.Forms;

namespace Bare10.Content
{
    public enum Tier
    {
        Bronze = 0,
        Silver,
        Gold,

        //NOTE: Always last - ensures a valid tier after completing all tiers
        Complete,
    }

    public static class TierExtensions
    {
        public static Tier[] GetAll()
        {
            //return Enum.GetValues(typeof(Tier)).Cast<Tier>().ToArray();
            return new[]
            {
                Tier.Bronze,
                Tier.Silver,
                Tier.Gold,
            };
        }

        public static string ToTierName(this Tier tier)
        {
            switch (tier)
            {
                case Tier.Bronze:
                    return AppText.achievement_tier_bronze.ToUpper();
                case Tier.Silver:
                    return AppText.achievement_tier_silver.ToUpper();
                case Tier.Gold:
                    return AppText.achievement_tier_gold.ToUpper();
                default:
                    return tier.ToString();
            }
        }

        public static Color ToTierColor(this Tier tier)
        {
            switch (tier)
            {
                case Tier.Bronze:
                    return Colors.Bronze;
                case Tier.Silver:
                    return Colors.Silver;
                case Tier.Gold:
                    return Colors.Gold;
                case Tier.Complete:
                    return Color.Cyan;
                default:
#if DEBUG
                    throw new ArgumentOutOfRangeException(nameof(tier), tier, null);
#else
                    return Color.Magenta;
#endif
            }
        }
    }
}
