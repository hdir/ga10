using Bare10.ViewModels.Accessibility.Items;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Bare10.Pages.Accessibility.Items
{
    public class AchievementMedalView : ViewCell
    {
        public AchievementMedalView()
        {
            View = Utils.Accessibility.CreateLabel("", nameof(AchievementMedalViewModel.Text));
        }
    }
}
