using Podcastomatik.MessageMarkers;
using Podcastomatik.Shared;
using Podcastomatik.Shared.Models.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Xamarin.Forms;

namespace Podcastomatik.Controls
{
    public class BottomSheetViewModel : BaseBindable
    {
        private string elapsedSecondsKey = "ElapsedSeconds";
        private string episodeIdKey = "EpisodeId";
        private string title;
        private string timeInfo;
        private Timer timer = new Timer();
        private int seconds = 0;
        private string duration = "";

        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged();
            }
        }
        public string TimeInfo
        {
            get => timeInfo;
            set
            {
                timeInfo = value;
                RaisePropertyChanged();
            }
        }

        public BottomSheetViewModel()
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;

            if (Application.Current.Properties.ContainsKey(elapsedSecondsKey))
                seconds = Convert.ToInt32(Application.Current.Properties[elapsedSecondsKey]);

            MessagingCenter.Subscribe<MediaPlayerMessage, string>(this, nameof(App.Messages.PlayEpisode), (sender, episodeInfo) =>
            {
                string[] args = episodeInfo.Split('|');
                Title = args[0];
                duration = args[1];
                seconds = 0;
            });

            MessagingCenter.Subscribe<MediaPlayerMessage>(this, nameof(App.Messages.PauseEpisode), sender =>
            {
                timer.Stop();
            });

            MessagingCenter.Subscribe<MediaPlayerMessage>(this, nameof(App.Messages.PlayerStarted), sender =>
            {
                timer.Start();
            });
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            seconds++;
            TimeInfo = $" {FormatTime(seconds)} / {duration}";
        }

        private void SetTimeInfo()
        {
            var p = Application.Current.Properties[""];
        }

        string FormatTime(double time)
        {
            double minutes = time / 60;
            double seconds = time % 60;

            return string.Format("{0}:{1:D2}", (int)minutes, (int)seconds);
        }
    }
}
