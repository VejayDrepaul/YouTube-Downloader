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
using System.Threading;
using System.Drawing;
using System.Windows.Media.Animation;
using AngleSharp.Text;
using System.Windows.Markup;
using Microsoft.Win32;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using FFMpegCore;
using System.Text.RegularExpressions;
using FFMpegCore.Enums;

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
                MessageBox.Show("YOU MUST ENETER A LINK TO A YOUTUBE VIDEO", "ATTENTION!", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else if (AudioRadioButton.IsChecked != true && VideoRadioButton.IsChecked != true && BothRadioButton.IsChecked != true)
            {
                MessageBox.Show("YOU MUST SELECT WHAT YOU WANT TO DOWNLOAD: VIDEO, AUDIO, OR BOTH", "ATTENTION!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void VideoInformation(object sender, string url)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);

            VideoTitle.Content = $"Title: {video.Title}";
            VideoAuthor.Content = $"Author: {video.Author}";
            VideoDuration.Content = $"Duration: {video.Duration}";

        }

        private async void DownloadAudio(string url, string path)
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
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{path}\\{video_title}.{streamInfo.Container}");

            await FFMpegArguments
                .FromFileInput($"{path}\\{video_title}.{streamInfo.Container}")
                .OutputToFile($"{path}\\{video_title}.mp3", false, options => options
                    .WithAudioCodec(AudioCodec.LibMp3Lame))
                .ProcessAsynchronously();

            if (AudioRadioButton.IsChecked == true)
            {
                File.Delete($"{path}\\{video_title}.{streamInfo.Container}");
            }
        }

        private async void DownloadVideoAndAudio(object sender, string url, string path)
        {
            DownloadAudio(url, path);

            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(url);
            string video_title = video.Title;
            var test = video.Engagement;

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
            var streamInfo = streamManifest
                .GetVideoOnlyStreams()
                .Where(s => s.Container == Container.Mp4)
                .GetWithHighestVideoQuality();
            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{path}\\{video_title}.{streamInfo.Container}");

            FFMpeg.ReplaceAudio($"{path}\\{video_title}.mp4 ", $"{path}\\{video_title}.webm", $"{path}\\conv.mp4");

            if (BothRadioButton.IsChecked != true)
            {
                File.Delete($"{path}\\{video_title}.webm");
                File.Delete($"{path}\\{video_title}.mp4");
            }

            MessageBox.Show("Download Complete", "ATTENTION!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorChecks(true);
            VideoInformation(true, url.Text);

            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();          

            if (dialog.ShowDialog(this).GetValueOrDefault())  
            {
                string FilePath = dialog.SelectedPath;

                if (AudioRadioButton.IsChecked == true)
                {
                    DownloadAudio(url.Text, FilePath);
                    MessageBox.Show("Download Complete", "ATTENTION!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (VideoRadioButton.IsChecked == true)
                {
                    DownloadVideoAndAudio(true, url.Text, FilePath);
                }
                else if (BothRadioButton.IsChecked == true)
                {
                    DownloadVideoAndAudio(true, url.Text, FilePath);
                }
            }
        }
    }
}
