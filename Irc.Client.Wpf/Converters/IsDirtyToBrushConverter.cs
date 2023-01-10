using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Irc.Client.Wpf.Converters
{
    public class IsDirtyToBrushConverter : IValueConverter
    {
        public Brush CleanBrush { get; set; }
        public Brush DirtyBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDirty)
            {
                return isDirty ? DirtyBrush : CleanBrush;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
