using System.Collections.Generic;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;

namespace Bare10.Utils
{
    public static  class SvgUtils
    {
        public static string ToHex(this Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            return $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";
        }

        /// <summary>
        /// Changes color in svg by replacing strings.
        /// </summary>
        public static SvgCachedImage ReplaceColor(this SvgCachedImage svg, Color color, string replaceTitle = "fill", string replaceText = "\"#000000\"")
        {
            svg.ReplaceStringMap = new Dictionary<string, string>
            {
                {
                    $"{replaceTitle}={replaceText}",
                    $"{replaceTitle}=\"{color.ToHex()}\""
                }
            };
            return svg;
        }

        public static Dictionary<string, string> ReplaceStringMap(Color color, string replaceTitle = "fill", string replaceText = "\"#000000\"")
        {
            return new Dictionary<string, string>
            {
                {
                    $"{replaceTitle}={replaceText}",
                    $"{replaceTitle}=\"{color.ToHex()}\""
                }
            };
        }
    }
}
