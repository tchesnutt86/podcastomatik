using Podcastomatik.MessageMarkers;
using Podcastomatik.Services;
using Podcastomatik.Shared.Helpers;
using Podcastomatik.Shared.Models;
using Podcastomatik.Shared.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Podcastomatik.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayPauseButton
    {
        public PodcastEpisodeView PodcastEpisode { get; set; }
        public static readonly BindableProperty PodcastEpisodeProperty = BindableProperty.Create(
            propertyName: nameof(PodcastEpisode),
            returnType: typeof(PodcastEpisodeView),
            declaringType: typeof(PlayPauseButton),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: PodcastEpisodePropertyChanged);

        private static PlayPauseButtonViewModel playPauseButtonViewModel;

        public PlayPauseButton()
        {
            InitializeComponent();

            playPauseButtonViewModel = new PlayPauseButtonViewModel();

            MyButton.BindingContext = playPauseButtonViewModel;
        }

        private void PlayPauseButton_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            var playPauseClass = (PlayPauseButtonViewModel)btn.BindingContext;
            var episode = playPauseClass.PodcastEpisode;
            PropertyEpisodeState episodeState = AppPropertyManager.EpisodeState;

            if (episodeState != null && episodeState.IsPlaying && episodeState.EpisodeId == PodcastEpisode.Id)
            {
                MessagingCenter.Send(
                    new MediaPlayerPauseMessage(),
                    App.PAUSE_EPISODE);
            }
            else if (episodeState != null && episodeState.EpisodeId != PodcastEpisode.Id)
            {
                MessagingCenter.Send(
                    new MediaPlayerPlayMessage(),
                    App.PLAY_EPISODE,
                    BuildPlayPacket(episode));
            }
            else
            {
                MessagingCenter.Send(
                    new MediaPlayerPlayMessage(),
                    App.PLAY_EPISODE,
                    BuildPlayPacket(episode));
            }
        }

        private string BuildPlayPacket(PodcastEpisodeView episode)
        {
            string playPacket = "";

            if (episode != null)
            {
                TimeSpan originalDuration = Utilities.GetTimeSpanFromDuration(episode.OriginalDuration);
                string formattedDurationForDisplay = $"{originalDuration.Minutes}:{originalDuration.Seconds}";
                string totalSeconds = ((int)Math.Round(originalDuration.TotalSeconds)).ToString();

                playPacket = $"{episode.Id}|{episode.Title}|{totalSeconds}|{formattedDurationForDisplay}|{episode.MediaUrl}";
            }

            return playPacket;
        }

        private static void PodcastEpisodePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as PlayPauseButton;

            control.PodcastEpisode = (PodcastEpisodeView)newValue;

            playPauseButtonViewModel.PodcastEpisode = (PodcastEpisodeView)newValue;
        }
    }
}