using System.Text;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public static class IMyTextPanelExtensions
    {
        public static int MaxLines(this IMyTextSurface panel)
        {
            var scale = panel.FontSize;
            var font = panel.Font;

            var lineSize = panel.MeasureStringInPixels(new StringBuilder("X"), font, scale);
            var lineHeight = lineSize.Y;

            return (int)(panel.SurfaceSize.Y / lineHeight);
        }

        public static string FormatTitle(this IMyTextSurface panel, string title, char separator = '-')
        {
            var scale = panel.FontSize;
            var font = panel.Font;

            var sb = new StringBuilder(title);
            var size = panel.MeasureStringInPixels(sb, font, scale);
            var maxWidth = panel.SurfaceSize.X;

            while (size.X < maxWidth)
            {
                sb.Append(separator);
                size = panel.MeasureStringInPixels(sb, font, scale);
            }

            return sb.ToString();
        }
    }
}