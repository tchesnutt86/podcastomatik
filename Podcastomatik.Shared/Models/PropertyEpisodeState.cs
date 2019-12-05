using System;
using System.Collections.Generic;
using System.Text;

namespace Podcastomatik.Shared.Models
{
    public class PropertyEpisodeState
    {
        public int? EpisodeId { get; set; }
        public string EpisodeTitle { get; set; }
        public string EpisodeUrl { get; set; }
        public int? ElapsedSeconds { get; set; }
        public int? TotalDurationSeconds { get; set; }
        public string FormattedDuration { get; set; }
        public bool IsPlaying { get; set; }
    }
}
