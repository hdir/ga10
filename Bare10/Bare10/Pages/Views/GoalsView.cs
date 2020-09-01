using Bare10.Pages.Views.Goals;
using Bare10.ViewModels;
using DLToolkit.Forms.Controls;
using Xamarin.Forms;

namespace Bare10.Pages.Views
{
    public class GoalsView : ContentView
    {
        public GoalsView()
        {
            var goalList = new FlowListView(ListViewCachingStrategy.RecycleElement)
            {
                VerticalOptions = LayoutOptions.Fill,
                SeparatorVisibility = SeparatorVisibility.None,
                BackgroundColor = Color.Transparent,
                Margin = new Thickness(5, 0),
                HasUnevenRows = true,
                FlowColumnCount = 1,
                FlowColumnTemplate = new DataTemplate(typeof(GoalViewCell)),
            };
            goalList.SetBinding(FlowListView.FlowItemsSourceProperty, nameof(GoalsViewModel.Goals));
            goalList.SetBinding(FlowListView.FlowLastTappedItemProperty, nameof(GoalsViewModel.SelectedGoal));
            goalList.SetBinding(FlowListView.FlowItemTappedCommandProperty, nameof(GoalsViewModel.ItemTappedCommand));

            Content = goalList;
        }
    }
}
