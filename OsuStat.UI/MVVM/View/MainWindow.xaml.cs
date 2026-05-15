using System.Windows;
using System.Windows.Input;

namespace OsuStat.UI.MVVM.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMinimized_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
    }
}