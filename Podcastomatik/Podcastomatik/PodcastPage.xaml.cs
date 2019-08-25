using Podcastomatik.Shared.Models;
using Podcastomatik.Shared.Models.Views;
using Podcastomatik.Shared.Services;
using Podcastomatik.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Podcastomatik.ViewModels;
using Podcastomatik.MessageMarkers;

namespace Podcastomatik
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PodcastPage : ContentPage
    {
        private IStreaming streamingService;
        
        

        //public string SourceUrl { get; private set; }
        //public PlayerOption PlayerOption { get; private set; }

        public PodcastPage(Podcast podcast)
        {
            streamingService = DependencyService.Get<IStreaming>();

            streamingService.PlayerStarted += StreamingService_PlayerStarted;

            InitializeComponent();

            BindingContext = new PodcastPageViewModel(podcast);

            this.AddActivityIndicator();
        }

        private void StreamingService_PlayerStarted(object sender, EventArgs e)
        {
            MessagingCenter.Send(new MediaPlayerMessage(), nameof(App.Messages.PlayerStarted));
        }

        private void PlayPauseButton_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var episode = (PodcastEpisodeView)btn.BindingContext;
            bool playing = Convert.ToBoolean(btn.Resources["Playing"]);

            if (playing)
            {
                btn.Text = "|>";
                MessagingCenter.Send(new MediaPlayerMessage(), nameof(App.Messages.PauseEpisode), $"{episode.Title}|{episode.OriginalDuration}");
                streamingService.Play(episode.MediaUrl);
            }
            else
            {
                btn.Text = "| |";
                MessagingCenter.Send(new MediaPlayerMessage(), nameof(App.Messages.PlayEpisode), $"{episode.Title}|{episode.OriginalDuration}");
                streamingService.Play(episode.MediaUrl);
            }
        }
    }
}