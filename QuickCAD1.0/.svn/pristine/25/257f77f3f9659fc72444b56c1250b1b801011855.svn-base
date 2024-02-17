using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace IsolatorTester.Tools.Converter
{
    public class BitmapImageToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BitmapImage bitmapImage)
            {
                DrawingGroup drawingGroup = new DrawingGroup();
                using (DrawingContext drawingContext = drawingGroup.Open())
                {
                    drawingContext.DrawImage(bitmapImage, new Rect(0, 0, bitmapImage.PixelWidth, bitmapImage.PixelHeight));
                }

                Geometry geometry = new GeometryDrawing(new ImageBrush(bitmapImage), null, new RectangleGeometry(new Rect(0, 0, bitmapImage.PixelWidth, bitmapImage.PixelHeight))).Geometry;

                return geometry;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
