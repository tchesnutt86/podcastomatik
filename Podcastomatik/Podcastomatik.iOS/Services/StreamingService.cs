using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVFoundation;
using Foundation;
using Podcastomatik.iOS.Services;
using Podcastomatik.Shared.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(StreamingService))]
namespace Podcastomatik.iOS.Services
{
    public class StreamingService : IStreaming
    {
        AVPlayer player;
        bool isPrepared;
        string dataSource = "https://www.radiantmediaplayer.com/media/bbb-360p.mp4";

        public string CurrentPodcastTitle { get; set; }

        public event EventHandler PlayerStarted;

        //private bool IsPlaying
        //{
        //    get => player.Rate != 0 && player.Error == null;
        //}

        public void Play(string uri)
        {
            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
            if (!isPrepared || player == null)
                player = AVPlayer.FromUrl(NSUrl.FromString(uri));

            isPrepared = true;
            player.Play();
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Stop()
        {
            player.Dispose();
            isPrepared = false;
        }

        public string Status()
        {
            if (player.TimeControlStatus == AVPlayerTimeControlStatus.Playing)
                return $"{player.CurrentTime.Seconds} / {player.CurrentItem.Asset.Duration.Seconds}";

            return "nada";
        }
    }
}