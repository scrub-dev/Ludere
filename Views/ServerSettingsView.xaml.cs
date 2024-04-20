using NowPlaying.Core;
using NowPlaying.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NowPlaying.Views
{
    /// <summary>
    /// Interaction logic for ServerSettingsView.xaml
    /// </summary>
    public partial class ServerSettingsView : UserControl
    {

        public ServerSettingsView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Utility.Util.HandleRegexValidationNumbersOnly(sender, e);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            ServerSettingsViewModel vm = (ServerSettingsViewModel)this.DataContext;
            vm.OnTextChanged();
        }
    }
}
