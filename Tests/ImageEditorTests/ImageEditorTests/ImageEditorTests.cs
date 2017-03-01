﻿using System;
using Xunit;
using Xamarin.Forms.ImageEditor;
using System.Threading.Tasks;
#if __ANDROID__
using Xamarin.Forms.ImageEditor.Droid;
using Xamarin.Forms.Platform.Android;
using ImageEditorTests.Droid;
using Android.Graphics;
using System.Security.AccessControl;
using Java.IO;
#endif
#if __IOS__
using Xamarin.Forms.ImageEditor.iOS;
using UIKit;
using Foundation;
#endif

namespace Tests
{
    public class MainFixture : IDisposable
    {
        public byte[] PngData { get; set; }
        public byte[] JpegData { get; set; }
        public byte[] ShrinkingData { get; set; }
        public byte[] ExpansionData { get; set; }

        public MainFixture()
        {
#if __IOS__
            PngData = UIImage.FromBundle("pattern1.png").AsPNG().ToArray();
            JpegData = UIImage.FromBundle("pattern1.jpg").AsJPEG().ToArray();
            ShrinkingData = UIImage.FromBundle("shrinking.png").AsPNG().ToArray();
            ExpansionData = UIImage.FromBundle("expansion.png").AsPNG().ToArray();
#endif
#if __ANDROID__
            PngData = Getbytes("pattern1");
            JpegData = Getbytes("patternjpg");
            ShrinkingData = Getbytes("shrinking");
            ExpansionData = Getbytes("expansion");
#endif

        }

#if __ANDROID__
        byte[] Getbytes(string name)
        {
            var res = MainActivity.Context.Resources.GetIdentifier(name, "drawable", MainActivity.Context.PackageName);


            using (var raw = MainActivity.Context.Resources.OpenRawResource(res))
            using (var stream = new ByteArrayOutputStream()) {
                var buff = new byte[10240];
                var i = int.MaxValue;
                while ((i = raw.Read(buff, 0, buff.Length)) > 0) {
                    stream.Write(buff, 0, i);
                }

                return stream.ToByteArray();
            }
        }
#endif

        public void Dispose()
        {

        }
    }

    public class Main : IClassFixture<MainFixture>
    {
        MainFixture _main;
        IImageEditor _editor;
        public Main(MainFixture main)
        {
            _main = main;
            _editor = new ImageEditor();
        }

        [Fact]
        public void CreateImageTest()
        {
            using (var image = _editor.CreateImage(_main.PngData)) {
                CreateCompare(image);
            }
            using (var image = _editor.CreateImage(_main.JpegData)) {
                CreateCompare(image, false);
            }
        }

        [Fact]
        public async Task CreateImageAsyncTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {
                CreateCompare(image);
            }
            using (var image = await _editor.CreateImageAsync(_main.JpegData)) {
                CreateCompare(image, false);
            }
        }

        void CreateCompare(IEditableImage image, bool isPng = true)
        {
            var array = image.ToArgbPixels();

            Assert.Equal(3, image.Width);
            Assert.Equal(3, image.Height);

            if (isPng) {
                for (var i = 0; i < 3; i++) {
                    Assert.Equal(0xFF000000, (uint)array[i]);
                }
                for (var i = 3; i < array.Length; i++) {
                    Assert.NotEqual(0xFF000000, (uint)array[i]);
                }
            }
        }

        [Fact]
        public async Task Rotate90Test()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Rotate(90).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if ((i + 1) % 3 == 0) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }


        [Fact]
        public void Rotate180Test()
        {
            using (var image = _editor.CreateImage(_main.PngData)) {

                var pixels = image.Rotate(180).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if (i >= 6) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }

        [Fact]
        public async Task Rotate270Test()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Rotate(270).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if (i % 3 == 0) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }

        [Fact]
        public async Task Rotate90negativeTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Rotate(-90).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 3);
                Assert.Equal(image.Height, 3);

                for (var i = 0; i < pixels.Length; i++) {
                    if (i % 3 == 0) {
                        Assert.Equal(0xFF000000, (uint)pixels[i]);
                    }
                    else {
                        Assert.NotEqual(0xFF000000, (uint)pixels[i]);
                    }
                }
            }
        }

        [Fact]
        public async Task CropTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.PngData)) {

                var pixels = image.Crop(1, 1, 1, 1).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 1);
                Assert.Equal(image.Height, 1);

                Assert.Equal(0xFFFF0000, (uint)pixels[0]);
            }
        }

        [Fact]
        public async Task ResizeShrinkingTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.ShrinkingData)) {

                var pixels = image.Resize(2, 2).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 2);
                Assert.Equal(image.Height, 2);

                // approximate white
                Assert.InRange((uint)pixels[0], 0xFFE0E0E0, 0xFFFFFFFF);
                Assert.InRange((uint)pixels[1], 0xFFE0E0E0, 0xFFFFFFFF);
                Assert.InRange((uint)pixels[2], 0xFFE0E0E0, 0xFFFFFFFF);
                // approximate black
                Assert.InRange((uint)pixels[3], 0xFF000000, 0xFF3F3F3F);
            }
        }

        [Fact]
        public async Task ResizeExpansionTest()
        {
            using (var image = await _editor.CreateImageAsync(_main.ExpansionData)) {

                var pixels = image.Resize(4, 4).ToArgbPixels();

                SavePhoto(image);

                Assert.Equal(image.Width, 4);
                Assert.Equal(image.Height, 4);

                for (var i = 0; i < image.Height; i++) {
                    for (var j = 0; j < image.Width; j++) {
                        if (i >= 2 && j >= 2) {
                            // approximate black  (very loose for android)
                            Assert.InRange((uint)pixels[i * image.Width + j], 0xFF000000, 0xFF6F6F6F); 
                        }
                        else {
                            // approximate white (very loose for android)
                            Assert.InRange((uint)pixels[i * image.Width + j], 0xFFBFBFBF, 0xFFFFFFFF);
                        }
                    }
                }
            }
        }

        private void SavePhoto(IEditableImage image)
        {
#if __IOS__
            var tmp = new UIImage(NSData.FromArray(image.ToPng()));
            tmp.BeginInvokeOnMainThread(() => {
                tmp.SaveToPhotosAlbum(new UIImage.SaveStatus((UIImage affs, NSError error) => {
                    ;
                }));
            });
#endif
#if __ANDROID__

#endif
        }
    }
}
