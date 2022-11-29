using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Windows.Media.Core;

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    public sealed partial class HomePage : Page
    {
        
        public List<string> trackName = new List<string>();
        public List<string> trackTime = new List<string>();
        public HomePage()
        {
            this.InitializeComponent();
            TestView.Items.Add("Mazzelov | Помню");
            trackName.Add("file:///C:/Users/Илья/Downloads/mazellovvv_maz_korzh_-_YA_pomnyu_(musmore.com).mp3");
            TestView.Items.Add("Шарлот | Щека на щеку");
            trackName.Add("file:///C:/Users/Илья/Downloads/v0_10056371543_1_1.mp3");
            Player.TransportControls.IsZoomButtonVisible = false;
            Player.TransportControls.IsZoomEnabled = false;
            Player.TransportControls.IsPlaybackRateButtonVisible = false;
            Player.TransportControls.IsNextTrackButtonVisible = true;
            Player.TransportControls.IsPreviousTrackButtonVisible = true;
            Player.TransportControls.IsPlaybackRateEnabled = true;
            Player.TransportControls.IsCompact = true;
        }

        

        private void EditTask_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // I need to work on this later :)
            while (TestView.SelectedIndex > -1)
            {
            // Function to remove items
            TestView.Items.RemoveAt(TestView.SelectedIndex);
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
        private void TestView_SelectionChanged(object sender, SelectionChangedEventArgs e) // Event handler
        {
            // Looking at if the list is anything more than 0 items, they can be removed
            if (TestView.SelectedIndex > -1)
            {
                Player.Source = MediaSource.CreateFromUri(new Uri(trackName[TestView.SelectedIndex]));
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void TestView_ItemClick(object sender, ItemClickEventArgs e)
        {

            
            
            
        }
    }
}
