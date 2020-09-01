# Helsedirektoratet - Gå 10

## About

An app for Android and iOS for helping people get motivated to walk at least 10 minutes of brisk walk a day.
Collects data on average walking speed per minute from native APIs

- Android - [Google Fit Sdk](https://developers.google.com/fit/)
- iOS - [Pedometer](https://developer.apple.com/documentation/coremotion/cmpedometer)

## Requirements

The project requires that you have the following tools installed:

- [Visual Studio IDE 2019](http://visualstudio.com/) (with the following)
  - [Xamarin](https://docs.xamarin.com)

## Set up project

1. Open [the solution file](helsedirektoratet-bare-10-2018.sln)
2. Restore nuget packages for solution

## Notes

You should probably use actual devices rather than emulators to run the project, as you need proper walking data collected by the phone sensors.

### Public release

This is a public release of the source code, which has the following information and assets removed:

- App icons
- Image and animation assets
- API keys
- Certificates and provisioning profiles

### Platform specific services

There are two services which are platform specific: namely the ones implementing

[IWalkingDataService.cs](Bare10/Bare10/Services/Interfaces/IWalkingDataService.cs)

and

[IUpdateService.cs](Bare10/Bare10/Services/Interfaces/IUpdateService.cs)

These are loaded via lazy-loading in the following classes respectively (in the PCL)

[CrossWalkingDataService.cs](Bare10/Bare10/Services/Interfaces/CrossWalkingDataService.cs)

[CrossUpdateService.cs](Bare10/Bare10/Services/Interfaces/CrossUpdateService.cs)

These two files are added with link (by reference) to the Platform specific projects.

To make sure that the compiler does not optimize away the platform specific Service implementations, a [CrossServiceContainer](Bare10/Bare10/CrossServiceContainer.cs) class
has been created which contains the references. These are set at runtime at the earliest convenient time in the [iOS Appdelegate](Bare10/Bare10.iOS/AppDelegate.cs) and [Android MainActivity](Bare10/Bare10.Android/MainActivity.cs).
These are also set for the [Android UpdateJob](Bare10/Bare10.Android/Background/UpdateJob.cs) and the `PerformFetch()` method (for background polls) in the [iOS Appdelegate](Bare10/Bare10.iOS/AppDelegate.cs).

### Analytics

Analytics has been implemented for this app for tracking events. These include both data and UI events

### Crash reports and diagnostics

Crash reports and diagnostics have been implemented for this app, and all crashes should deliver a stack trace (where possible)

## Licenses

- [MvvmCross - MS-PL](https://opensource.org/licenses/ms-pl.html)
- [GooglePlayServicesComponents(Fitness) - MIT](https://github.com/xamarin/GooglePlayServicesComponents/blob/master/LICENSE.md)
- [Xamarin Android Appcompat - Mit](https://github.com/xamarin/AndroidSupportComponents/blob/master/LICENSE.md)
- [sqlite-net-pcl - MIT](https://github.com/praeclarum/sqlite-net/blob/master/LICENSE.md)
- [Settings plugins for Xamarin and Windows - MIT](https://github.com/jamesmontemagno/SettingsPlugin/blob/master/LICENSE)
- [CarouselView.FormsPlugin - MIT](https://github.com/alexrainman/CarouselView/blob/master/README.md)
- [Xamarin.FFImageLoading.Svg.Forms - MIT](https://raw.githubusercontent.com/luberda-molinet/FFImageLoading/master/LICENSE.md)
- [VG.XFShapeView.NetCore - MIT](https://github.com/vincentgury/XFShapeView/blob/master/LICENSE)
- [Rg.Plugins.Popup - MIT](https://github.com/rotorgames/Rg.Plugins.Popup/blob/master/LICENSE.md)
- [FlowListView for Xamarin.Forms - Apache](https://github.com/daniel-luberda/DLToolkit.Forms.Controls/blob/master/LICENSE)
- [LottieXamarin - Apache](https://github.com/martijn00/LottieXamarin/blob/master/LICENSE)
- [SkiaSharp - MIT](https://github.com/mono/SkiaSharp/blob/master/LICENSE.md)
- [Newtonsoft.Json - MIT](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)
- [Acr.UserDialogs - MIT](https://github.com/aritchie/userdialogs/blob/master/LICENSE.md)
