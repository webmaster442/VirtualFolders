using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VirtualFolders.Ui;

internal sealed class FileIconConverter : OneWayConverter<string, ImageSource>
{
    private readonly ConcurrentDictionary<string, ImageSource> _cache;

    public FileIconConverter()
    {
        _cache = new ConcurrentDictionary<string, ImageSource>();
    }

    protected override ImageSource Convert(string sourceValue, object parameter, CultureInfo culture)
    {
        var extension = System.IO.Path.GetExtension(sourceValue);
        if (string.Equals(extension, ".exe", StringComparison.OrdinalIgnoreCase))
        {
            var name = System.IO.Path.GetFileName(sourceValue);
            if (_cache.TryGetValue(sourceValue, out ImageSource? image))
            {
                return image;
            }
            else
            {
                var extractedImage = GetIcon(sourceValue, false);
                _cache.TryAdd(sourceValue, extractedImage);
                return extractedImage;
            }
        }
        else
        {
            if (_cache.TryGetValue(extension, out ImageSource? image))
            {
                return image;
            }
            else
            {
                var extracted = GetIcon(sourceValue, false);
                _cache.TryAdd(extension, extracted);
                return extracted;
            }
        }
    }

    private static ImageSource GetIcon(string strPath, bool bSmall)
    {
        Native.SHFILEINFO info = new Native.SHFILEINFO(true);
        int cbFileInfo = Marshal.SizeOf(info);
        Native.SHGFI flags;
        if (bSmall)
            flags = Native.SHGFI.Icon | Native.SHGFI.SmallIcon | Native.SHGFI.UseFileAttributes;
        else
            flags = Native.SHGFI.Icon | Native.SHGFI.LargeIcon | Native.SHGFI.UseFileAttributes;

        Native.SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);

        IntPtr iconHandle = info.hIcon;
        //if (IntPtr.Zero == iconHandle) // not needed, always return icon (blank)
        //  return DefaultImgSrc;

        ImageSource img = Imaging.CreateBitmapSourceFromHIcon(
                    iconHandle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

        if (img.CanFreeze)
            img.Freeze();


        Native.DestroyIcon(iconHandle);
        return img;
    }
}
