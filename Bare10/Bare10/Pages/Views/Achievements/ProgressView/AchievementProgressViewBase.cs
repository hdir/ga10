using Bare10.Utils.Converters;
using Bare10.ViewModels.Items;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Achievements.ProgressView
{
    public abstract class AchievementProgressViewBase : MvxContentView
    {
        protected new View Content { get; }

        protected AchievementProgressViewBase()
        {
            var progressView = ProgressView();
            progressView.SetBinding(IsVisibleProperty, nameof(AchievementTierProgressViewModel.IsLocked),
                converter: new InvertedBooleanConverter());

            base.Content = new ContentView()
            {
                VerticalOptions = LayoutOptions.Start,
                Content = progressView,
            };

            Content = base.Content;
        }

        protected abstract View ProgressView();

    }
}