using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DanaProject
{
    public class ImageProcessing
    {
        public static unsafe void convertBitmapToGrayScale(byte* p, int nOffset, int width, int height)
        {
                byte red, green, blue;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);

                        p += 3;
                    }

                    p += nOffset;
                }
        }


        public static unsafe void convertBitmapToBlackAndWhiteWithStaticThreshold(byte* p, int nOffset, int width, int height, int threshold)
        {
                byte red;

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        red = p[2];

                        p[0] = p[1] = p[2] = red < threshold ? (byte)0 : (byte)255;

                        p += 3;
                    }

                    p += nOffset;
                }
        }

        public static unsafe int calculateMean(byte* p, int nOffset, int width, int height)
        {
            int sum = 0;
            int numOfPixels = width * height;
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        sum += p[2];
                        p += 3;
                    }

                    p += nOffset;
                }
            return sum / numOfPixels;
        }


        public static void convertToBlackAndWhite(Bitmap img, bool useMean = true, int threshold = 128)
        {

            BitmapData bmData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - img.Width * 3;
                int converstionThreshold = useMean ? calculateMean(p, nOffset, img.Width, img.Height) : threshold;
                convertBitmapToGrayScale(p, nOffset, img.Width, img.Height);
                convertBitmapToBlackAndWhiteWithStaticThreshold(p, nOffset, img.Width, img.Height, converstionThreshold);
            }
           
            img.UnlockBits(bmData);
            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tempImage.bmp");
            img.Save(tempFilePath, ImageFormat.Bmp);
            Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
        }

        public static void Main()
        {
            string filePath = @"C:\Users\Youse\source\repos\DanaProject\DanaProject\image.jpg.jpg";
            Bitmap bitmap = new Bitmap(filePath);
            convertToBlackAndWhite(bitmap, false, 200);
        }

    }
}
