using System.Drawing;
using System.Drawing.Imaging;

namespace WinBoostHotkeys
{
    public static class IconHelper
    {
        /// <summary>
        /// Creates a colored tray icon (16x16)
        /// </summary>
        public static Icon CreateColoredIcon(Color color)
        {
            using var bitmap = new Bitmap(16, 16);
            using var graphics = Graphics.FromImage(bitmap);
            
            // Draw filled circle
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.FillEllipse(new SolidBrush(color), 2, 2, 12, 12);
            graphics.DrawEllipse(new Pen(Color.Black, 1), 2, 2, 12, 12);
            
            // Convert to icon
            IntPtr hIcon = bitmap.GetHicon();
            return Icon.FromHandle(hIcon);
        }

        public static Icon CreateGreenIcon() => CreateColoredIcon(Color.FromArgb(76, 220, 96));
        public static Icon CreateBlueIcon() => CreateColoredIcon(Color.FromArgb(0, 168, 255));
    }
}
