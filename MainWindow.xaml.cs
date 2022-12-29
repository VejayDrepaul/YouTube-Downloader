using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.IO;
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
using System.Drawing;
using System.Windows.Media.Animation;
using AngleSharp.Text;
//  https://www.youtube.com/watch?v=YgZjL1Go4uE&list=RDGMEMCMFH2exzjBeE_zAHHJOdxg&start_radio=1&rv=bwW7ni0aTOE
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
        private void ErrorChecks(object sender)
        {
            if (url.Text == "")
            {
                MessageBox.Show("YOU MUST ENETER A LINK TO A YOUTUBE VIDEO", "ATTENTION!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (audio_radiobutton.IsChecked != true && video_radiobuttton.IsChecked != true && both_radiobutton.IsChecked != true)
            {
                MessageBox.Show("YOU MUST SELECT WHAT YOU WANT TO DOWNLOAD: VIDEO, AUDIO, OR BOTH", "ATTENTION!", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }
        }

        private static async void DownloadAudio(string url)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);
            string video_title = video.Title;

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest
                .GetAudioOnlyStreams()
                .Where(s => s.Container == Container.WebM)
                .GetWithHighestBitrate();
            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{video_title}.{streamInfo.Container}");
        }

        private async void DownloadVideoAndAudio(object sender, string url)
        {
            DownloadAudio(url);

            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);
            string video_title = video.Title;

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest
                .GetVideoOnlyStreams()
                .Where(s => s.Container == Container.Mp4)
                .GetWithHighestVideoQuality();
            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{video_title}.{streamInfo.Container}");

            FFMpeg.ReplaceAudio($"{video_title}.{streamInfo.Container}", $"{video_title}.webm", "conv.mp4");

            if (both_radiobutton.IsChecked != true)
            {
                File.Delete($"{video_title}.webm");
                File.Delete($"{video_title}.mp4");
            }

            MessageBox.Show("Download Complete");
        }

        private void download_button_Click(object sender, RoutedEventArgs e)
        {
            ErrorChecks(true);

            if (audio_radiobutton.IsChecked == true)
            {
                DownloadAudio(url.Text);
                MessageBox.Show("Download Complete");
            }
            else if (video_radiobuttton.IsChecked == true)
            {
                DownloadVideoAndAudio(true, url.Text);
            }
            else if (both_radiobutton.IsChecked == true)
            {
                DownloadVideoAndAudio(true, url.Text);
            }
        }
    }
}
