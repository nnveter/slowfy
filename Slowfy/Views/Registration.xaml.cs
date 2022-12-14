// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using App2;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace XamlBrewer.WinUI3.Navigation.Sample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Registration : Page
    {
        public string State = "Reg";
        public Frame ContentFrame;
        public NavigationView navigationView;
        public Registration()
        {
            this.InitializeComponent();
            ContentFrame = MainWindow.ContentFr;
            navigationView = MainWindow.Nav;

            TextBlock.Text = "??????? ????? ???????";
            Name.PlaceholderText = "???";
            Email.PlaceholderText = "?????";
            passworBoxWithRevealmode.PlaceholderText = "??????";
            revealModeCheckBox.Content = "???????? ??????";
            myButton.Content = "??????";
            Hyperlink.Content = "? ??? ??? ???? ????????";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (State == "Reg")
            {
                State = "Log";
                TextBlock.Text = "?????";
                Name.Visibility = Visibility.Collapsed;
                Hyperlink.Content = "? ??? ??? ??? ?????????";
            }
            else
            {
                State = "Reg";
                TextBlock.Text = "??????? ????? ???????";
                Name.Visibility = Visibility.Visible;
                Hyperlink.Content = "? ??? ??? ???? ????????";

            }
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Visibility = Visibility.Visible;
            var Name1 = Name.Text;
            var Email1 = Email.Text;
            var Password = passworBoxWithRevealmode.Password;
            var client = new HttpClient();

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
            if (State == "Reg")
            {
                string response = await new ReqService().Post($"{Constants.URL}users/Create", new Dictionary<string, string>()
                {
                    { "Email", Email1 },
                    { "Password", Password },
                    { "Name", Name1 },
                });

                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                var res = response;
                if (res != "bad request")
                {

                    localSettings.Values["Name"] = Name1;
                    ProgressBar.Visibility = Visibility.Collapsed;
                    localSettings.Values["JwtToken"] = response;
                    ContentFrame.Navigate(typeof(HomePage));

                    navigationView.IsPaneVisible = true;
                }
                else
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    Info.Visibility = Visibility.Visible;
                    Info.Message = res;

                }
            }
            else
            {
                string response = await new ReqService().Post($"{Constants.URL}users/Login", new Dictionary<string, string>()
                {
                    { "Email", Email1 },
                    { "Password", Password },
                });


                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                var res = response;

                if (res != "bad request")
                {

                    String Datacontext = "?????";
                    String localValue2 = localSettings.Values["Name"] as string;

                    if (DateTime.Now.Hour <= 24 && DateTime.Now.Hour >= 19)
                    {
                        Datacontext = "?????? ?????, " + localValue2;
                    }
                    else if (DateTime.Now.Hour < 19 && DateTime.Now.Hour >= 9)
                    {
                        Datacontext = "?????? ????, " + localValue2;
                    }
                    else if (DateTime.Now.Hour < 9 && DateTime.Now.Hour >= 6)
                    {
                        Datacontext = "?????? ????, " + localValue2;
                    }
                    else if (DateTime.Now.Hour < 6)
                    {
                        Datacontext = "??????????? " + localValue2;
                    }



                    localSettings.Values["Name"] = await new ReqService().Get($"{Constants.URL}users/GetMyName", response);
                    ProgressBar.Visibility = Visibility.Collapsed;
                    localSettings.Values["JwtToken"] = response;
                    ContentFrame.Navigate(typeof(HomePage));

                    MainWindow.Nav.IsBackEnabled = true;
                    ContentFrame.Navigate(
                    Type.GetType("XamlBrewer.WinUI3.Navigation.Sample.Views.HomePage"), "Home");
                    MainWindow.Nav.Header = Datacontext;
                    MainWindow.Nav.SelectedItem = typeof(HomePage);

                    navigationView.IsPaneVisible = true;
                }
                else
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    Info.Visibility = Visibility.Visible;
                    Info.Message = res;

                }

            }

        }
        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealModeCheckBox.IsChecked == true)
            {
                passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                passworBoxWithRevealmode.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }
    }
}
