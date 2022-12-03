using App2;
using App2.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.UI.Composition;

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    public sealed partial class HomePage : Page
    {

        public List<Track> trackName;
        public MediaPlayerElement Player;
        public static StackPanel Stackpan;
        public static TextBlock txtTitle;
        public static TextBlock txtAutor;
        public static List<Track> PopularTracks;
        MediaPlayer _mediaPlayer;
        DispatcherTimer dispatcherTimer;

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
            PopularText.Text = "Популярные треки";
            PopularText2.Text = "Все треки";
            DispatcherTimerSetup();
            //_mediaPlayer = new MediaPlayer();
            //_mediaPlayer.MediaEnded += MediaEnded;
            //Player.SetMediaPlayer(_mediaPlayer);

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
                PopularImage.Source = new BitmapImage(new Uri(PopularTracks[0].image));
            }
            if (PopularTitle2.Text != PopularTracks[0].title && PopularTAutor2.Text != PopularTracks[1].author)
            {
                PopularTitle2.Text = PopularTracks[1].title;
                PopularTAutor2.Text = PopularTracks[1].author;
                PopularImage2.Source = new BitmapImage(new Uri(PopularTracks[1].image));
            }
            if (PopularTitle3.Text != PopularTracks[0].title && PopularTAutor3.Text != PopularTracks[2].author)
            {
                PopularTitle3.Text = PopularTracks[2].title;
                PopularTAutor3.Text = PopularTracks[2].author;
                PopularImage3.Source = new BitmapImage(new Uri(PopularTracks[2].image));
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
            foreach (Track track in rec)
            {
                result4 = await new ReqService().Get($"{App2.Constants.URL}favtracks/isfavourite?trackId={track.id}", localValue);
                if (result4 == "1")
                {
                    track.like = "ms-appx:///Views/heart2.png";
                }
                else { track.like = "ms-appx:///Views/hear1.png"; }
                TestView.Items.Add(track);
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

            PopularImage.Source = new BitmapImage(new Uri(PopularTracks[0].image));
            PopularImage2.Source = new BitmapImage(new Uri(PopularTracks[1].image));
            PopularImage3.Source = new BitmapImage(new Uri(PopularTracks[2].image));

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
            // Looking at if the list is anything more than 0 items, they can be removed
            if (TestView.SelectedIndex > -1)
            {
                Player.Source = MediaSource.CreateFromUri(new Uri(trackName[TestView.SelectedIndex].source));


                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                Stackpan.Visibility = Visibility.Visible;
                // load a setting that is local to the device
                Player.MediaPlayer.AudioCategory = MediaPlayerAudioCategory.Media;
                
                String localValue = localSettings.Values["JwtToken"] as string;
                localSettings.Values["LastSource"] = trackName[TestView.SelectedIndex].source;
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
            Image element = (Image)sender;

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
            Player.Source = MediaSource.CreateFromUri(new Uri(PopularTracks[2].source));


            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Stackpan.Visibility = Visibility.Visible;
            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            localSettings.Values["LastSource"] = PopularTracks[2].source;
            localSettings.Values["LastTitle"] = PopularTracks[2].title;
            localSettings.Values["LastAutor"] = PopularTracks[2].author;

            txtTitle.Text = PopularTracks[2].title;
            txtAutor.Text = PopularTracks[2].author;

            await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + PopularTracks[2].id, localValue);
        }

        private async void PopularButton2_Click(object sender, RoutedEventArgs e)
        {
            Player.Source = MediaSource.CreateFromUri(new Uri(PopularTracks[1].source));


            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Stackpan.Visibility = Visibility.Visible;
            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            localSettings.Values["LastSource"] = PopularTracks[1].source;
            localSettings.Values["LastTitle"] = PopularTracks[1].title;
            localSettings.Values["LastAutor"] = PopularTracks[1].author;

            txtTitle.Text = PopularTracks[1].title;
            txtAutor.Text = PopularTracks[1].author;

            await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + PopularTracks[1].id, localValue);
        }

        private async void PopularButton_Click(object sender, RoutedEventArgs e)
        {
            Player.Source = MediaSource.CreateFromUri(new Uri(PopularTracks[0].source));


            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Stackpan.Visibility = Visibility.Visible;
            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            localSettings.Values["LastSource"] = PopularTracks[0].source;
            localSettings.Values["LastTitle"] = PopularTracks[0].title;
            localSettings.Values["LastAutor"] = PopularTracks[0].author;

            txtTitle.Text = PopularTracks[0].title;
            txtAutor.Text = PopularTracks[0].author;

            await new ReqService().Get($"{App2.Constants.URL}Auditions/AddAudition?trackId=" + PopularTracks[0].id, localValue);
        }
    }
}
