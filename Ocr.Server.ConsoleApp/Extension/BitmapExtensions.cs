using System;
using System.Drawing;

namespace Ocr.Server.ConsoleApp.Extension
{
    public static class BitmapExtensions
    {
        internal static void ToBlackWhite(this Bitmap bitmap, byte convertGrade)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    bitmap.SetPixel(i, j, bitmap.GetPixel(i, j).R >= convertGrade ? Color.Black : Color.White);
                }
            }
        }

        internal static void ToBlackWhite(this Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var pixelColor = bitmap.GetPixel(i, j);
                    if (pixelColor.R > 200 && pixelColor.G > 200 && pixelColor.B > 200)
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.White);
                    }
                }
            }
        }

        internal static void ToGrayScale(this Bitmap bitmap)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                    bitmap.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
        }
    }
}
