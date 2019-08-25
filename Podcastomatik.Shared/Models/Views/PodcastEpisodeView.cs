using System;
using System.Collections.Generic;
using System.Text;

namespace Podcastomatik.Shared.Models.Views
{
    public class PodcastEpisodeView
    {
        public int Id { get; set; }
        public int PodcastId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Subtitle { get; set; }
        public string Summary { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public string TimeInfoBlock { get; set; }
        public DateTime PublishDateUtc { get; set; }
        public string Duration { get; set; }
        public string OriginalDuration { get; set; }
        public bool IsExplicit { get; set; }
        public string Keywords { get; set; }
    }
}
