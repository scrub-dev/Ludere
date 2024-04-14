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

        private Dictionary<String, UserControl> frames = [];

        public MainWindow()
        {
            InitializeComponent();

            frames.Add("HOME", new Views.HomeView());
            frames.Add("SPOTIFYSETTINGS", new Views.SpotifySettingsView());
            frames.Add("SERVERSETTINGS", new Views.ServerSettingsView());
            frames.Add("STYLESETTINGS", new Views.StyleSettingsView());
            frames.Add("CUSTOMSTYLE", new Views.CustomStyleView());

            FrameMain.Content = frames["HOME"];
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            Util.OpenUri(Properties.Settings.Default.HelpURI);
        }

        private void BtnNav_Click(object sender, RoutedEventArgs e)
        {
            if(!sender.GetType().Equals(typeof(System.Windows.Controls.Button))){
                return;
            }
            string nameString = (((Button)sender).Name.Split("_")[1]).ToUpper();

            if (nameString is null) return;

            UserControl clickedFrame = frames[nameString];

            FrameMain.Navigate(clickedFrame);
        }
    }
}