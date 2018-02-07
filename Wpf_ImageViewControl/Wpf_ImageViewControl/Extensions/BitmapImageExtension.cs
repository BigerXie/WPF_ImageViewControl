using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Wpf_ImageViewControl.Extensions
{
    public static class BitmapImageExtension
    {
        public static BitmapImage ConvertToDecodeBitmap(this Stream stream, int? decodePixelWidth = null)
        {
            try
            {
                if (stream == null)
                    return null;
                stream.Seek(0, SeekOrigin.Begin);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                //降低图片分辨率 减小内存占用
                if (decodePixelWidth.HasValue)
                    bitmap.DecodePixelWidth = decodePixelWidth.Value;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
