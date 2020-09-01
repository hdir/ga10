using Xamarin.Forms;

namespace Bare10.Resources
{
    public class Fonts
    {
        public static string Normal { get; } = RobotoLight;
        public static string Bold { get; } = RobotoBold;

        private static string RobotoLight
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "Roboto-Light.ttf#Roboto-Light";
                    case Device.iOS:
                        return "Roboto-Light";
                    default:
                        return "";
                }
            }
        }

        private static string RobotoBold
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "Roboto-Medium.ttf#Roboto-Medium";
                    case Device.iOS:
                        return "Roboto-Medium";
                    default:
                        return "";
                }
            }
        }
    }
}
