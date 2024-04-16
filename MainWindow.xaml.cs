using NowPlaying.Utility;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NowPlaying
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly Dictionary<String, Type> frames = [];
        private Type? _currentlyVisibleFrame;

        public MainWindow()
        {
            InitializeComponent();

            frames.Add("HOME",            typeof(Views.HomeView));
            frames.Add("SPOTIFYSETTINGS", typeof(Views.SpotifySettingsView));
            frames.Add("SERVERSETTINGS",  typeof(Views.ServerSettingsView));
            frames.Add("STYLESETTINGS",   typeof(Views.StyleSettingsView));
            frames.Add("CUSTOMSTYLE",     typeof(Views.CustomStyleView));

            SetFrameContent(FrameMain, typeof(Views.HomeView));
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            Util.OpenUri(Properties.Settings.Default.HelpURI);
        }
        private Type GetFrame (string frameString)
        {
            return frames[frameString.Split("_")[1].ToUpper()];
        }
        private void SetFrameContent (Frame rootFrame, Type subframe)
        {
            if (_currentlyVisibleFrame == subframe.GetType()) return;
            if (rootFrame.NavigationService.CanGoBack) rootFrame.NavigationService.RemoveBackEntry();

            UserControl FrameToNavigateTo = (UserControl)Activator.CreateInstance(subframe)! ?? new Views.HomeView();
            FrameMain.Navigate(FrameToNavigateTo);
            _currentlyVisibleFrame = FrameToNavigateTo.GetType();
        }

        private void BtnNav_Click(object sender, RoutedEventArgs e)
        {
            if(!sender.GetType().Equals(typeof(Button))) return;

            Type FrameToShow = GetFrame(((Button)sender).Name);
            SetFrameContent(FrameMain, FrameToShow);
        }
        private void BtnMinimise_Click(object sender, RoutedEventArgs e)
        {
            if (!sender.GetType().Equals(typeof(Button))) return;
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}