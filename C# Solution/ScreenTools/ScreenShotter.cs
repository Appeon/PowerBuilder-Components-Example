using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appeon.ComponentsApp.ScreenTools
{
    public class ScreenShotter
    {
        public static byte[] TakeScreenshot(double scale = 1.0)
        {
            var screenDef = ScreenManager.GetScreens();

            return TakeScreenshot(screenDef.First(), scale);
        }

        public static byte[] TakeScreenshot(int width, int height)
        {
            var screenDef = ScreenManager.GetScreens();
            var first = screenDef.First();

            return TakeScreenshot(first, width, height);
        }

        public static byte[] TakeScreenshot(ScreenDefinition screenDef, int width, int height)
        {
            if (screenDef == default)
            {
                return null;
            }

            var containerProportions = (double)width / height;
            var screenProportions = (double)screenDef.Width / screenDef.Height;
            var proportionDelta = containerProportions - screenProportions;
            double scale;
            if (proportionDelta < 0)
            {
                scale = (double)width / screenDef.Width;
            }
            else
            {
                scale = (double)height / screenDef.Height;
            }


            return TakeScreenshot(screenDef, scale);
        }


        public static byte[] TakeScreenshot(ScreenDefinition screenDef, double scale = 1.0)
        {
            if (screenDef == default)
            {
                return null;
            }

            var conv = new ImageConverter();
            using (Bitmap bitmap = new Bitmap(screenDef.Width, screenDef.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(screenDef.X, screenDef.Y), Point.Empty, new Size(screenDef.Width, screenDef.Height));
                }



                return (byte[])conv.ConvertTo(bitmap, typeof(byte[]));

                //using (Bitmap scaled = new Bitmap(bitmap, new Size((int)(bitmap.Width * scale),
                //    (int)(bitmap.Height * scale))))
                //{
                //    return (byte[])conv.ConvertTo(scaled, typeof(byte[]));
                //}
            }
        }
    }
}
