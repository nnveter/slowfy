// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using App2.Model;
using App2;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.Core;
using Windows.Storage;
using System.Text.Json;


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
        public FindPage()
        {
            this.InitializeComponent();
            Player.TransportControls.IsZoomButtonVisible = false;
            Player.TransportControls.IsZoomEnabled = false;
            Player.TransportControls.IsPlaybackRateButtonVisible = false;
            Player.TransportControls.IsNextTrackButtonVisible = true;
            Player.TransportControls.IsPreviousTrackButtonVisible = true;
            Player.TransportControls.IsPlaybackRateEnabled = true;
            Player.TransportControls.IsCompact = true;
            Pro();
            Find.PlaceholderText = "¬ведите название трека";
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
            idTrack = 0;

            var result = await new ReqService().Get($"{Constants.URL}tracks/search?q={Find.Text}&count=10");
            List<Track> rec =
                JsonSerializer.Deserialize<List<Track>>(result);
            //rec.Reverse();
            trackName = rec;
            foreach (Track track in rec)
            {
                track.listid = idTrack;
                TestView.Items.Add(track);
                idTrack++;
            }
        }

        private async void Pro()
        {
            string result2 = await new ReqService().Get($"{Constants.URL}tracks");

            List<Track> rec =
                JsonSerializer.Deserialize<List<Track>>(result2);
            //rec.Reverse();
            trackName = rec;
            foreach (Track track in rec)
            {
                TestView.Items.Add(track);
            }
            
        }
        private async void TestView_SelectionChanged(object sender, SelectionChangedEventArgs e) // Event handler
        {
            // Looking at if the list is anything more than 0 items, they can be removed
            if (TestView.SelectedIndex > -1)
            {
                Player.Source = MediaSource.CreateFromUri(new Uri(trackName[TestView.SelectedIndex].source));


                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;


                // load a setting that is local to the device
                String localValue = localSettings.Values["JwtToken"] as string;

                await new ReqService().Get($"{Constants.URL}Auditions/AddAudition?trackId=" + trackName[TestView.SelectedIndex].id, localValue);
                //trackName = await new ReqService().GetTracks();
            }
        }

        private void TestView_ItemClick(object sender, ItemClickEventArgs e)
        {




        }

        private async void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Image element = (Image)sender;
            element.Source = new BitmapImage(new Uri("ms-appx:///Views/heart2.png"));
            int a = (int)element.DataContext;
            //element.Symbol = Symbol.SolidStar;
            //element.Visibility = Visibility.Collapsed;
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String localValue = localSettings.Values["JwtToken"] as string;
            await new ReqService().Get($"{Constants.URL}FavTracks/AddToFavourite?trackId=" + trackName[a].id, localValue);
        }
    }
}
