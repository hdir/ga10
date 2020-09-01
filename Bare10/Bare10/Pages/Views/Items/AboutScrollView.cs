using Bare10.Localization;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Items
{
    public class AboutScrollView : WallOfTextBaseView
    {
        public AboutScrollView()
        {
            Content = new StackLayout()
            {
                Padding = new Thickness(0, 0, 0, InternalPadding),
                Children =
                {
                    Section(SectionContent(AppText.about_1_title, AppText.about_1_text)),
                    Section(SectionContent(AppText.about_2_title, AppText.about_2_text)),
                    Section(SectionContent(AppText.about_3_title, Device.RuntimePlatform == Device.Android 
                            ? AppText.about_3_text_android 
                            : AppText.about_3_text_ios)),
                    Section(SectionContent(AppText.about_4_title, AppText.about_4_text, false)),
                }
            };
        }

        private static View SectionContent(string titleText, string contentText, bool hasSeparator = true)
        {
            var layout = new StackLayout(){ Spacing = 10 };

            // add title
            layout.Children.Add(TitleView(titleText));

            // add paragraphs
            foreach (var paragraph in MakeParagraphs(contentText))
                layout.Children.Add(paragraph);

            return layout;
        }
    }
}
