using System.Threading.Tasks;

namespace Xamarin.Forms.ImageEditor.iOS
{
    public class ImageEditor : IImageEditor
    {
        public IEditableImage CreateImage(byte[] imageArray)
        {
            return new EditableImage(imageArray);
        }

        public async Task<IEditableImage> CreateImageAsync(byte[] imageArray)
        {
            return await Task.Run(() => CreateImage(imageArray));
        }
    }
}
