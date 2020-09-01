using System;
using System.Collections.Generic;
using Bare10.Resources;
using Bare10.ViewModels;
using Bare10.Localization;
using Bare10.Pages.Custom;
using Bare10.Pages.Custom.Graph;
using Bare10.Pages.Views.Items;
using Bare10.Utils;
using Bare10.Utils.Converters;
using Bare10.Utils.Views;
using FFImageLoading.Svg.Forms;
using Xamarin.Forms;
using XFShapeView;

namespace Bare10.Pages.Views
{
    public class HistoryView : ScrollView
    {
        private const int SectionPadding = 18;

        public HistoryView()
        {
            var progressWheel = new ProgressWheelView()
            {
                WidthRequest = App.ScreenWidth - SectionPadding,
                HeightRequest = App.ScreenWidth - SectionPadding,
                Margin = new Thickness(0, 0, 0, 0 ),
            };
            progressWheel.SetBinding(BindingContextProperty, nameof(HistoryViewModel.Progress));

            Content = new Grid()
            {
                Children =
                {
                    new StackLayout()
                    {
                        Margin = 0,
                        Spacing = 10,
                        Children =
                        {
                            Section(false,
                                progressWheel,
                                ProgressOverview()
                                //GoalTotalView()
                            ),
                            Section(
                                true, 
                                TodayView()
                            ),
                            Section(true,
                                WeekView()
                            ),
                            Section(true,
                                MonthView()
                            ),
                        }
                    }, 
                }
            };
        }

        #region Progress Overview

