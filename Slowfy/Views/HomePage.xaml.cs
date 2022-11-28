using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    public sealed partial class HomePage : Page
    {
        

        public HomePage()
        {
            this.InitializeComponent();
        }

        private void EditTask_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // I need to work on this later :)
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
                SuccessBar.IsOpen = true;
                string addNewTask = (string)dialog.Tag;
                CancelBar.IsOpen = false;
                TestView.Items.Add(addNewTask);
                // Waits 3 seconds then hides bar again
                await Task.Delay(TimeSpan.FromSeconds(3));
                SuccessBar.IsOpen = false;
            }
            else
            {
                CancelBar.IsOpen = true;
                SuccessBar.IsOpen = false;
                // Waits 3 seconds then hides bar again
                await Task.Delay(TimeSpan.FromSeconds(3));
                CancelBar.IsOpen = false;
            }
        }

        // Handles removal of items in the List.
        private void TestView_SelectionChanged(object sender, SelectionChangedEventArgs e) // Event handler
        {
            // Looking at if the list is anything more than 0 items, they can be removed
            while (TestView.SelectedIndex > -1)
            {
                // Function to remove items
                TestView.Items.RemoveAt(TestView.SelectedIndex);
            }
        }

    }
}
