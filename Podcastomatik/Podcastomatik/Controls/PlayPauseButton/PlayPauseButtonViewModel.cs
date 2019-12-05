using Podcastomatik.MessageMarkers;
using Podcastomatik.Services;
using Podcastomatik.Shared;
using Podcastomatik.Shared.Helpers;
using Podcastomatik.Shared.Models.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Podcastomatik.Controls
{
    public class PlayPauseButtonViewModel : BaseBindable
    {
        private string buttonText = ">";
        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;

                RaisePropertyChanged();
            }
        }

        public PodcastEpisodeView PodcastEpisode { get; set; }

        public PlayPauseButtonViewModel()
        {
            MessagingCenter.Subscribe<ResourcePropertyChangedMessage>(this, App.RESOURCE_PROPERTY_CHANGED, (sender) =>
            {
                UpdateButton();
            });

            UpdateButton();
        }

        private void UpdateButton()
        {
            var episodeState = AppPropertyManager.EpisodeState;

            if (PodcastEpisode == null && episodeState == null)
                return;
            else if (PodcastEpisode != null && episodeState != null && episodeState.EpisodeId == PodcastEpisode.Id)
                ButtonText = episodeState.IsPlaying ? "||" : ">";
            else
                ButtonText = ">";
        }
    }
}
