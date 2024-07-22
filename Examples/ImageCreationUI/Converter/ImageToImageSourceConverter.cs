using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HPPH;
using HPPH.System.Drawing;

namespace ImageCreationUI.Converter;

[ValueConversion(typeof(IImage), typeof(ImageSource))]
public class ImageToImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        using Bitmap? bitmap = (value as IImage)?.ToBitmap();
        if (bitmap == null) return null;

        using MemoryStream ms = new();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        ms.Position = 0;

        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = ms;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        return bitmapImage;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}