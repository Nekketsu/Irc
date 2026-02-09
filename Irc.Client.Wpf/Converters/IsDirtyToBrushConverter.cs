using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Irc.Client.Wpf.Converters
{
    public class IsDirtyToBrushConverter : Freezable, IValueConverter
    {
        public static readonly DependencyProperty CleanBrushProperty =
            DependencyProperty.Register(nameof(CleanBrush), typeof(Brush), typeof(IsDirtyToBrushConverter));

        public static readonly DependencyProperty DirtyBrushProperty =
            DependencyProperty.Register(nameof(DirtyBrush), typeof(Brush), typeof(IsDirtyToBrushConverter));

        public Brush CleanBrush
        {
            get => (Brush)GetValue(CleanBrushProperty);
            set => SetValue(CleanBrushProperty, value);
        }

        public Brush DirtyBrush
        {
            get => (Brush)GetValue(DirtyBrushProperty);
            set => SetValue(DirtyBrushProperty, value);
        }

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

        protected override Freezable CreateInstanceCore()
        {
            return new IsDirtyToBrushConverter();
        }
    }
}
