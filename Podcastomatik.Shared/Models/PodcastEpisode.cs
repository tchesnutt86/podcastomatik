using System;

namespace Podcastomatik.Shared.Models
{
    public struct PodcastEpisode
    {
        public int Id { get; set; }
        public int PodcastId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Subtitle { get; set; }
        public string Summary { get; set; }
        public string MediaUrl { get; set; }
        public string MediaType { get; set; }
        public DateTime PublishDateUtc { get; set; }
        public string Duration { get; set; }
        public bool IsExplicit { get; set; }
        public string Keywords { get; set; }
    }
}
