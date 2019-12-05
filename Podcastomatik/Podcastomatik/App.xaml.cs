using System;
using Xamarin.Forms;

namespace Podcastomatik
{
    public partial class App : Application
    {
        public static string PLAY_EPISODE = "PlayEpisode";
        public static string PAUSE_EPISODE = "PauseEpisode";
        public static string PLAYER_STARTED = "PlayerStarted";
        public static string RESOURCE_PROPERTY_CHANGED = "ResourcePropertyChanged";
        
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
