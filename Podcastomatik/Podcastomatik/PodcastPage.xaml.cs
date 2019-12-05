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
using Podcastomatik.Services;
using Podcastomatik.Shared.Helpers;

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
            MessagingCenter.Send(new MediaPlayerStartedMessage(), App.PLAYER_STARTED);
        }

        private void PlayPauseButton_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var episode = (PodcastEpisodeView)btn.BindingContext;
            bool playing = Convert.ToBoolean(btn.Resources["Playing"]);

            if (playing)
            {
                btn.Text = "▶";
                MessagingCenter.Send(new MediaPlayerPauseMessage(), App.PAUSE_EPISODE);
                //streamingService.Pause();
                btn.Resources["Playing"] = false;
            }
            else
            {
                TimeSpan originalDuration = Utilities.GetTimeSpanFromDuration(episode.OriginalDuration);
                // issue. need to figure out how to go about setting the episode state in the correct order and properly.
                // right now the episode details are not being stored correctly resulting in wrong data during play/pause.
                //AppPropertyManager.EpisodeState = new PropertyEpisodeState
                //{
                //    ElapsedSeconds = 0,
                //    EpisodeId = episode.Id,
                //    EpisodeTitle = episode.Title,
                //    TotalDurationSeconds = (int)Math.Round(originalDuration.TotalSeconds),
                //};
                btn.Text = "❚❚";
                string formattedDurationForDisplay = $"{originalDuration.Minutes}:{originalDuration.Seconds}";
                string totalSeconds = ((int)Math.Round(originalDuration.TotalSeconds)).ToString();
                MessagingCenter.Send(
                    new MediaPlayerPlayMessage(),
                    App.PLAY_EPISODE,
                    $"{episode.Id}|{episode.Title}|{totalSeconds}|{formattedDurationForDisplay}|{episode.MediaUrl}");
                //streamingService.Play(episode.MediaUrl);
                btn.Resources["Playing"] = true;
            }
        }
    }
}