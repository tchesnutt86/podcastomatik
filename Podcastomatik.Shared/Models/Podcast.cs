namespace Podcastomatik.Shared.Models
{
    public struct Podcast
    {
        public int Id { get; set; }
        public int ApplePodcastId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PodcastFeed { get; set; }
        public string Description { get; set; }
        public int EpisodeCount { get; set; }
        public string Homepage { get; set; }
        public string Keywords { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public string Language { get; set; }
    }
}
