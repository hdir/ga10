using System;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace Bare10.Pages.Custom.Base
{
    public class TemplateView : ContentView
    {
        public static readonly BindableProperty TemplateProperty =
            BindableProperty.Create(
                nameof(Template),
                typeof(DataTemplate),
                typeof(TemplateView),
                default(DataTemplate)
            );

        public DataTemplate Template
        {
            get => (DataTemplate) GetValue(TemplateProperty);
            set => SetValue(TemplateProperty, value);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            SetupView();
        }

        private void SetupView()
        {
            var template = (Template != null && Template is DataTemplateSelector selector)
                ? selector.SelectTemplate(BindingContext, null)
                : Template;

            try
            {
                var templateInstance = template.CreateContent();

                var templateView = templateInstance as View;
                var templateCell = templateInstance as ViewCell;

                if (templateView == null && templateCell == null)
                    throw new InvalidOperationException("DataTemplate must be either a Cell or a View");

                var view = templateView ?? templateCell.View;

                if (view != null)
                {
                    view.BindingContext = BindingContext;
                    Content = view;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
    }
}