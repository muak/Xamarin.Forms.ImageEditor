using System;
using System.Threading.Tasks;
namespace Xamarin.Forms.ImageEditor
{
    public interface IImageEditor
    {
        IEditableImage CreateImage(byte[] imageArray);
        Task<IEditableImage> CreateImageAsync(byte[] imageArray);
    }

}
