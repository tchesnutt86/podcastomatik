using Podcastomatik.MessageMarkers;
using Podcastomatik.Services;
using Podcastomatik.Shared;
using Podcastomatik.Shared.Models;
using Podcastomatik.Shared.Models.Views;
using Podcastomatik.Shared.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Xamarin.Forms;

namespace Podcastomatik.Controls
{
    public class BottomSheetViewModel : BaseBindable
    {
        private IStreaming streamingService;
        
        private Timer timer = new Timer();
        private int elapsedSeconds = 0;
        private string duration = "";

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }

        private string timeInfo;
        public string TimeInfo
        {
            get => timeInfo;
            set
            {
                timeInfo = value;
                RaisePropertyChanged();
            }
        }

        private bool playHistoryExists = false;
        public bool PlayHistoryExists
        {
            get => playHistoryExists;
            set
            {
                playHistoryExists = value;
                RaisePropertyChanged();
            }
        }

        private string playPauseButtonText = ">";
        public string PlayPauseButtonText
        {
            get => playPauseButtonText;
            set
            {
                playPauseButtonText = value;
                RaisePropertyChanged();
            }
        }

        public BottomSheetViewModel()
        {
            streamingService = DependencyService.Get<IStreaming>();

            streamingService.PlayerStarted += (object sender, EventArgs e) => timer.Start();

            SubscribeToMessagingCenter();

            timer.Interval = 1000; // One second.
            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                elapsedSeconds++;

                UpdateTimeInfo();
            };

            LoadPlayHistory();
        }

        private void SubscribeToMessagingCenter()
        {
            MessagingCenter.Subscribe<ResourcePropertyChangedMessage>(this, App.RESOURCE_PROPERTY_CHANGED, (sender) =>
            {
                UpdateButton();
            });

            MessagingCenter.Subscribe<MediaPlayerPlayMessage, string>(this, App.PLAY_EPISODE, (sender, episodeInfo) =>
            {
                PropertyEpisodeState existingEpisodeState = AppPropertyManager.EpisodeState;

                if (string.IsNullOrEmpty(episodeInfo))
                {
                    Title = existingEpisodeState.EpisodeTitle;
                    duration = existingEpisodeState.FormattedDuration;
                    elapsedSeconds = existingEpisodeState.ElapsedSeconds ?? 0;

                    AppPropertyManager.EpisodeState = new PropertyEpisodeState
                    {
                        ElapsedSeconds = existingEpisodeState.ElapsedSeconds,
                        EpisodeId = existingEpisodeState.EpisodeId,
                        EpisodeTitle = existingEpisodeState.EpisodeTitle,
                        EpisodeUrl = existingEpisodeState.EpisodeUrl,
                        FormattedDuration = existingEpisodeState.FormattedDuration,
                        IsPlaying = true,
                        TotalDurationSeconds = existingEpisodeState.TotalDurationSeconds,
                    };
                }
                else
                {
                    string[] args = episodeInfo.Split('|');
                    int argEpisodeId = Convert.ToInt32(args[0]);
                    string argEpisodeTitle = args[1];
                    int argEpisodeTotalSeconds = Convert.ToInt32(args[2]);
                    string argFormattedDurationForDisplay = args[3];
                    string argMediaUrl = args[4];

                    if (existingEpisodeState != null && argEpisodeId == existingEpisodeState.EpisodeId)
                    {
                        Title = existingEpisodeState.EpisodeTitle;
                        duration = argFormattedDurationForDisplay;
                        elapsedSeconds = existingEpisodeState.ElapsedSeconds ?? 0;

                        AppPropertyManager.EpisodeState = new PropertyEpisodeState
                        {
                            ElapsedSeconds = existingEpisodeState.ElapsedSeconds,
                            EpisodeId = existingEpisodeState.EpisodeId,
                            EpisodeTitle = existingEpisodeState.EpisodeTitle,
                            EpisodeUrl = existingEpisodeState.EpisodeUrl,
                            FormattedDuration = existingEpisodeState.FormattedDuration,
                            IsPlaying = true,
                            TotalDurationSeconds = existingEpisodeState.TotalDurationSeconds,
                        };
                    }
                    else
                    {
                        Title = argEpisodeTitle;
                        duration = argFormattedDurationForDisplay; // Formatted already.
                        elapsedSeconds = 0;

                        AppPropertyManager.EpisodeState = new PropertyEpisodeState
                        {
                            ElapsedSeconds = 0,
                            EpisodeId = argEpisodeId,
                            EpisodeTitle = argEpisodeTitle,
                            EpisodeUrl = argMediaUrl,
                            TotalDurationSeconds = argEpisodeTotalSeconds,
                            FormattedDuration = argFormattedDurationForDisplay,
                            IsPlaying = true,
                        };
                    }
                }
                

                streamingService.Play(AppPropertyManager.EpisodeState.EpisodeUrl);

                UpdateTimeInfo();

                PlayHistoryExists = (AppPropertyManager.EpisodeState != null);
            });

            MessagingCenter.Subscribe<MediaPlayerPauseMessage>(this, App.PAUSE_EPISODE, sender =>
            {
                timer.Stop();
                streamingService.Pause();
                PropertyEpisodeState episodeState = AppPropertyManager.EpisodeState;
                AppPropertyManager.EpisodeState = new PropertyEpisodeState // find out what is setting this value so elapsedseconds is set to 0
                {
                    ElapsedSeconds = elapsedSeconds,
                    EpisodeId = episodeState.EpisodeId,
                    EpisodeTitle = episodeState.EpisodeTitle,
                    EpisodeUrl = episodeState.EpisodeUrl,
                    FormattedDuration = episodeState.FormattedDuration,
                    IsPlaying = false,
                    TotalDurationSeconds = episodeState.TotalDurationSeconds,
                };
            });

            //MessagingCenter.Subscribe<MediaPlayerStartedMessage>(this, nameof(App.Messages.PlayerStarted), sender =>
            //{
            //    timer.Start();
            //});

            //MessagingCenter.Subscribe<ResourcePropertyChangedMessage, string>(this, nameof(App.Messages.ResourcePropertyChanged), (sender, resourceName) =>
            //{
            //    if (resourceName == "EpisodeState")
            //        PlayHistoryExists = (AppPropertyManager.EpisodeState != null);
            //});
        }

        private void UpdateButton()
        {
            var episodeState = AppPropertyManager.EpisodeState;

            if (episodeState == null)
                return;
            else if (episodeState != null) // BottomSheet button.
                PlayPauseButtonText = episodeState.IsPlaying ? "||" : ">";
        }

        private void LoadPlayHistory()
        {
            if (AppPropertyManager.EpisodeState != null)
                elapsedSeconds = AppPropertyManager.EpisodeState.ElapsedSeconds.GetValueOrDefault(0);

            PlayHistoryExists = (AppPropertyManager.EpisodeState != null);
        }

        private void UpdateTimeInfo()
        {
            TimeInfo = $" {FormatTime(elapsedSeconds)} / {duration}";
        }

        string FormatTime(double seconds)
        {
            double minutes = seconds / 60;
            double remainingSeconds = seconds % 60;

            return string.Format("{0}:{1:D2}", (int)minutes, (int)remainingSeconds);
        }
    }
}
