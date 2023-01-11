using System.Drawing;
using System.Drawing.Drawing2D;

namespace PictureToStudio
{
    public class PictureResizer
    {
        public PictureResizer() { }

        public Bitmap BicubicInterpolation(Bitmap img, float scaleFactor)
        {
            int width = (int)(img.Width * scaleFactor);
            int height = (int)(img.Height * scaleFactor);
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.Bicubic;
                g.DrawImage(img, 0, 0, width, height);
            }
            return result;
        }
    }
}
