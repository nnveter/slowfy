// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI;
using Microsoft.UI.Windowing;
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
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Composition.SystemBackdrops;
using System.Runtime.InteropServices; // For DllImport
using WinRT;
using WinRT.Interop;
using Microsoft.Toolkit.Uwp.Notifications;
using XamlBrewer.WinUI3.Navigation.Sample.Views;
using System.Net.Http;
using Windows.Storage;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using PInvoke;

namespace App2
{
  
    public sealed partial class MainWindow : Window
    {

        WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See below for implementation.
        MicaController m_backdropController;
        SystemBackdropConfiguration m_configurationSource;
        int nav = 1;
        String State = "Reg";
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Slowfy";
            
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            TrySetSystemBackdrop();
            NavigationView.IsPaneVisible = false;

        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            nav--;
            if (nav <= 0) {
                NavigationView.IsBackEnabled = false;
            }
            ContentFrame.GoBack();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (State == "Reg")
            {
                State = "Log";
                TextBlock.Text = "Login";
                Name.Visibility = Visibility.Collapsed;
                Hyperlink.Content = "Don't have an account yet?";
            }
            else {
                State = "Reg";
                TextBlock.Text = "Create new account";
                Name.Visibility = Visibility.Visible;
                Hyperlink.Content = "Do you already have an account?";

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
                string response = await new ReqService().Post("https://localhost:7148/users/Create", new Dictionary<string, string>()
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

                    SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(HomePage)).First());
                    NavigationView.IsPaneVisible = true;
                }
                else
                {
                    ProgressBar.Visibility = Visibility.Collapsed;
                    Info.Visibility = Visibility.Visible;
                    Info.Message = res;

                }
            }
            else {
                string response = await new ReqService().Post("https://localhost:7148/users/Login", new Dictionary<string, string>()
                {
                    { "Email", Email1 },
                    { "Password", Password },
                });


                ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                var res = response;
                if (res != "bad request")
                {
                    localSettings.Values["Name"] = await new ReqService().Get("https://localhost:7148/users/GetMyName", response);
                    ProgressBar.Visibility = Visibility.Collapsed;
                    localSettings.Values["JwtToken"] = response;
                    ContentFrame.Navigate(typeof(HomePage));

                    SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(HomePage)).First());
                    NavigationView.IsPaneVisible = true;
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







        private void NavigationView_SelectionChanged(
    NavigationView sender,
    NavigationViewSelectionChangedEventArgs args)
        {
            SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
        }

        public void SetCurrentNavigationViewItem(
    NavigationViewItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item.Tag == null)
            {
                return;
            }
            nav++;
            NavigationView.IsBackEnabled = true;
            ContentFrame.Navigate(
            Type.GetType(item.Tag.ToString()),item.Content);
            NavigationView.Header = item.Content;
            NavigationView.SelectedItem = item;
        }

        private async void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // load a setting that is local to the device
            String localValue = localSettings.Values["JwtToken"] as string;
            String result = null;
            if (localValue != null && localValue != "bad")
            {

                result = await new ReqService().Get("https://localhost:7148/users/checktoken", localValue);

                if (result == "Ok")
                {

                    ContentFrame.Navigate(typeof(HomePage));

                    SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(HomePage)).First());
                    NavigationView.IsPaneVisible = true;
                    NavigationView.IsBackEnabled = false;
                    nav = 0;
                }
            }

           
            
        }

        public List<NavigationViewItem> GetNavigationViewItems()
        {
            var result = new List<NavigationViewItem>();
            var items = NavigationView.MenuItems.Select(i => (NavigationViewItem)i).ToList();
            items.AddRange(NavigationView.FooterMenuItems.Select(i => (NavigationViewItem)i));
            result.AddRange(items);

            foreach (NavigationViewItem mainItem in items)
            {
                result.AddRange(mainItem.MenuItems.Select(i => (NavigationViewItem)i));
            }

            return result;
        }

        public List<NavigationViewItem> GetNavigationViewItems(
            Type type)
        {
            return GetNavigationViewItems().Where(i => i.Tag.ToString() == type.FullName).ToList();
        }

        public List<NavigationViewItem> GetNavigationViewItems(
            Type type,
            string title)
        {
            return GetNavigationViewItems(type).Where(ni => ni.Content.ToString() == title).ToList();
        }

        public NavigationViewItem GetCurrentNavigationViewItem()
        {
            return NavigationView.SelectedItem as NavigationViewItem;
        }

        public interface INavigation
        {
            NavigationViewItem GetCurrentNavigationViewItem();

            List<NavigationViewItem> GetNavigationViewItems();

            List<NavigationViewItem> GetNavigationViewItems(Type type);

            List<NavigationViewItem> GetNavigationViewItems(Type type, string title);

            void SetCurrentNavigationViewItem(NavigationViewItem item);
        }

        

       







        //Thems
        bool TrySetSystemBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Create the policy object.
                m_configurationSource = new SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_backdropController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_backdropController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_backdropController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed
            // so it doesn't try to use this closed window.
            if (m_backdropController != null)
            {
                m_backdropController.Dispose();
                m_backdropController = null;
            }
            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }

    }

    class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

        object m_dispatcherQueueController = null;
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (m_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;    // DQTYPE_THREAD_CURRENT
                options.apartmentType = 2; // DQTAT_COM_STA

                CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
            }
        }

    }
}




