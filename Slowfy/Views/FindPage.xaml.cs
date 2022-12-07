// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
using Windows.Storage;
using static System.Net.Mime.MediaTypeNames;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FindPage : Page
    {
        public List<Track> trackName;
        public int idTrack;
        public MediaPlayerElement Player;
        public static StackPanel Stackpan;
        public static TextBlock txtTitle;
        public static TextBlock txtAutor;
        DispatcherTimer dispatcherTimer;
        public static int next = 0;
        public FindPage()
        {
            this.InitializeComponent();
            txtAutor = MainWindow.txtAutor;
            txtTitle = MainWindow.txtTitle;
            Player = MainWindow.pl;
            Stackpan = MainWindow.Stackpan;
            Player.TransportControls.IsZoomButtonVisible = false;
            Player.TransportControls.IsZoomEnabled = false;
            Player.TransportControls.IsPlaybackRateButtonVisible = false;
            Player.TransportControls.IsNextTrackButtonVisible = true;
            Player.TransportControls.IsPreviousTrackButtonVisible = true;
            Player.TransportControls.IsPlaybackRateEnabled = true;
            Player.TransportControls.IsCompact = true;
            Pro();
            Find.PlaceholderText = "¬ведите название трека";
            Player.MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            DispatcherTimerSetup2();
        }

        private void MediaPlayer_MediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            next++;
        }

        public void DispatcherTimerSetup2()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick2;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        public async void dispatcherTimer_Tick2(object sender, object e)
        {
            if (MainWindow.Page_ != "XamlBrewer.WinUI3.Navigation.Sample.Views.FindPage")
            {
                //dispatcherTimer.Stop();
            }
            else
            {
                if (next == 1 && MainWindow.Page_ == "XamlBrewer.WinUI3.Navigation.Sample.Views.FindPage")
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

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            TestView.Items.Clear();

            if (Find.Text != null && !String.IsNullOrWhiteSpace(Find.Text) && !String.IsNullOrEmpty(Find.Text))
            {
                var result = await new ReqService().Get($"{Constants.URL}tracks/search?q={Find.Text}&count=10");
                List<Track> rec =
                    JsonSerializer.Deserialize<List<Track>>(result);
                //rec.Reverse();
                trackName = rec;
                idTrack = 1;
                foreach (Track track in rec)
                {
                    track.listid = idTrack;
                    track.image = $"{Constants.URL}file/mp3?mp3={track.id}.jpg";
                    TestView.Items.Add(track);
                    idTrack++;
                }
            }
        }

        private async void Pro()
        {
            string result2 = await new ReqService().Get($"{App2.Constants.URL}tracks/getmostpopulartracks?count=10");

            List<Track> rec =
                JsonSerializer.Deserialize<List<Track>>(result2);
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
                track.listid = idTrack;
                track.image = $"{Constants.URL}file/mp3?mp3={track.id}.jpg";
                TestView.Items.Add(track);
                idTrack++;
            }

        }
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
    }
}
