using Xamarin.Forms;

namespace Bare10.ViewModels.Items
{
    public interface IInfoViewModel
    {
        string Title { get; }
        string Description { get; }
        ImageSource Icon { get; }
        string Animation { get; }
    }
}
