using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Bare10.Resources;
using Bare10.Utils.Views;
using Xamarin.Forms;

namespace Bare10.Pages.Views.Items
{
    public class FeedbackView : ContentView
    {
        public FeedbackView()
        {
            var webView = new WebView()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Source = Config.FeedbackUrl,
            };
            var spinner = new ActivityIndicator()
            {
                HorizontalOptions = LayoutOptions.Center,
                IsRunning = true,
            };

            var lbl = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Text = "Laster",
            };

            var retry = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                IsVisible =  false,
                FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        new Span()
                        {
                            Text = "\u21BB",
                            FontSize = Sizes.TextLarge,
                        },
                        new Span()
                        {
                            Text = "\nPrøv igjen",
                        },
                    }
                }
            };
            retry.AddTouch(async (sender, args) =>
            {
                if (!spinner.IsRunning)
                {
                    spinner.IsRunning = true;
                    lbl.Text = "Laster";
                    retry.IsVisible = false;
                    await Task.Delay(1000);
                    webView.Reload();
                }
            });

            var overlay = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    spinner,
                    lbl,
                    retry,
                }
            };

            webView.Navigated += (sender, args) =>
            {
                switch (args.Result)
                {
                    case WebNavigationResult.Success:
                        overlay.IsVisible = false;
                        webView.IsVisible = true;
                        break;
                    case WebNavigationResult.Cancel:
                    case WebNavigationResult.Timeout:
                    case WebNavigationResult.Failure:
                        lbl.Text = "Nettverksproblem";
                        spinner.IsRunning = false;
                        webView.IsVisible = false;
                        retry.IsVisible = true;
                        break;
                    default:
                        break;
                }
            };

            Content = new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Children =
                {
                    webView,
                    overlay
                }
            };
        }
    }

    //<WebView x:Name="WebView"
    //HorizontalOptions="Fill"
    //VerticalOptions="Fill"
    //Source="{x:Static heiaMeg:Config.FeedbackUrl}"
    //Navigated="WebView_OnNavigated" />

    //<StackLayout x:Name="InfoView"
    //HorizontalOptions="Center"
    //VerticalOptions="Center">

    //<ActivityIndicator x:Name="Spinner"
    //HorizontalOptions="Center"
    //IsRunning="True" />

    //<Label x:Name="TextLabel"
    //HorizontalOptions="Center"
    //HorizontalTextAlignment="Center"
    //Text="Laster" />
    //</StackLayout>
}