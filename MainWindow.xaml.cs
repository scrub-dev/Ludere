using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace NowPlaying
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();
        private void BtnMinimise_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}