using System.Windows.Input;
using FFImageLoading.Svg.Forms;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Bare10.Services.Interfaces
{
    public class Bulletin : MvxViewModel
    {
        public string Title { get; private set; }
        public string Description { get; private set; }

        private string _iconSource;
        public string IconSource
        {
            get => _iconSource;
            set => SetProperty(ref _iconSource, value);
        }

        private string _animation;
        public string Animation
        {
            get => _animation;
            set => SetProperty(ref _animation, value);
        }

        private Content.Tier _tier;
        public Content.Tier Tier
        {
            get => _tier;
            set => SetProperty(ref _tier, value);
        }

        [JsonIgnore]
        public ICommand CloseCommand { get; }

        [JsonIgnore] //NOTE: Bulletin is stored as JSON
        public ImageSource Icon => IconSource != null ? SvgImageSource.FromResource(IconSource) : null;

        public Bulletin(string title, string description)
        {
            Title = title;
            Description = description;

            CloseCommand = new Command(() =>
            {
                BulletinService.Instance.BulletinClosed(this);
            });
        }
    }
}
