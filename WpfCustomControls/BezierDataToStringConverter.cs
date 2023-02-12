using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfCustomControls
{
    [ValueConversion(typeof(BezierSegmentData), typeof(string))]
    public class BezierDataToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not BezierSegmentData data)
                throw new NotImplementedException($"Реализовано только для \"{typeof(BezierSegmentData).FullName}\".");
            Vector offset = new Vector(data.StartPoint.X, data.StartPoint.Y);

            string p1 = (data.Point1 - offset).ToString(culture);
            string p2 = (data.Point2 - offset).ToString(culture);
            string end = (data.FinishPoint - offset).ToString(culture);

            return $"M0,0 C {p1} {p2} {end}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BezierDataToStringConverter() { }

        public static BezierDataToStringConverter Instance { get; } = new();

    }
    [MarkupExtensionReturnType(typeof(BezierDataToStringConverter))]
    public class BezierDataToStringExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
            => BezierDataToStringConverter.Instance;
    }
}
