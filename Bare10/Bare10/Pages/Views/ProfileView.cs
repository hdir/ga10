using Bare10.Localization;
using Bare10.Pages.Custom.ViewControllers;
using Bare10.Pages.Views.Items;
using Bare10.Resources;
using Bare10.Utils.Views;
using Bare10.ViewModels;
using FFImageLoading.Svg.Forms;
using MvvmCross.Forms.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views
{
    public class ProfileView : MvxContentView<ProfileViewModel>
    {
        public ProfileView()
        {
            BackgroundColor = Color.Transparent;

            var masterDetailView = new MasterDetailView()
            {
                Master = SettingsView(),
                ItemTemplate = new DetailDataTemplateSelector(),
                AnimationTime = 140,
            };
            masterDetailView.SetBinding(MasterDetailView.DetailViewProperty, nameof(ViewModel.SelectedSetting));
            masterDetailView.SetBinding(MasterDetailView.ShowDetailProperty, nameof(ViewModel.ShowDetailView));
            if (Device.RuntimePlatform == Device.iOS)
                masterDetailView.SetBinding(MasterDetailView.IsSwipeableProperty, nameof(ViewModel.ShowDetailView));

            Content = masterDetailView;
        }

        private View SettingsView()
        {
            var name = NameEntryView();
            var height = HeightEntryView();
            var notification = AllowNotificationsView();
            var statistics = AllowStatisticsView();
            var about = SettingWithDetailsView(AppText.profile_setting_about_title, SettingViewModel.SettingType.About);
            var terms = SettingWithDetailsView(AppText.profile_setting_terms_title, SettingViewModel.SettingType.Terms);
            var deleteData = SettingsTitleLabel(AppText.profile_setting_delete_title);
            deleteData.AddTouchCommand(new Binding(nameof(ViewModel.DeleteAllData)));
            var feedback = SettingWithDetailsView(AppText.profile_setting_give_feedback, SettingViewModel.SettingType.Feedback);

            var credit = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0),
                FontAttributes = FontAttributes.Italic, //TODO: add italic font
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro,
                Text = AppText.profile_setting_credit
            };

            var creditIcon = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 140,
                Aspect = Aspect.AspectFit,
                Source = Images.Helsedirektoratet,
            };

            var versionName = new Label()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                Margin = 8,
                Opacity = 0.4f,
                Text = $"v{App.VersionName}",
                FontSize = Sizes.TextSmall,
            };

            return new Grid()
            {
                Children =
                {
                    new ScrollView()
                    {
                        Content = new StackLayout()
                        {
                            Padding = new Thickness(24, 4),
                            Spacing = 0,
                            Children =
                            {
                                SectionWrapper(name),
                                SectionWrapper(height),
                                SectionWrapper(notification),
                                SectionWrapper(statistics),
                                SectionWrapper(about),
                                SectionWrapper(terms),
                                SectionWrapper(deleteData),
                                SectionWrapper(feedback),
                                //SectionWrapper(testing),    // TODO: Remove, here for debugging purposes
                                //SectionWrapper(update),    // TODO: Remove, here for debugging purposes
                                credit,
                                creditIcon,
                            }
                        },
                    },
#if  DEBUG
                    versionName,
#endif
                }
            };
        }

        private View NameEntryView()
        {
            var lblTitle = new Label()
            {
                FontSize = Sizes.TextMicro,
                Text = AppText.profile_name_header,
                TextColor = Colors.TextFaded,
            };

            var entry = new Entry()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.TextSpecial,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.Transparent,
                FontSize = Sizes.Title,
                Keyboard = Keyboard.Text,
                IsSpellCheckEnabled = false,
            };
            entry.SetBinding(Entry.TextProperty, nameof(ViewModel.Name));
            entry.AddTouch((sender, args) =>
            {
                entry.Focus();
            });

            var pen = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = Sizes.Title,
                WidthRequest = Sizes.Title,
                Aspect = Aspect.AspectFit,
                Source = Images.ProfilePen,
            };
            pen.AddTouch((sender, args) =>
            {
                entry.Focus();
            });

            entry.Focused += (sender, args) =>
            {
                pen.IsVisible = false;
                entry.InputTransparent = false;

                // crude fix to place cursor at end of text
                var text = entry.Text;
                entry.Text = "";
                entry.Text = text;
            };
            entry.Unfocused += (sender, args) =>
            {
                pen.IsVisible = true;
                entry.InputTransparent = true;
            };

            return new StackLayout()
            {
                Spacing = Device.RuntimePlatform == Device.Android ? -5 : 0,
                Children =
                {
                    lblTitle,
                    new Grid()
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection()
                        {
                            new ColumnDefinition(){ Width = GridLength.Star },
                            new ColumnDefinition(){ Width = GridLength.Auto },
                        },
                        Children =
                        {
                            {entry, 0, 0},
                            {pen, 1, 0},
                        }
                    },
                }
            };
        }

        private View HeightEntryView()
        {
            var lblTitle = new Label()
            {
                FontSize = Sizes.TextMicro,
                Text = "HØYDE:",
                TextColor = Colors.TextFaded,
            };

            var entry = new Entry()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Transparent,
                FontSize = Sizes.TextLarge,
                Keyboard = Keyboard.Numeric,
                MaxLength = 3,
                IsSpellCheckEnabled = false,
                WidthRequest = 100,
            };
            entry.SetBinding(Entry.TextProperty, nameof(ViewModel.Height));

            var lblUnit = new Label()
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                FontSize = Sizes.TextLarge,
                Text = "cm",
            };

            var unitLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 1,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    entry,
                    lblUnit,
                }
            };
            unitLayout.AddTouch((sender, args) =>
            {
                entry.Focus();
            });

            return new StackLayout()
            {
                Spacing = Device.RuntimePlatform == Device.Android ? -5 : 0,
                Children =
                {
                    lblTitle,
                    unitLayout,
                }
            };
        }

        private static View SwitchView(string title, string description, BindingBase binding)
        {
            var lblTitle = SettingsTitleLabel(title);

            var toggle = new Switch();
            toggle.SetBinding(Switch.IsToggledProperty, binding);

            var lblDescription = new Label()
            {
                VerticalOptions = LayoutOptions.Start,
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextSmall,
                Text = description,
            };

            return new Grid()
            {
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 8f),
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){Height = GridLength.Auto},
                    new RowDefinition(){Height = GridLength.Auto},
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition(){Width = GridLength.Star},
                    new ColumnDefinition(){Width = GridLength.Auto},
                },
                Children =
                {
                    {lblTitle, 0, 0},
                    {toggle, 1, 0},
                    {lblDescription, 0, 2, 1, 2}
                }
            };
        }

        private View AllowNotificationsView()
        {
            return SwitchView(AppText.profile_setting_notification_title,
                AppText.profile_setting_notification_description,
                new Binding(nameof(ViewModel.WillSendNotifications), BindingMode.TwoWay));
        }

        private View AllowStatisticsView()
        {
            return SwitchView(AppText.profile_setting_statistics_title,
                AppText.profile_setting_statistics_description,
                new Binding(nameof(ViewModel.WillRegisterStatistics), BindingMode.TwoWay));
        }

        private View SettingWithDetailsView(string title, SettingViewModel.SettingType commandParameterBinding)
        {
            var lbl = SettingsTitleLabel(title);

            //var arrow = new SvgCachedImage()
            //{
            //    HorizontalOptions = LayoutOptions.End,
            //    VerticalOptions = LayoutOptions.Center,
            //    WidthRequest = 30,
            //    HeightRequest = 20,
            //    Source = Images.ProfileArrow,
            //    Aspect = Aspect.AspectFit,
            //};
            //arrow.ReplaceColor(Colors.Text);

            var layout = new Grid()
            {
                Children =
                {
                    lbl,
                    //arrow,
                }
            };
            layout.AddTouchCommand(new Binding(nameof(ViewModel.SelectedSettingCommand)), commandParameterBinding);

            return layout;
        }

        private static View SectionWrapper(View content)
        {
            var padding = new Thickness(1, 10);

            var underline = DefaultView.SectionLine;
            underline.Margin = new Thickness(0, 0, 0, -padding.Bottom);

            var grid = new Grid()
            {
                Padding = padding,
                Children =
                {
                    content,
                    underline
                }
            };

            return grid;
        }

        private static Label SettingsTitleLabel(string text) => new Label()
        {
            HorizontalOptions = LayoutOptions.Start,
            FontAttributes = FontAttributes.Bold,
            FontSize = Sizes.TextMedium,
            Text = text,
        };
    }

    public class DetailDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is SettingViewModel setting)
            {
                switch (setting.Type)
                {
                    case SettingViewModel.SettingType.About:
                        return new DataTemplate(typeof(AboutScrollView));
                    case SettingViewModel.SettingType.Terms:
                        return new DataTemplate(typeof(TermsScrollView));
                    case SettingViewModel.SettingType.Feedback:
                        return new DataTemplate(typeof(FeedbackView));
                    case SettingViewModel.SettingType.Testing:
                        return new DataTemplate(typeof(TestingView));
                }
            }

            return null;
        }
    }
}
