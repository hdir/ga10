using Bare10.Resources;
using Bare10.ViewModels;
using Xamarin.Forms;

namespace Bare10.Pages.Views
{
    public class TestingView : ScrollView
    {
        public TestingView()
        {
            var stack = new StackLayout
            {
                Orientation = StackOrientation.Vertical
            };

            stack.Children.Add(Selectors());
            stack.Children.Add(Buttons());
            stack.Children.Add(DebugOutput());
            stack.Children.Add(Log());

            Content = stack;
        }

        private View Selectors()
        {
            var today = Selector("Today", nameof(TestingViewModel.SelectTodaysWalking));
            var daily = Selector("Daily", nameof(TestingViewModel.SelectDailyWalking));
            var achievements = Selector("Achievements", nameof(TestingViewModel.SelectAchievements));
            var goals = Selector("Goals", nameof(TestingViewModel.SelectGoals));

            return new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    today,
                    daily,
                    achievements,
                    goals,
                }
            };

        }

        private View Selector(string name, string bindingTarget)
        {
            var label = new Label { Text = name };
            var toggle = new Switch();
            toggle.SetBinding(Switch.IsToggledProperty, bindingTarget);

            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    label,
                    toggle,
                }
            };
        }

        private View Buttons()
        {
            var btnUpdate = new Button()
            {
                Text = "Request Update",
            };
            btnUpdate.SetBinding(Button.CommandProperty, nameof(TestingViewModel.RequestWalkingData));

            var btnGenerate = new Button()
            {
                Text = "Generate data",
            };
            btnGenerate.SetBinding(Button.CommandParameterProperty, nameof(TestingViewModel.SelectedTables));
            btnGenerate.SetBinding(Button.CommandProperty, nameof(TestingViewModel.GenerateTestData));

            var btnDelete = new Button()
            {
                Text = "Delete data from flags",
            };
            btnDelete.SetBinding(Button.CommandParameterProperty, nameof(TestingViewModel.SelectedTables));
            btnDelete.SetBinding(Button.CommandProperty, nameof(TestingViewModel.DeleteStoredData));

            var btnDeleteAllAndResetStartDate = new Button
            {
                Text = "Delete all and reset start date"
            };
            btnDeleteAllAndResetStartDate.SetBinding(Button.CommandProperty, nameof(TestingViewModel.DeleteAllAndResetStartDate));

            var btnCompleteGoal = new Button
            {
                Text = "Complete daily goal for today",
            };
            btnCompleteGoal.SetBinding(Button.CommandProperty, nameof(TestingViewModel.CompleteTodaysGoal));

            var buttons = new StackLayout
            {
                Children =
                {
                    btnUpdate,
                    btnGenerate,
                    btnDelete,
                    btnDeleteAllAndResetStartDate,
                    btnCompleteGoal,
                }
            };
            return buttons;
        }

        private View DebugOutput()
        {
            var outputLabel = new Label
            {
                FontSize = Sizes.TextSmall
            };
            outputLabel.SetBinding(Label.TextProperty, nameof(TestingViewModel.Output));

            return outputLabel;
        }

        private View Log()
        {
            var outputLabel = new Label
            {
                FontSize = Sizes.TextSmall
            };
            outputLabel.SetBinding(Label.TextProperty, nameof(TestingViewModel.Log));

            return outputLabel;
        }
    }
}