        private static View ProgressOverview()
        {
            var left = ProgressOverviewElement(AppText.brisk_walk,
                new Binding(nameof(HistoryViewModel.TodaysBriskWalkingMinutes)),
                Colors.TextBriskWalk,
                LayoutOptions.Start, 
                "brisk");

            var right = ProgressOverviewElement(AppText.regular_walk,
                new Binding(nameof(HistoryViewModel.TodaysRegularWalkingMinutes)),
                Colors.TextNormalWalk,
                LayoutOptions.End, 
                "normal");

            return new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(SectionPadding, 0, SectionPadding, SectionPadding),
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() { Width = GridLength.Star },
                },
                Children =
                {
                    {left, 0, 0},
                    {right, 1, 0},
                }
            };
        }

        private static View ProgressOverviewElement(string title, BindingBase countBinding, Color color, 
            LayoutOptions alignment, string parameter)
        {
            var lblTitle = new Label()
            {
                HorizontalOptions = alignment,
                Text = title.ToUpper(),
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro,
            };

            var infoSize = (int) Sizes.TextMicro - 1;
            var info = new SvgCachedImage()
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = infoSize,
                HeightRequest = infoSize,
                Source = Images.HistoryInfo,
            };
            info.ReplaceColor(Colors.TextFaded);

            var titleLayout = new StackLayout()
            {
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    lblTitle,
                    info,
                }
            };

            var line = DefaultView.UnderLine;
            line.BackgroundColor = color;
            line.Margin = new Thickness(0, 0, infoSize, 0);

            var spanMinutes = new Span()
            {
                TextColor = color,
                FontAttributes = FontAttributes.Bold,
                FontSize = Sizes.TextLarge,
            };
            countBinding.Mode = BindingMode.OneWay; //Span default is OneTime: https://github.com/xamarin/Xamarin.Forms/issues/2177
            spanMinutes.SetBinding(Span.TextProperty, countBinding);

            var lblCounter = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(0, 0, infoSize, 0),
                FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        spanMinutes,
                        new Span()
                        {
                            Text = " min",
                            TextColor = color,
                            FontSize = Sizes.TextLarge,
                        }
                    }
                }
            };
            //lblCounter.SetBinding(IsVisibleProperty, nameof(HistoryViewModel.IsLoadingWalkingData));

            var layout = new StackLayout()
            {
                HorizontalOptions = alignment,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    titleLayout,
                    line,
                    lblCounter
                }
            };
            layout.AddTouchCommand(new Binding(nameof(HistoryViewModel.OpenInfo)), parameter);

            return layout;
        }

        #endregion

        #region Goal total
        private View GoalTotalView()
        {
            var preText = new Label
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.TextMicro,
                Text = "Du har oppnådd dagens mål ",
            };

            var goalNumberCircle = new ShapeView
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                ShapeType = ShapeType.Circle,
                BackgroundColor = Color.Transparent,
                BorderWidth = 1,
                BorderColor = Colors.ProgressWheelForeground,
                //Scale = 2,
                WidthRequest = 35,
                HeightRequest = 35,
            };

            var goalNumber = new Label
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.TextMicro,
            };
            goalNumber.SetBinding(Label.TextProperty, nameof(HistoryViewModel.GoalsMetTotal));

            var postText = new Label
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Sizes.TextMicro,
            };
            postText.SetBinding(Label.TextProperty, nameof(HistoryViewModel.GoalsMetPostfix));

            var goalTotalLayout = new Grid
            {
                Scale = 1.2f,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(30, 30),
                ColumnSpacing = 5,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                },
                Children =
                {
                    { preText, 0, 0 },
                    { goalNumberCircle, 1, 0 },
                    { goalNumber, 1, 0 },
                    { postText, 2, 0 },
                },
            };

            var goalTotal = new ShapeView
            {
                Margin = new Thickness(10, 0, 10, 15),
                BackgroundColor = Color.Transparent,
                BorderColor = Colors.AchievementOutline,
                BorderWidth = 1f,
                Content = goalTotalLayout,
            };
            goalTotal.SizeChanged += (s, a) => goalTotal.CornerRadius = (float)goalTotal.Height * 0.5f;
            goalTotal.SetBinding(IsVisibleProperty, nameof(HistoryViewModel.ShouldShowGoalsMet));

            return goalTotal;
        }
        #endregion

        #region Today

        private View TodayView()
        {
            var leftGradient = new GradientView(Colors.Background)
            {
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = SectionPadding + 10,
            };
            var rightGradient = new GradientView(Colors.Background)
            {
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = SectionPadding + 10,
                Margin = new Thickness(0, 0, -1, 0),
                ScaleX = -1,
            };

            var todaysGraphContainer = new ScrollView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(-SectionPadding, 0, -SectionPadding, 0),
                Padding = new Thickness(SectionPadding, 0, SectionPadding, 0),
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                Orientation = ScrollOrientation.Horizontal,
                Content = TodayGraph(),
                ScaleX = -1,
            };

            //var arrow = new SvgCachedImage()
            //{
            //    HorizontalOptions = LayoutOptions.End,
            //    VerticalOptions = LayoutOptions.Center,
            //    WidthRequest = 40,
            //    HeightRequest = 12,
            //    Aspect = Aspect.AspectFit,
            //    Source = Images.ToolbarArrow,
            //    IsVisible = false,
            //};
            //arrow.ReplaceColor(Colors.Text);

            //scrollView.SizeChanged += (sender, args) =>
            //{
            //    var parent = ((ScrollView) sender);
            //    var child = ((ScrollView)sender).Children[0];
            //    if (child != null)
            //    {
            //        if (((View) child).Width > parent.Width)
            //            arrow.IsVisible = true;
            //    }
            //};
            //scrollView.Scrolled += async (sender, args) =>
            //{
            //    var parent = ((ScrollView) sender);
            //    if (!arrow.IsVisible && parent.ScrollX <= 0)
            //    {
            //        arrow.IsVisible = true;
            //        await arrow.FadeTo(1f, 100);
            //    }
            //    else if (arrow.IsVisible && parent.ScrollX > 0)
            //    {
            //        await arrow.FadeTo(0f, 100);
            //        arrow.IsVisible = false;
            //    }
            //};

            var fallback = new Label()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMicro,
                Opacity = 0.6f,
                Margin = new Thickness(40, 0),
                Text = AppText.history_todays_graph_fallback,
            };
            fallback.SetBinding(IsVisibleProperty, nameof(HistoryViewModel.HourlyData),
                converter: new ListIsEmptyToBooleanConverter());

            return new StackLayout()
            {
                Spacing = 20f,
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    new Grid()
                    {
                        Children =
                        {
                            SectionTitle(AppText.history_chart_today_title, 
                                AppText.history_chart_generic_title_today),
                            //arrow,
                        }
                    },
                    new Grid()
                    {
                        Margin = new Thickness(-SectionPadding, 0),
                        Children = {
                            todaysGraphContainer,
                            fallback,
                            leftGradient,
                            rightGradient,
                        }
                    }
                }
            };
        }

        private View TodayGraph()
        {
            var graph = new HourlyAreaGraph()
            {
                Padding = new Thickness(SectionPadding, 0),
                HeightRequest = 100f,
                ScaleX = -1,
            };
            graph.SetBinding(HourlyAreaGraph.ItemSourceProperty, nameof(HistoryViewModel.HourlyData));

            return graph;
        }

        #endregion

        #region Week

        private View WeekView()
        {
            var title = SectionTitle(AppText.history_chart_week_title);
            var graph = WeekGraph();
            return new StackLayout()
            {
                Spacing = 10f,
                Children =
                {
                    title,
                    graph,
                }
            };
        }

        private View WeekGraph()
        {
            var graph = new BarChartGraph()
            {
                HeightRequest = 170f,
                yMax = 40,
                VerticalLabels = VerticalLabels,
                HorizontalLabels = HorizontalWeekLabels,
                ShowFirstAndLastLines = false,
            };
            graph.SetBinding(BarChartGraph.ItemSourceProperty, nameof(HistoryViewModel.WeekData));
            return graph;
        }

        private static IList<View> HorizontalWeekLabels
        {
            get
            {
                var list = new List<View>()
                {
                    HorizontalLabel(DateTimeUtils.DayOfWeekToShortName(DayOfWeek.Monday)),
                    HorizontalLabel(DateTimeUtils.DayOfWeekToShortName(DayOfWeek.Tuesday)),
                    HorizontalLabel(DateTimeUtils.DayOfWeekToShortName(DayOfWeek.Wednesday)),
                    HorizontalLabel(DateTimeUtils.DayOfWeekToShortName(DayOfWeek.Thursday)),
                    HorizontalLabel(DateTimeUtils.DayOfWeekToShortName(DayOfWeek.Friday)),
                    HorizontalLabel(DateTimeUtils.DayOfWeekToShortName(DayOfWeek.Saturday)),
                    HorizontalLabel(DateTimeUtils.DayOfWeekToShortName(DayOfWeek.Sunday)),
                };

                var today = ((Label) list[DateTimeUtils.DateTimeToDayOfWeekIndexInNorwegian(DateTime.Now)]);
                today.TextColor = Colors.Text;
                //today.FontAttributes = FontAttributes.Bold;

                return list;
            }
        } 

        #endregion

        #region Month

        private View MonthView()
        {
            var title = SectionTitle(AppText.history_chart_month_title);
            var graph = MonthGraph();
            return new StackLayout()
            {
                Spacing = 10f,
                Children =
                {
                    title,
                    graph
                }
            };
        }

        private View MonthGraph()
        {
            var graph = new BarChartGraph()
            {
                HeightRequest = 170f,
                yMax = 40,
                VerticalLabels = VerticalLabels,
                HorizontalLabels = HorizontalMonthLabels,
                ShowFirstAndLastLines = true,
            };
            graph.SetBinding(BarChartGraph.ItemSourceProperty, nameof(HistoryViewModel.MonthData));
            return graph;
        }

        private static IList<View> HorizontalMonthLabels
        {
            get
            {
                var startDate = HorizontalLabel(DateTime.Now.AddDays(-30).ToString("d MMM.", Language.NORWEGIAN).ToUpper(), TextAlignment.Start);
                startDate.HorizontalOptions = LayoutOptions.StartAndExpand;
                startDate.FontSize = Sizes.TextMicro;

                var endDate = HorizontalLabel(DateTime.Now.ToString("d MMM.", Language.NORWEGIAN).ToUpper(), TextAlignment.End);
                endDate.HorizontalOptions = LayoutOptions.EndAndExpand;
                endDate.TextColor = Colors.Text;
                endDate.FontSize = Sizes.TextMicro;
                endDate.FontAttributes = FontAttributes.Bold;

                return new List<View>()
                {
                    startDate,
                    endDate
                };
            }
        }

        #endregion

        #region Graphs
        private static IList<View> VerticalLabels
        {
            get
            {
                var labels = new List<View>()
                {
                    VerticalLabel("40 min"),
                    VerticalLabel("30 min"),
                    VerticalLabel("20 min"),
                    VerticalLabel("10 min"),
                    VerticalLabel("0 min"),
                };
                var selected = labels.Count - (int)Settings.CurrentGoal - 2;
                ((Label) labels[selected]).TextColor = Colors.TextBriskWalk;

                return labels;
            }
        }

        private static Label HorizontalLabel(string text, TextAlignment alignment = TextAlignment.Center) 
            => new Label()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End,
                HorizontalTextAlignment = alignment,
                FontSize = Sizes.TextMicro + 2,
                TextColor = Colors.TextFaded,
                Text = text,
                LineBreakMode = LineBreakMode.NoWrap,
            };

        private static Label VerticalLabel(string text) => new Label()
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.StartAndExpand,
            HorizontalTextAlignment = TextAlignment.End,
            VerticalTextAlignment = TextAlignment.Center,
            FontSize = Sizes.TextMicro,
            TextColor = Colors.TextFaded,
            Text = text,
        };
        #endregion

        #region Section

        private static View SectionTitle(string titleText, string generic = null)
        {
            var lblGeneric = new Span()
            {
                Text = $"{generic ?? AppText.history_chart_generic_title} - ",
                TextColor = Colors.TextFaded,
                FontSize = Sizes.TextMedium,
            };

            var lblTitle = new Span()
            {
                Text = titleText,
                TextColor = Colors.Text,
                FontSize = Sizes.TextMedium,
            };

            return new Label()
            {
                FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        lblGeneric, 
                        lblTitle,
                    }
                }
            };
        }

        private static View Section(bool padding, params View[] children)
        {
            var section = new StackLayout()
            {
                Padding = !padding ? 0 : new Thickness(SectionPadding, SectionPadding, SectionPadding, SectionPadding),
            };

            foreach (var child in children)
                section.Children.Add(child);

            return new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    section,
                    DefaultView.SectionLine,
                }
            };
        }

        private static ActivityIndicator Indicator => new ActivityIndicator()
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            HeightRequest = 40,
            WidthRequest = 40,
            Margin = 20,
            IsRunning = true,
            IsVisible = false,
            Color = Colors.TextSpecial,
        };

        #endregion
    }
}
