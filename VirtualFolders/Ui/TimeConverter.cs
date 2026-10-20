using System.Globalization;

namespace VirtualFolders.Ui;

internal sealed class TimeConverter : OneWayConverter<DateTime, string>
{
    protected override string Convert(DateTime sourceValue, object parameter, CultureInfo culture)
    {
        TimeSpan diff = DateTime.Now - sourceValue;
        if (diff.TotalSeconds < 60)
        {
            return $"{(int)diff.TotalSeconds} seconds ago";

        }
        else if (diff.TotalMinutes < 60)
        {
            return $"{(int)diff.TotalMinutes} minutes ago";
        }
        else if (diff.TotalHours < 24)
        {
            return $"{(int)diff.TotalHours} hours ago";
        }
        else
        {
            return $"{(int)diff.TotalDays} days ago";
        }
    }
}