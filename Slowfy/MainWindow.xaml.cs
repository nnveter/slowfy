// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices; // For DllImport
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System.Display;
using WinRT;
using XamlBrewer.WinUI3.Navigation.Sample.Views;

namespace App2
{

    public sealed partial class MainWindow : Window
    {

        WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See below for implementation.
        MicaController m_backdropController;
        SystemBackdropConfiguration m_configurationSource;

        int nav = 1;
        String State = "Reg";

        public static MediaPlayerElement pl;
        public static StackPanel Stackpan;
        public static TextBlock txtTitle;
        public static TextBlock txtAutor;
        public static Frame ContentFr;
        public static NavigationView Nav;
        public static String Page_ = "Home";
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Slowfy";

            pl = Player;
            ContentFr = ContentFrame;
            Nav = NavigationView;
            Stackpan = StackPan;
            txtTitle = TxtTitle;
            txtAutor = TxtAutor;

            Player.MediaPlayer.Volume = 0.15;
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            TrySetSystemBackdrop();
            NavigationView.IsPaneVisible = false;


            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String localValue = localSettings.Values["Name"] as string;

            if (DateTime.Now.Hour <= 24 && DateTime.Now.Hour >= 19)
            {
                home.DataContext = "?????? ?????, " + localValue;
            }
            else if (DateTime.Now.Hour < 19 && DateTime.Now.Hour >= 9)
            {
                home.DataContext = "?????? ????, " + localValue;
            }
            else if (DateTime.Now.Hour < 9 && DateTime.Now.Hour >= 6)
            {
                home.DataContext = "?????? ????, " + localValue;
            }
            else if (DateTime.Now.Hour < 6)
            {
                home.DataContext = "??????????? " + localValue;
            }

            find.DataContext = "?????";
            music.DataContext = "??? ?????????";
            account.DataContext = "???????";
            find.Content = "?????";
            music.Content = "??? ?????????";
            home.Content = "???????";
            account.Content = "???????";
            NavigationView.PaneTitle = "????";

            TextBlock.Text = "??????? ????? ???????";
            Name.PlaceholderText = "???";
            Email.PlaceholderText = "?????";
            passworBoxWithRevealmode.PlaceholderText = "??????";
            revealModeCheckBox.Content = "???????? ??????";
            myButton.Content = "??????";
            Hyperlink.Content = "? ??? ??? ???? ????????";
        } 

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            nav--;
            if (nav <= 0)
            {
                NavigationView.IsBackEnabled = false;
            }
            ContentFrame.GoBack();

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
                    localSettings.Values["Name"] = await new ReqService().Get($"{Constants.URL}users/GetMyName", response);
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
            if ((String)item.DataContext == "My music" || (String)item.DataContext == "??? ?????????")
            {
                ico.Source = new BitmapImage(new Uri("ms-appx:///Views/heart2.png"));
            }
            else
            {
                ico.Source = new BitmapImage(new Uri("ms-appx:///Views/hear1.png"));
            }
            

            if (((String)item.DataContext == "My music" || (String)item.DataContext == "??? ?????????" ||
                (String)item.DataContext == "Find" || (String)item.DataContext == "?????" ||
                (String)item.DataContext == "Tracks" || (String)item.Content == "???????") && Player.Source != null)
            {
                StackPan.Visibility = Visibility.Visible;
            }
            else
            {
                StackPan.Visibility = Visibility.Collapsed;
            }

            Page_ = (string)item.Tag;
            nav++;
            NavigationView.IsBackEnabled = true;
            ContentFrame.Navigate(
            Type.GetType(item.Tag.ToString()), item.Content);
            NavigationView.Header = item.DataContext;
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

                result = await new ReqService().Get($"{Constants.URL}users/checktoken", localValue);

                if (result == "Ok")
                {
                    ContentFrame.Navigate(typeof(HomePage));
                    SetCurrentNavigationViewItem(GetNavigationViewItems(typeof(HomePage)).First());
                    NavigationView.IsPaneVisible = true;
                    NavigationView.IsBackEnabled = false;
                    nav = 0;

                    String Sc = localSettings.Values["LastSource"] as string;
                    if (Sc != null)
                    {
                        String title = localSettings.Values["LastTitle"] as string;
                        String autor = localSettings.Values["LastAutor"] as string;
                        TxtTitle.Text = title;
                        TxtAutor.Text = autor;
                        Player.Source = MediaSource.CreateFromUri(new Uri(Sc));
                        Player.MediaPlayer.Pause();

                        StackPan.Visibility = Visibility.Visible;
                    }
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

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {

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




