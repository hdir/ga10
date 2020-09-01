using Bare10.Localization;
using Bare10.ViewModels.Accessibility;
using Xamarin.Forms;

namespace Bare10.Pages.Accessibility
{
    public class HistoryPage : AccessibilityBasePage<HistoryViewModel>
    {
        public HistoryPage() 
            : base(false)
        {
            var hourlyList = Utils.Accessibility.CreateListWithHeader(AppText.accessibility_history_hourly_breakdown_label, nameof(HistoryViewModel.HourlyBreakdown));
            var weeklyList = Utils.Accessibility.CreateList(AppText.accessibility_history_weekly_breakdown_label, nameof(HistoryViewModel.WeeklyBreakDown));
            var monthlyList = Utils.Accessibility.CreateListWithHeader(AppText.accessibility_history_monthly_breakdown_label, nameof(HistoryViewModel.MonthlyBreakdown));

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                },
                Children =
                {
                    { hourlyList, 0, 0 },
                    { weeklyList, 0, 1 },
                    { monthlyList, 0, 2 },
                },
            };

            AddContent(grid);
        }
    }
}
