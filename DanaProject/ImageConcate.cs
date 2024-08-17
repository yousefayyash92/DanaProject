using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DanaProject
{
    public class ImageConcate
    {
        public static unsafe void concateVertical(byte* p1, byte* p2, byte* pOut, int outputImgWidth, int img1Width, int img1Height, int img2Width, int img2Height, int nOffset1, int nOffset2, int nOffsetOut)
        {
                for (int y = 0; y < img1Height; ++y)
                {
                  for (int x = 0; x < outputImgWidth; ++x){
                        if (x < img1Width)
                        {
                            pOut[0] = p1[0];
                            pOut[1] = p1[1];
                            pOut[2] = p1[2];
                            p1 += 3;
                        }
                     pOut += 3;
                  }
                  p1 += nOffset1;
                  pOut += nOffsetOut;
                }
                for (int y = 0; y < img2Height; ++y) {
                    for (int x = 0; x < outputImgWidth; ++x){
                        if(x < img2Width) { 
                            pOut[0] = p2[0];
                            pOut[1] = p2[1];
                            pOut[2] = p2[2];
                            p2 += 3;
                        }
                     pOut += 3;
                    }
                   p2 += nOffset2;
                   pOut += nOffsetOut;
                }
         
        }

        public static unsafe void concateHorizontally(byte* p1, byte* p2, byte* pOut, int outputImgHeight, int img1Width, int img2Width, int nOffset1, int nOffset2, int nOffsetOut)
        {        
                for (int y = 0; y < outputImgHeight; ++y)
                {
                    for (int x = 0; x < img1Width; ++x)
                    {
                        pOut[0] = p1[0];
                        pOut[1] = p1[1];
                        pOut[2] = p1[2];
                        p1 += 3;
                        pOut += 3;
                    }
                    for (int x = 0; x < img2Width; ++x)
                    {
                        pOut[0] = p2[0];
                        pOut[1] = p2[1];
                        pOut[2] = p2[2];
                        p2 += 3;
                        pOut += 3;
                    }
                    p2 += nOffset2;
                    p1 += nOffset1;
                    pOut += nOffsetOut;
                }
            
        }

    public static Bitmap concateImages(Bitmap img1, Bitmap img2, bool isHorizontal) {
        int outputImgWidth = isHorizontal? img1.Width+img2.Width: Math.Max(img1.Width, img2.Width);
        int outputImgHeight = isHorizontal ? Math.Max(img1.Height,img2.Height) : img1.Height + img2.Height;
        Bitmap outputImg = new Bitmap(outputImgWidth, outputImgHeight);

        BitmapData bmData1 = img1.LockBits(new Rectangle(0, 0, img1.Width, img1.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmData2 = img2.LockBits(new Rectangle(0, 0, img2.Width, img2.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData bmDataOut = outputImg.LockBits(new Rectangle(0, 0, outputImg.Width, outputImg.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

        int stride1 = bmData1.Stride;
        int stride2 = bmData2.Stride;
        int strideOut = bmDataOut.Stride;

        IntPtr scanOut = bmDataOut.Scan0;

        unsafe
            {
                byte* p1 = (byte*)bmData1.Scan0;
                byte* p2 = (byte*)bmData2.Scan0;
                byte* pOut = (byte*)scanOut;


                int nOffset1 = stride1 - img1.Width * 3;
                int nOffset2 = stride2 - img2.Width * 3;
                int nOffsetOut = strideOut - outputImgWidth * 3;

                if (isHorizontal)
                {
                    concateHorizontally(p1, p2, pOut, outputImgHeight, img1.Width, img2.Width, nOffset1, nOffset2, nOffsetOut);
                }
                else
                {
                    concateVertical(p1, p2, pOut, outputImgWidth, img1.Width, img1.Height, img2.Width, img2.Height, nOffset1, nOffset2, nOffsetOut);
                }
            }
        img1.UnlockBits(bmData1);
        img2.UnlockBits(bmData2);
        outputImg.UnlockBits(bmDataOut);
        return outputImg;
    }
        public static void Main()
        {
            string filePath = @"C:\Users\Youse\source\repos\DanaProject\DanaProject\image.jpg.jpg";
            string filePath2 = @"C:\Users\Youse\source\repos\DanaProject\DanaProject\pexels-ozanculha-20359974.jpg";
            Bitmap bitmap2 = new Bitmap(filePath);
            Bitmap bitmap1 = new Bitmap(filePath2);
            Bitmap img = concateImages(bitmap1, bitmap2, false);
            string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tempImage.bmp");
            img.Save(tempFilePath, ImageFormat.Bmp);
            Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
        }
    }
}
