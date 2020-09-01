using Xamarin.Forms;

namespace Bare10.Resources
{
    public static class Colors
    {
        #region Fra Victoria
        private static readonly Color Red = Color.FromHex("FF5737");
        private static readonly Color DarkRed = Color.FromHex("833A2C");
        private static readonly Color White = Color.FromHex("FFFFFF");
        private static readonly Color BackgroundDark = Color.FromHex("081E21");
        private static readonly Color BackgroundDarkAlt = Color.FromHex("0B272B");
        private static readonly Color BackgroundMedium = Color.FromHex("0D2F34");
        private static readonly Color NormalWalkBlue = Color.FromHex("C1F3F7");
        private static readonly Color TextLightGray = Color.FromHex("D1D1D1");
        private static readonly Color TextBlack = Color.FromHex("081E21");
        private static readonly Color WheelBackgroundBlue = Color.FromHex("103C42");
        private static readonly Color LinesBlue = Color.FromHex("394748");
        private static readonly Color OrganicColor = Color.FromHex("152D30");
        #endregion

        #region General

        public static Color Background { get; } = BackgroundDark;

        public static Color Border { get; } = LinesBlue;
        public static Color LightBorderColor { get; } = TextLightGray;

        public static Color Text { get; } = White;
        public static Color TextLinkColor { get; } = White;
        public static Color TextFaded { get; } = TextLightGray.MultiplyAlpha(0.8);
        public static Color TextSpecial { get; } = Red;
        public static Color TextInverted { get; } = TextBlack;

        #endregion

        #region Onboarding
        public static Color OnboardingButton { get; } = Red;
        public static Color OnboardingButtonInactive { get; } = WheelBackgroundBlue;
        public static Color OnboardingOrganicColor { get; } = OrganicColor;
        #endregion

        #region Tab menu
        public static Color TabPanelForeground { get; } = White;
        public static Color TabPanelBackground { get; } = BackgroundDarkAlt;
        #endregion

        #region History View
        public static Color TextBriskWalk { get; } = Red;
        public static Color TextNormalWalk { get; } = NormalWalkBlue;
        public static Color TextSpecialDark { get; } = Red.MultiplyAlpha(0.4f);
        #endregion

        #region Info
        public static Color InfoMessageBackground { get; } = BackgroundMedium.MultiplyAlpha(0.6f);
        public static Color InfoLoadingMessageBackground { get; } = BackgroundMedium.MultiplyAlpha(0.6f);
        public static Color InfoWarningBackground { get; } = BackgroundMedium.MultiplyAlpha(0.6f);
        public static Color InfoErrorMessageBackground { get; } = Red.MultiplyAlpha(0.5f);
        #endregion

        #region Achievements
        public static Color AchievementDescriptionText { get; } = TextFaded;
        public static Color AchievementDescriptionTitle { get; } = White;
        public static Color AchievementHealthBackground { get; } = White;
        public static Color AchievementHealthText { get; } = TextBlack;
        public static Color AchievementOutline { get; } = LinesBlue;
        public static Color AchievementDescriptionBackground{ get; } = BackgroundDarkAlt;


        public static Color Bronze { get; } = Color.FromHex("#AF875E");
        public static Color Silver { get; } = Color.FromHex("#C7C7C7");
        public static Color Gold { get; } = Color.FromHex("#F7CC54");

        #endregion

        #region Goals
        public static Color GoalBackgroundInactive { get; } = BackgroundMedium;
        public static Color GoalBackgroundActive { get; } = Red;

        public static Color GoalIconActive { get; } = White;
        public static Color GoalIconInactive { get; } = LinesBlue;

        public static Color GoalTitleActive { get; } = White;
        public static Color GoalTitleInactive { get; } = GoalBackgroundActive;

        public static Color GoalTextActive { get; } = TextBlack;
        public static Color GoalTextInactive { get; } = White;
        #endregion

        #region Popup
        public static Color InfoPopupText { get; } = TextBlack;
        public static Color InfoPopupBackground { get; } = White;
        #endregion

        #region Graphs
        public static Color GraphBrisk { get; } = Red;
        public static Color GraphNormal { get; } = NormalWalkBlue;
        public static Color GraphWhiteLine { get; } = White;
        public static Color GraphFadedLine { get; } = White.MultiplyAlpha(0.4f);
        #endregion

        #region Bulletin
        public static Color BulletinPopupBackground { get; } = Color.Black.MultiplyAlpha(0.2f);
        #endregion

        #region ProgressWheel

        public static Color ProgressWheelBackground { get; } = WheelBackgroundBlue;
        public static Color ProgressWheelForeground { get; } = Red;

        #endregion

        #region BarChart

        public static Color BarChartBackground { get; } = Red.MultiplyAlpha(0.4f);
        public static Color BarChartForeground { get; } = Red;

        #endregion
    }
}
