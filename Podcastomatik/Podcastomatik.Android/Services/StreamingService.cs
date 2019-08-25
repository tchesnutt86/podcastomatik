using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Podcastomatik.Droid.Services;
using Podcastomatik.Shared.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(StreamingService))]
namespace Podcastomatik.Droid.Services
{
    public class StreamingService : IStreaming
    {
        MediaPlayer player;
        string dataSource = "https://www.radiantmediaplayer.com/media/bbb-360p.mp4";
        public event EventHandler PlayerStarted;

        bool IsPrepared = false;

        public void Play(string uri)
        {
            if (!IsPrepared)
            {
                if (player == null)
                    player = new MediaPlayer();
                else
                    player.Reset();

                player.SetDataSource(uri);
                player.PrepareAsync();
            }

            player.Prepared += (sender, args) =>
            {
                player.Start();
                IsPrepared = true;

                PlayerStarted?.Invoke(this, args);
            };
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Stop()
        {
            player.Stop();
            IsPrepared = false;
        }

        public string Status()
        {
            if (player.IsPlaying)
                return $"{player.CurrentPosition} / {player.Duration}";

            return $"nada {player.IsPlaying.ToString()}";
        }

        public void ElapsedTimeChanged()
        {
            
        }
    }
}