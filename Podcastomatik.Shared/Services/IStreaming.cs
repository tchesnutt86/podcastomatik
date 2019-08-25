using System;

namespace Podcastomatik.Shared.Services
{
    public interface IStreaming
    {
        event EventHandler PlayerStarted;
        void Play(string uri);
        void Pause();
        void Stop();
        string Status();
    }
}
