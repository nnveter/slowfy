using App2;
using App2.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using static System.Net.Mime.MediaTypeNames;

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    public sealed partial class MusicPage : Page
    {
        public List<Track> trackName;
        public MediaPlayerElement Player;
        public static StackPanel Stackpan;
        public static TextBlock txtTitle;
        public static TextBlock txtAutor;
        public int idTrack;
        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public static int next = 0;
        public MusicPage()
        {
            this.InitializeComponent();
            txtAutor = MainWindow.txtAutor;
            txtTitle = MainWindow.txtTitle;
            Pro();
            Player = MainWindow.pl;
            Stackpan = MainWindow.Stackpan;

            HomePage.dispatcherTimer.Stop();
            Player.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            Player.TransportControls.IsZoomButtonVisible = false;
            Player.TransportControls.IsZoomEnabled = false;
            Player.TransportControls.IsPlaybackRateButtonVisible = false;
            Player.TransportControls.IsNextTrackButtonVisible = true;
            Player.TransportControls.IsPreviousTrackButtonVisible = true;
            Player.TransportControls.IsPlaybackRateEnabled = true;
            Player.TransportControls.IsCompact = true;
            DispatcherTimerSetup2();
        }

        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            next++;
        }

        public void DispatcherTimerSetup2()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick2;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        public async void dispatcherTimer_Tick2(object sender, object e)
        {

            if (MainWindow.Page_ != "XamlBrewer.WinUI3.Navigation.Sample.Views.MusicPage")
            {
                //dispatcherTimer.Stop();
            }
            else
            {
                if (next == 1 && MainWindow.Page_ == "XamlBrewer.WinUI3.Navigation.Sample.Views.MusicPage")
                {
                    if (TestView.SelectedIndex < trackName.Count - 1)
                    {
                        TestView.SelectedItem = TestView.SelectedIndex + 1;
                        TestView.SelectedIndex = TestView.SelectedIndex + 1;
                        next = 0;
                        Player.Source = MediaSource.CreateFromUri(new Uri($"{Constants.URL}file/mp3?mp3={trackName[TestView.SelectedIndex].id}.mp3"));

                        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                        Stackpan.Visibility = Visibility.Visible;

                        String localValue = localSettings.Values["JwtToken"] as string;
                        localSettings.Values["LastSource"] = $"{Constants.URL}file/mp3?mp3={trackName[TestView.SelectedIndex].id}.mp3";
                        localSettings.Values["LastTitle"] = trackName[TestView.SelectedIndex].title;
                        localSettings.Values["LastAutor"] = trackName[TestView.SelectedIndex].author;
                        localSettings.Values["LastId"] = trackName[TestView.SelectedIndex].id;

                        txtTitle.Text = trackName[TestView.SelectedIndex].title;
                        txtAutor.Text = trackName[TestView.SelectedIndex].author;

                        await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + trackName[TestView.SelectedIndex].id, localValue);

                    }
                }
            }

        }

        private async void Pro()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            String localValue = localSettings.Values["JwtToken"] as string;

            string result2 = await new ReqService().Get($"{Constants.URL}favtracks/GetMyFavorite", localValue);

            List<Track> rec =
                JsonSerializer.Deserialize<List<Track>>(result2);
            rec.Reverse();
            trackName = rec;
            string result4;
            idTrack = 1;
            foreach (Track track in rec)
            {
                result4 = await new ReqService().Get($"{App2.Constants.URL}favtracks/isfavourite?trackId={track.id}", localValue);
                if (result4 == "1")
                {
                    track.like = "ms-appx:///Views/heart2.png";
                }
                else { track.like = "ms-appx:///Views/hear1.png"; }
                track.listid = idTrack;
                track.image = $"{Constants.URL}file/mp3?mp3={track.id}.jpg";
                TestView.Items.Add(track);
                idTrack++;
            }
            //trackName = await new ReqService().GetTracks();
            //TestView.Items.Add(trackName[0].id.ToString());

            String Name = localSettings.Values["Name"] as string;
            NamePlayList.Text = "Плейлист";
            FolowTracksText.Text = "Любимые треки";
            Username.Text = Name;
            CountTracks.Text = $"Всего треков: {rec.Count}";
        }

        private void EditTask_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // I need to work on this later :)
            while (TestView.SelectedIndex > -1)
            {
                // Function to remove items
                //TestView.Items.RemoveAt(TestView.SelectedIndex);
            }
        }

        // Event handler for "Add" button on Task page
        private async void AddTask_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Sets content dialog
            ContentDialog dialog = new CreateTaskDialog();
            dialog.XamlRoot = this.XamlRoot;
            // Stores result for use in statement
            var result = await dialog.ShowAsync();

            // Statement to manage state detection and string handler
            if (result == ContentDialogResult.Primary)
            {
                string addNewTask = (string)dialog.Tag;
                TestView.Items.Add(addNewTask);
            }
        }

        // Handles removal of items in the List.
        private async void TestView_SelectionChanged(object sender, SelectionChangedEventArgs e) // Event handler
        {
            // Looking at if the list is anything more than 0 items, they can be removed
            if (TestView.SelectedIndex > -1)
            {
                Player.Source = MediaSource.CreateFromUri(new Uri($"{Constants.URL}file/mp3?mp3={trackName[TestView.SelectedIndex].id}.mp3"));


                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                Stackpan.Visibility = Visibility.Visible;
                // load a setting that is local to the device
                String localValue = localSettings.Values["JwtToken"] as string;
                localSettings.Values["LastSource"] = $"{Constants.URL}file/mp3?mp3={trackName[TestView.SelectedIndex].id}.mp3";
                localSettings.Values["LastTitle"] = trackName[TestView.SelectedIndex].title;
                localSettings.Values["LastAutor"] = trackName[TestView.SelectedIndex].author;

                txtTitle.Text = trackName[TestView.SelectedIndex].title;
                txtAutor.Text = trackName[TestView.SelectedIndex].author;

                await new ReqService().Get($"{Constants.URL}Auditions/AddAudition?trackId=" + trackName[TestView.SelectedIndex].id, localValue);
                //trackName = await new ReqService().GetTracks();
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {


        }

        private void TestView_ItemClick(object sender, ItemClickEventArgs e)
        {




        }

        private async void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Microsoft.UI.Xaml.Controls.Image element = (Microsoft.UI.Xaml.Controls.Image)sender;

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String localValue2 = localSettings.Values["JwtToken"] as string;

            String result = await new ReqService().Get($"{App2.Constants.URL}favtracks/isfavourite?trackId={(int)element.Tag}", localValue2);
            if (result == "1")
            {
                element.Source = new BitmapImage(new Uri("ms-appx:///Views/hear1.png"));
                //element.Symbol = Symbol.SolidStar;
                //element.Visibility = Visibility.Collapsed;
                await new ReqService().Get($"{App2.Constants.URL}FavTracks/RemoveFromFavourites?trackId={(int)element.Tag}", localValue2);
            }
            else
            {
                element.Source = new BitmapImage(new Uri("ms-appx:///Views/heart2.png"));
                //element.Symbol = Symbol.SolidStar;
                //element.Visibility = Visibility.Collapsed;
                await new ReqService().Get($"{App2.Constants.URL}FavTracks/AddToFavourite?trackId={(int)element.Tag}", localValue2);
            }

        }

        private async void but_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;


            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            await new ReqService().Get($"{Constants.URL}FavTracks/AddToFavourite?trackId=" + trackName[TestView.SelectedIndex].id, localValue);
            // Button a = (Button)TestView.SelectedItem;
            // a.Content = localValue;
        }
    }
}
