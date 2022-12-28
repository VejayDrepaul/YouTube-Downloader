using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
using FFMpegCore;
using System.Threading;
//https://www.youtube.com/watch?v=afIJop8PriY
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
            var video = await youtube.Videos.GetAsync(link);
            string title = video.Title;

            string inputStream = "C:\\Users\\drepa\\Projects\\YouTube-Downloader\\bin\\Debug\\net7.0-windows\\Rack Me, Rack Me.mp4";
            string auioFile = "C:\\Users\\drepa\\Projects\\YouTube-Downloader\\bin\\Debug\\net7.0-windows\\Rack Me, Rack Me.webm";
            string outputStream = "C:\\Users\\drepa\\Projects\\YouTube-Downloader\\test\\new.mp4";


            if (audio_radiobutton.IsChecked == true)
            {
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{title}.{streamInfo.Container}");
                MessageBox.Show("Download Complete");
            }
            else if (video_radiobuttton.IsChecked == true)
            {
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
                var streamInfo = streamManifest
                    .GetVideoOnlyStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestVideoQuality();
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{title}.{streamInfo.Container}");

                FFMpeg.ReplaceAudio(inputStream, auioFile, outputStream);

                MessageBox.Show("Download Complete");
            }
        }
    }
}
