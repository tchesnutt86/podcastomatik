using System;
using System.Collections.Generic;
using System.Text;

namespace Podcastomatik.Shared.Services
{
    public class AudioPlayer
    {
        public bool IsPlaying { get; set; } = false;

        public AudioPlayer() { }

        public void Play()
        {
            IsPlaying = true;
        }


    }
}
