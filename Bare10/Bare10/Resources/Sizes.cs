using Xamarin.Forms;

namespace Bare10.Resources
{
    public static class Sizes
    {
        #region Images
        public static double TabIconScale = 0.8;
        #endregion

        #region Frames
        public static float CornerRadius = 6;
        #endregion

        #region Text

        public static double TextMicro { get; } = IsAndroid ? 10 : Device.GetNamedSize(NamedSize.Micro, typeof(Label)) + 1;
        public static double TextSmall { get; } = IsAndroid ? 14 : Device.GetNamedSize(NamedSize.Small, typeof(Label)) + 1;
        public static double TextMedium { get; } = IsAndroid ? 16 : Device.GetNamedSize(NamedSize.Medium, typeof(Label)) + 1;
        public static double TextLarge { get; } = IsAndroid ? 20 : Device.GetNamedSize(NamedSize.Large, typeof(Label)) + 2;

        public static double Title { get; } = TextLarge + 2;

        public static double ProgressBarText { get; } = (IsAndroid ? 1.6 : 1.3 ) * TextLarge;

        #endregion

        private static bool IsAndroid => Device.RuntimePlatform == Device.Android;
    }
}
