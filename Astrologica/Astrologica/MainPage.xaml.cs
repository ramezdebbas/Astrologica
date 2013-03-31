using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Astrologica
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;
            const string starString = "Aries,Taurus,Gemini,Cancer,Leo,Virgo,Libra,Scorpio,Sagittarius,Capricorn,Aquarius,Pisces";
            var stars=new ObservableCollection<string>();
            foreach (var str in starString.Split(Convert.ToChar(",")))
            {
                stars.Add(str);    
            }
            CmbStars.ItemsSource = stars;
            CmbStars.SelectionChanged += CmbStars_SelectionChanged;

            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();

            var interfaceType = profile.NetworkAdapter.IanaInterfaceType;
            // 71 is WiFi & 6 is Ethernet
            if (!(interfaceType == 71 | interfaceType == 6))
            {
                TxtResult.Text = "Error, Check connection or Try connecting to the Internet...";
                CmbStars.IsEnabled = false;
            }
            else
            {
                CmbStars.IsEnabled = true;
            }
        }

        void CmbStars_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbStars.SelectedValue != null) GetStarResult(CmbStars.SelectedValue.ToString());
        }

        private async void GetStarResult(string star)
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();

            var interfaceType = profile.NetworkAdapter.IanaInterfaceType;
            // 71 is WiFi & 6 is Ethernet
            if (!(interfaceType == 71 | interfaceType == 6))
            {
                TxtResult.Text = "Error, Check connection or Try connecting to the Internet...";
                CmbStars.IsEnabled = false;
            }
            else
            {
                CmbStars.IsEnabled = true;
                var url = "http://www.santabanta.com/astro_js.asp?sign=" + star;

                var client = new HttpClient { MaxResponseContentBufferSize = 256000 };
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = jsonResponse.Substring(jsonResponse.IndexOf("</b>", System.StringComparison.Ordinal) + 4);
                var finalResult = result.Substring(0, result.IndexOf("</p>", System.StringComparison.Ordinal));
                TxtResult.Text = finalResult;
            }
            
        }

        

        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            bool afound = false;
            bool sfound = false;
            bool pfound = false;
            foreach (var command in args.Request.ApplicationCommands.ToList())
            {
                if (command.Label == "About")
                {
                    afound = true;
                }
                if (command.Label == "Settings")
                {
                    sfound = true;
                }
                if (command.Label == "Policy")
                {
                    pfound = true;
                }
            }
            if (!afound)
                args.Request.ApplicationCommands.Add(new SettingsCommand("s", "About", (p) => { cfoAbout.IsOpen = true; }));
            //if (!sfound)
            //    args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Settings", (p) => { cfoSettings.IsOpen = true; }));
            //if (!pfound)
            //    args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Policy", (p) => { cfoPolicy.IsOpen = true; }));
            if (!afound)
            args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy policy", OpenPrivacyPolicy));

        }

        private async void OpenPrivacyPolicy(IUICommand command)
        {
            var uri = new Uri("http://www.thatslink.com/privacy-statment/ ");
            await Launcher.LaunchUriAsync(uri);
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
