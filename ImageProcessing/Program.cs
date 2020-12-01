using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static void IconFiles()
        {
            Icon icon1 = new Icon(SystemIcons.Exclamation, 40, 40);
            Stream stream = new FileStream(@"C:\ImageSamples\SystemIcons.Exclamation.ico", FileMode.CreateNew);
            icon1.Save(stream);

            icon1.Dispose();
            stream.Dispose();
            Icon icon2 = Icon.ExtractAssociatedIcon(@"C:\Windows\explorer.exe");
            Icon icon3 = new Icon(@"C:\ImageSamples\SystemIcons.Exclamation.ico");
            Bitmap bitmap2 = icon2.ToBitmap();
            Bitmap bitmap3 = icon3.ToBitmap();
            
            bitmap2.Save(@"C:\ImageSamples\explorer.exe.png", ImageFormat.Png);
            bitmap3.Save(@"C:\ImageSamples\SystemIcons.Exclamation.png", ImageFormat.Png);
        }

        private static void AnaimatedGif()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\tiffFrame_0.png");
            using Bitmap image2 = (Bitmap)Image.FromFile(@"C:\ImageSamples\tiffFrame_1.png");
            using Bitmap image3 = (Bitmap)Image.FromFile(@"C:\ImageSamples\tiffFrame_2.png");

            image.Save(@"C:\ImageSamples\tiffFrame_0.gif", ImageFormat.Gif);
            image2.Save(@"C:\ImageSamples\tiffFrame_1.gif", ImageFormat.Gif);
            image3.Save(@"C:\ImageSamples\tiffFrame_2.gif", ImageFormat.Gif);

            using Bitmap gifImage = (Bitmap)Image.FromFile(@"C:\ImageSamples\tiffFrame_0.gif");
            using Bitmap gifImage1 = (Bitmap)Image.FromFile(@"C:\ImageSamples\tiffFrame_1.gif");
            using Bitmap gifImage2 = (Bitmap)Image.FromFile(@"C:\ImageSamples\tiffFrame_2.gif");

            ImageCodecInfo gifEncroder = null;

            foreach (ImageCodecInfo item in ImageCodecInfo.GetImageEncoders())
            {
                if (item.MimeType == "image/gif")
                {
                    gifEncroder = item;

                    break;
                }
            }

            if (gifEncroder == null)
            {
                Console.WriteLine("Gif encoder is null!");

                return;
            }

            // create a new gif file
            using (var stream = new FileStream(@"C:\ImageSamples\animation.gif", FileMode.Create))
            {
                int PropertyTagFrameDelay = 0x5100;
                int propertyTagFrameloop = 0x5101;
                int unitBybtes = 4;

                PropertyItem frameDelay = gifImage.GetPropertyItem(PropertyTagFrameDelay);
                PropertyItem loopPropertyItem = gifImage.GetPropertyItem(propertyTagFrameloop);

                var encoderParams1 = new EncoderParameters(1)
                {
                    Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame) }
                };

                Bitmap animatedGif = gifImage;

                frameDelay.Len = 3 * unitBybtes;
                frameDelay.Value = new byte[3 * unitBybtes];
                loopPropertyItem.Value = BitConverter.GetBytes((ushort)0);

                animatedGif.SetPropertyItem(frameDelay);
                animatedGif.SetPropertyItem(loopPropertyItem);

                animatedGif.Save(stream, gifEncroder, encoderParams1);

                var encoderParamsN = new EncoderParameters(1)
                {
                    Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionTime) }
                };

                animatedGif.SaveAdd(gifImage1, encoderParamsN);
                animatedGif.SaveAdd(gifImage2, encoderParamsN);

                var encoderParamsFlush = new EncoderParameters(1)
                {
                    Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush) }
                };

                animatedGif.SaveAdd(encoderParamsFlush);
            }

            var test = ImageAnimator.CanAnimate(new Bitmap(@"C:\ImageSamples\animation.gif"));
        }

        private static void ReadTiffFiles()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World.tif");

            var dimension = new FrameDimension(image.FrameDimensionsList[0]);
            var frameCount = image.GetFrameCount(dimension);

            for (int i = 0; i < frameCount; i++)
            {
                using (Stream currentFram = new MemoryStream())
                {
                    image.SelectActiveFrame(dimension, i);
                    image.Save(currentFram, ImageFormat.Png);

                    var tiffFrame = Image.FromStream(currentFram);

                    tiffFrame.Save(@$"C:\ImageSamples\tiffFrame_{i}.png");
                }
            }
        }

        private static void CreateTiffFile()
        {
            ImageCodecInfo imageCodecInfo = null;

            foreach (ImageCodecInfo item in ImageCodecInfo.GetImageEncoders())
            {
                if (item.MimeType == "image/tiff")
                {
                    imageCodecInfo = item;
                    break;
                }
            }

            if (imageCodecInfo == null)
            {
                Console.WriteLine("Image ImageCodecInfo for tiff file not found!");

                return;
            }

            using EncoderParameters encoderParameters = new EncoderParameters(1)
            {
                Param = { [0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame) }
            };

            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World_Rotate180FlipNone.png");

            image.Save(@"C:\ImageSamples\Hello World.tif", imageCodecInfo, encoderParameters);

            using Bitmap image2 = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World_Rotate180FlipX.png");

            encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);

            image.SaveAdd(image2, encoderParameters);

            using Bitmap image3 = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World_RotateNoneFlipX.png");

            image.SaveAdd(image3, encoderParameters);

            encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);

            image.SaveAdd(encoderParameters);
        }

        private static void ConvertImage()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World.png");

            image.Save(@"C:\ImageSamples\Hello World.jpg", ImageFormat.Jpeg);
        }

        private static void Thumbnail()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World.png");

            using Image imageThumbnail = image.GetThumbnailImage(100, 100, GetThumnailImage, new IntPtr());

            imageThumbnail.Save(@"C:\ImageSamples\Hello World_thumbnail.png");
        }

        private static bool GetThumnailImage()
        {
            return true;
        }

        private static void Grayscale()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World.png");

            //create a blank bitmap the same size as image
            using Bitmap newBitmap = new Bitmap(image.Width, image.Height);

            //get a graphics object from the new image
            using Graphics graphics = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            using ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            Rectangle rectangle = new Rectangle(0, 0, image.Width, image.Height);

            //draw the image image on the new image
            //using the grayscale color matrix
            graphics.DrawImage(image, rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

            // save the new image
            newBitmap.Save(@"C:\ImageSamples\Hello World_grayscale.png");
        }

        private static void WriteText()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World.png");
            using Graphics graphics = Graphics.FromImage(image);
            using Font font = new Font("Arial", 50);
            using SolidBrush solidBrush = new SolidBrush(Color.FromArgb(125, 0, 255, 0));

            //graphics.DrawString("Copy", font, Brushes.Red, 90, 202);
            graphics.DrawString("Copy", font, solidBrush, 90, 202);

            image.Save(@"C:\ImageSamples\Hello World_copyText.png");
        }

        private static void ResizeImage()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World.png");
            using Bitmap newSizeBitmap = new Bitmap(image.Width * 2, image.Height * 2);
            //Bitmap newSizeBitmap = new Bitmap(image.Width / 2, image.Height / 2);
            using Graphics graphics = Graphics.FromImage(newSizeBitmap);

            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, 0, 0, image.Width * 2, image.Height * 2);
            //graphics.DrawImage(image, 0, 0, image.Width / 2, image.Height / 2);

            newSizeBitmap.Save(@"C:\ImageSamples\Hello World_newSize_d2.png");
        }

        private static void CropImage()
        {
            using Bitmap image = (Bitmap)Image.FromFile(@"C:\ImageSamples\Hello World.png");

            // x=81, y=150, w=269, h=131
            Rectangle cropRectangle = new Rectangle(81, 150, 269, 131);
            using Bitmap cropBitmap = image.Clone(cropRectangle, image.PixelFormat);

            cropBitmap.Save(@"C:\ImageSamples\Hello World_crop.png");
        }

        private static void RotateImage()
        {
            using Bitmap image = new Bitmap(@"C:\ImageSamples\Hello World.png");

            int angle = 30;
            double angleRadians = angle * Math.PI / 180d;
            double cos = Math.Abs(Math.Cos(angleRadians));
            double sin = Math.Abs(Math.Sin(angleRadians));
            int newWidth = (int)Math.Round(image.Width * cos + image.Height * sin);
            int newHeight = (int)Math.Round(image.Width * sin + image.Height * cos);

            PointF offset = new PointF(image.Width / 2, image.Height / 2);

            using Bitmap rotateBitmap = new Bitmap(newWidth, newHeight);

            rotateBitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using Graphics graphics = Graphics.FromImage(rotateBitmap);

            graphics.TranslateTransform(newWidth / 2, newHeight / 2);
            graphics.RotateTransform(angle);
            graphics.DrawImage(image, new PointF(-offset.X, -offset.Y));

            rotateBitmap.Save(@"C:\ImageSamples\Hello World_Rotate30.png");
        }
    }
}