﻿using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.ImageEditor.iOS;

[assembly: Dependency(typeof(ImageEditor))]
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
