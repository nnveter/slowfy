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
    public sealed partial class HomePage : Page
    {

        public List<Track> trackName;
        public int idTrack;
        public MediaPlayerElement Player;
        public static StackPanel Stackpan;
        public static TextBlock txtTitle;
        public static TextBlock txtAutor;
        public static List<Track> PopularTracks;
        public static DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public static int next = 0;

        public HomePage()
        {
            this.InitializeComponent();
            txtAutor = MainWindow.txtAutor;
            txtTitle = MainWindow.txtTitle;
            Pro();
            Player = MainWindow.pl;
            Stackpan = MainWindow.Stackpan;

            //TestView.Items.Add(trackName[1].title);
            Player.TransportControls.IsZoomButtonVisible = false;
            Player.TransportControls.IsZoomEnabled = false;
            Player.TransportControls.IsPlaybackRateButtonVisible = false;
            Player.TransportControls.IsNextTrackButtonVisible = true;
            Player.TransportControls.IsPreviousTrackButtonVisible = true;
            Player.TransportControls.IsPlaybackRateEnabled = true;
            Player.TransportControls.IsCompact = true;
            Player.TransportControls.IsRepeatButtonVisible = true;
            Player.TransportControls.IsRepeatEnabled = true;
            PopularText.Text = "Популярные треки";
            PopularText2.Text = "Все треки";
            PopularText.Visibility = Visibility.Collapsed;
            PopularText2.Visibility = Visibility.Collapsed;
            Player.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            DispatcherTimerSetup();
            DispatcherTimerSetup2();
            //_mediaPlayer = new MediaPlayer();
            //_mediaPlayer.MediaEnded += MediaEnded;
            //Player.SetMediaPlayer(_mediaPlayer);

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
            if (next == 1 && MainWindow.Page_ == "XamlBrewer.WinUI3.Navigation.Sample.Views.HomePage") {
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


        //private async void MediaEnded(MediaPlayer sender, object args)
        //{
        //    txtAutor.Text = "1";


        //}

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
            dispatcherTimer.Start();
        }
        public async void dispatcherTimer_Tick(object sender, object e)
        {
            string result3 = await new ReqService().Get($"{App2.Constants.URL}tracks/getmostpopulartracks?count=3");
            PopularTracks =
                JsonSerializer.Deserialize<List<Track>>(result3);
            if (PopularTitle.Text != PopularTracks[0].title && PopularTAutor.Text != PopularTracks[0].author)
            {
                PopularTitle.Text = PopularTracks[0].title;
                PopularTAutor.Text = PopularTracks[0].author;
                PopularImage.Source = new BitmapImage(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[0].id}.jpg"));
            }
            if (PopularTitle2.Text != PopularTracks[1].title && PopularTAutor2.Text != PopularTracks[1].author)
            {
                PopularTitle2.Text = PopularTracks[1].title;
                PopularTAutor2.Text = PopularTracks[1].author;
                PopularImage2.Source = new BitmapImage(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[1].id}.jpg"));
            }
            if (PopularTitle3.Text != PopularTracks[2].title && PopularTAutor3.Text != PopularTracks[2].author)
            {
                PopularTitle3.Text = PopularTracks[2].title;
                PopularTAutor3.Text = PopularTracks[2].author;
                PopularImage3.Source = new BitmapImage(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[2].id}.jpg"));
            }

        }



        private async void Pro()
        {
            string result2 = await new ReqService().Get($"{App2.Constants.URL}tracks");
            string result3 = await new ReqService().Get($"{App2.Constants.URL}tracks/getmostpopulartracks?count=3");

            List<Track> rec =
                JsonSerializer.Deserialize<List<Track>>(result2);
            PopularTracks =
                JsonSerializer.Deserialize<List<Track>>(result3);
            //rec.Reverse();
            trackName = rec;
            string result4;
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            idTrack = 1;
            foreach (Track track in rec)
            {
                result4 = await new ReqService().Get($"{App2.Constants.URL}favtracks/isfavourite?trackId={track.id}", localValue);
                if (result4 == "1")
                {
                    track.like = "ms-appx:///Views/heart2.png";
                }
                else { track.like = "ms-appx:///Views/hear1.png"; }
                track.image = $"{Constants.URL}file/mp3?mp3={track.id}.jpg";
                track.listid = idTrack;
                TestView.Items.Add(track);
                idTrack++;
            }
            Border1.Visibility = Visibility.Visible;
            PopularTitle.Text = PopularTracks[0].title;
            PopularTAutor.Text = PopularTracks[0].author;
            Border2.Visibility = Visibility.Visible;
            PopularTitle2.Text = PopularTracks[1].title;
            PopularTAutor2.Text = PopularTracks[1].author;
            Border3.Visibility = Visibility.Visible;
            PopularTitle3.Text = PopularTracks[2].title;
            PopularTAutor3.Text = PopularTracks[2].author;

            PopularImage.Source = new BitmapImage(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[0].id}.jpg"));
            PopularImage2.Source = new BitmapImage(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[1].id}.jpg"));
            PopularImage3.Source = new BitmapImage(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[2].id}.jpg"));

            PopularText.Visibility = Visibility.Visible;
            PopularText2.Visibility = Visibility.Visible;

            //trackName = await new ReqService().GetTracks();
            //TestView.Items.Add(trackName[0].id.ToString());
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
            if (MainWindow.Page_ == "XamlBrewer.WinUI3.Navigation.Sample.Views.HomePage")
            {
                //dispatcherTimer.Stop();
                //dispatcherTimer.Start();
            }
            // Looking at if the list is anything more than 0 items, they can be removed
            if (TestView.SelectedIndex > -1)
            {
                Player.Source = MediaSource.CreateFromUri(new Uri($"{Constants.URL}file/mp3?mp3={trackName[TestView.SelectedIndex].id}.mp3"));


                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                Stackpan.Visibility = Visibility.Visible;
                // load a setting that is local to the device
                Player.MediaPlayer.AudioCategory = MediaPlayerAudioCategory.Media;

                String localValue = localSettings.Values["JwtToken"] as string;
                localSettings.Values["LastSource"] = $"{Constants.URL}file/mp3?mp3={trackName[TestView.SelectedIndex].id}.mp3";
                localSettings.Values["LastTitle"] = trackName[TestView.SelectedIndex].title;
                localSettings.Values["LastAutor"] = trackName[TestView.SelectedIndex].author;
                localSettings.Values["LastId"] = trackName[TestView.SelectedIndex].id;

                txtTitle.Text = trackName[TestView.SelectedIndex].title;
                txtAutor.Text = trackName[TestView.SelectedIndex].author;

                await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + trackName[TestView.SelectedIndex].id, localValue);
                //trackName = await new ReqService().GetTracks();
            }
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


        private async void PopularButton3_Click(object sender, RoutedEventArgs e)
        {
            Player.Source = MediaSource.CreateFromUri(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[2].id}.mp3"));


            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Stackpan.Visibility = Visibility.Visible;
            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            localSettings.Values["LastSource"] = $"{Constants.URL}file/mp3?mp3={PopularTracks[2].id}.mp3";
            localSettings.Values["LastTitle"] = PopularTracks[2].title;
            localSettings.Values["LastAutor"] = PopularTracks[2].author;

            txtTitle.Text = PopularTracks[2].title;
            txtAutor.Text = PopularTracks[2].author;

            await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + PopularTracks[2].id, localValue);
        }

        private async void PopularButton2_Click(object sender, RoutedEventArgs e)
        {
            Player.Source = MediaSource.CreateFromUri(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[1].id}.mp3"));


            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Stackpan.Visibility = Visibility.Visible;
            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            localSettings.Values["LastSource"] = $"{Constants.URL}file/mp3?mp3={PopularTracks[1].id}.mp3";
            localSettings.Values["LastTitle"] = PopularTracks[1].title;
            localSettings.Values["LastAutor"] = PopularTracks[1].author;

            txtTitle.Text = PopularTracks[1].title;
            txtAutor.Text = PopularTracks[1].author;

            await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + PopularTracks[1].id, localValue);
        }

        private async void PopularButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Source = MediaSource.CreateFromUri(new Uri($"{Constants.URL}file/mp3?mp3={PopularTracks[0].id}.mp3"));


            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Stackpan.Visibility = Visibility.Visible;
            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            localSettings.Values["LastSource"] = $"{Constants.URL}file/mp3?mp3={PopularTracks[0].id}.mp3";
            localSettings.Values["LastTitle"] = PopularTracks[0].title;
            localSettings.Values["LastAutor"] = PopularTracks[0].author;

            txtTitle.Text = PopularTracks[0].title;
            txtAutor.Text = PopularTracks[0].author;

            await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + PopularTracks[0].id, localValue);
        }
    }
}
