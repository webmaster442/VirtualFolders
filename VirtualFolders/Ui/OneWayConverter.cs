using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace VirtualFolders.Ui;

internal abstract class OneWayConverter<TSource, TTarget> : MarkupExtension, IValueConverter
    where TTarget: notnull
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TSource sourceValue)
        {
            Debug.WriteLine($"{GetType()}: value is not of type {typeof(TSource)}");
            return Binding.DoNothing;
        }

        if (targetType != typeof(TTarget))
        {
            Debug.WriteLine($"{GetType()}: targetType is not of type {typeof(TTarget)}, but {targetType}");
            return Binding.DoNothing;
        }

        return Convert(sourceValue, parameter, culture);
    }

    protected abstract TTarget Convert(TSource sourceValue, object parameter, CultureInfo culture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
