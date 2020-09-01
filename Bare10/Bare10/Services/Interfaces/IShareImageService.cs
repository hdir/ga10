namespace Bare10.Services.Interfaces
{
    public interface IShareImageService
    {
        void Show(string title, string message, byte[] imageData);
    }
}
