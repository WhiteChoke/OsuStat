using System.Windows;
using System.Windows.Media;

namespace OsuStat.UI.Styles
{
    public static class ButtonProps
    {
        public static readonly DependencyProperty HoverColorProperty =
            DependencyProperty.RegisterAttached("HoverColor", typeof(Brush), typeof(ButtonProps), new PropertyMetadata(Brushes.Transparent));

        public static void SetHoverColor(DependencyObject obj, Brush value) => obj.SetValue(HoverColorProperty, value);
        public static Brush GetHoverColor(DependencyObject obj) => (Brush)obj.GetValue(HoverColorProperty);
    }

}
