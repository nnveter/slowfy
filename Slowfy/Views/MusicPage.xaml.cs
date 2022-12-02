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

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    public sealed partial class MusicPage : Page
    {
        public List<Track> trackName;
        public MediaPlayerElement Player;
        public static StackPanel Stackpan;
        public MusicPage()
        {
            this.InitializeComponent();

            Pro();
            Player = MainWindow.pl;
            Stackpan = MainWindow.Stackpan;

            Player.TransportControls.IsZoomButtonVisible = false;
            Player.TransportControls.IsZoomEnabled = false;
            Player.TransportControls.IsPlaybackRateButtonVisible = false;
            Player.TransportControls.IsNextTrackButtonVisible = true;
            Player.TransportControls.IsPreviousTrackButtonVisible = true;
            Player.TransportControls.IsPlaybackRateEnabled = true;
            Player.TransportControls.IsCompact = true;
        }

        private async void Pro()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            String localValue = localSettings.Values["JwtToken"] as string;

            string result2 = await new ReqService().Get($"{Constants.URL}favtracks/GetMyFavorite", localValue);

            List<Track> rec =
                JsonSerializer.Deserialize<List<Track>>(result2);
            //rec.Reverse();
            trackName = rec;
            foreach (Track track in rec)
            {
                TestView.Items.Add(track);
            }
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
                String localValue = localSettings.Values["JwtToken"] as string;

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
            Image element = (Image)sender;

            element.Source = new BitmapImage(new Uri("ms-appx:///Views/hear1.png"));
            //element.Symbol = Symbol.SolidStar;
            //element.Visibility = Visibility.Collapsed;
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String localValue = localSettings.Values["JwtToken"] as string;
            await new ReqService().Get($"{Constants.URL}FavTracks/AddToFavourite?trackId=" + trackName[(int)element.DataContext - 1].id, localValue);
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
