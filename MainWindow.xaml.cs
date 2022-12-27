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
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YouTube_Downloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void download_button_Click(object sender, RoutedEventArgs e)
        {
            string link = url.Text;
            var youtube = new YoutubeClient();
            var audio = await youtube.Videos.GetAsync(link);
            string title = audio.Title;


            if (audio_radiobutton.IsChecked == true)
            {
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"test/{title}.{streamInfo.Container}");
            }

            MessageBox.Show("Download Complete");
        }
    }
}
