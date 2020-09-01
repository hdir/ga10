using System;
using System.Collections.Generic;
using System.Linq;
using Bare10.Resources;
using Bare10.Utils;
using Bare10.Utils.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Items
{
    public abstract class WallOfTextBaseView : ScrollView
    {
        protected const int InternalPadding = 30;

        protected static View Section(View content, bool hasSeparator = true)
        {
            var section = new StackLayout()
            {
                Padding = new Thickness(InternalPadding, InternalPadding, InternalPadding, hasSeparator ? InternalPadding : 0),
                Children =
                {
                    content,
                }
            };

            var line = DefaultView.SectionLine;
            line.IsVisible = hasSeparator;

            return new Grid()
            {
                VerticalOptions = LayoutOptions.StartAndExpand,
                Children =
                {
                    section,
                    line,
                }
            };
        }

        protected static View TitleView(string titleText) => 
            new Label()
            {
                FontSize = Sizes.TextLarge,
                TextColor = Colors.TextSpecial,
                Text = titleText,
                TextDecorations = TextDecorations.Underline,
                FontAttributes = FontAttributes.Bold,
            };

        protected static View Paragraph(string text, int pad = 0) => 
            new Label()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Margin = pad,
                FormattedText = TextUtils.ParseHtmlAnchors(text, Colors.TextLinkColor),
                FontSize = Sizes.TextSmall,
                TextColor = Colors.TextFaded,
            };

        protected static IEnumerable<View> MakeParagraphs(string rawText)
        {
            var paragraphs = rawText.Split(new[] { "\r\n", Environment.NewLine }, StringSplitOptions.None);
            return (from paragraph in paragraphs where !string.IsNullOrEmpty(paragraph) select Paragraph(paragraph)).ToList();
        }
    }
}