// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using App2;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        public AccountPage()
        {
            this.InitializeComponent();
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String Name = localSettings.Values["Name"] as string;
            if (!String.IsNullOrEmpty(Name))
            {
                textblock.Text = Name;
            }
            else
            {
                SetName();
            }
        }

        private async void SetName()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String token = localSettings.Values["JwtToken"] as string;
            textblock.Text = await new ReqService().Get($"{Constants.URL}users/GetMyName", token);
        }



        private void bt_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["JwtToken"] = null;
            localSettings.Values["LastSource"] = null;
            MainWindow.ContentFr.Navigate(typeof(Registration));
            MainWindow.ContentFr.Navigate(
            Type.GetType("XamlBrewer.WinUI3.Navigation.Sample.Views.Registration"), "Autorization");
            MainWindow.Nav.Header = "";

            MainWindow.Nav.IsPaneVisible = false;

        }
    }
}
