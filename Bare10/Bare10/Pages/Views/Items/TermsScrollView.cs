using System.Globalization;
using Bare10.Localization;
using Bare10.Resources;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Items
{
    public class TermsScrollView : WallOfTextBaseView
    {
        public TermsScrollView()
        {
            Content = new StackLayout()
            {
                Children =
                {
                    Section(SectionContent("1.", AppText.terms_1_0_title,  AppText.terms_1_0_text)),
                    Section(SectionContent("2.", AppText.terms_2_0_title,  AppText.terms_2_0_text)),
                    Section(SectionContent("3.", AppText.terms_3_0_title,  AppText.terms_3_0_text), false),
                    Section(SectionContent("3.1", AppText.terms_3_1_title,  AppText.terms_3_1_text), false),
                    Section(SectionContent("3.2", AppText.terms_3_2_title,  AppText.terms_3_2_text), false),
                    Section(SectionContent("3.3", AppText.terms_3_3_title,  AppText.terms_3_3_text), false),
                    //Section(SectionContent("3.3", AppText.terms_3_3_title,  AppText.terms_3_3_text)),
                    //Paragraph(AppText.terms_final_text, InternalPadding),
                }
            };
        }

        private static View SectionContent(string id, string titleText, string contentText)
        {
            var layout = new StackLayout() { Spacing = 10 };

            // Add Section Number
            layout.Children.Add(SectionNumber(id));

            // Add Title
            layout.Children.Add(TitleView(titleText));

            // Add Paragraphs
            foreach (var paragraph in MakeParagraphs(contentText))
                layout.Children.Add(paragraph);

            return layout;
        }

        private static View SectionNumber(string id) => new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Text = id,
                FontSize = Sizes.TextSmall,
            };

        private static View SectionNumber(double id) => SectionNumber(id.ToString("0.0", CultureInfo.InvariantCulture));
    }
}
