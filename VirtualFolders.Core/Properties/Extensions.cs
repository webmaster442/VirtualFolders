namespace VirtualFolders.Core.Properties;

internal static class Extensions
{
    public static string Format(this string str, params object[] args)
    {
        return string.Format(str, args);
    }

    public static string Format(this string str, Exception ex)
    {
        return string.Format(str, ex.Message);
    }
}
