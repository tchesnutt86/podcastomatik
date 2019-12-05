using Podcastomatik.Services;
using Podcastomatik.Shared;
using Podcastomatik.Shared.Extensions;
using Podcastomatik.Shared.Helpers;
using Podcastomatik.Shared.Models;
using Podcastomatik.Shared.Models.Views;
using Podcastomatik.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Podcastomatik.ViewModels
{
    public class PodcastPageViewModel : BaseBindable
    {
        private readonly DbConnector db = new DbConnector();
        private IEnumerable<PodcastEpisodeView> podcastEpisodesOriginal;
        private IEnumerable<PodcastEpisodeView> podcastEpisodes;
        private string podcastFilter;

        public ImageSource PodcastImageSource { get; set; }
        public Podcast Podcast { get; set; }
        public IEnumerable<PodcastEpisodeView> PodcastEpisodes
        {
            get => podcastEpisodes;
            set
            {
                podcastEpisodes = value;
                RaisePropertyChanged();
            }
        }
        public string PodcastFilter
        {
            get => podcastFilter;
            set
            {
                podcastFilter = value;

                if (string.IsNullOrEmpty(podcastFilter))
                    PodcastEpisodes = podcastEpisodesOriginal;
                else
                    PodcastEpisodes = podcastEpisodesOriginal.Where(__episode => __episode.Title.CaseInsensitiveContains(podcastFilter));
            }
        }

        public PodcastPageViewModel(Podcast podcast)
        {
            PodcastImageSource = GetImageSourceFromPath(podcast.ImageUrl);
            //PodcastTitle = podcast.Title;
            Podcast = podcast;

            LoadEpisodes().ContinueWith(__x => IsWorking = false);
        }

        private async Task LoadEpisodes()
        {
            IsWorking = true;

            var results = (await db.GetAsync<PodcastEpisode>($"podcasts/{Podcast.Id}/episodes")).Take(20);

            var convertedEpisodes = results.Select(__episode => new PodcastEpisodeView
            {
                Author = __episode.Author,
                Duration = GetDurationStrMinutes(__episode.Duration),
                Id = __episode.Id,
                IsExplicit = __episode.IsExplicit,
                Keywords = __episode.Keywords,
                MediaType = __episode.MediaType,
                MediaUrl = __episode.MediaUrl,
                PodcastId = __episode.PodcastId,
                Subtitle = __episode.Subtitle,
                Summary = __episode.Summary,
                PublishDateUtc = __episode.PublishDateUtc,
                TimeInfoBlock = GetTimeInfoBlock(__episode.PublishDateUtc, __episode.Duration),
                OriginalDuration = __episode.Duration,
                Title = __episode.Title,
            });

            podcastEpisodesOriginal = convertedEpisodes;
            PodcastEpisodes = convertedEpisodes;
        }

        private string GetTimeInfoBlock(DateTime publishDateUtc, string duration)
        {
            return $"{GetTimeSincePublished(publishDateUtc)} | {GetDurationStrMinutes(duration)}";
        }

        private string GetTimeSincePublished(DateTime publishDateUtc)
        {
            TimeSpan timeSince = DateTime.UtcNow - publishDateUtc;

            if (timeSince.TotalHours < 1)
            {
                return $"{Math.Round(timeSince.TotalMinutes)} minutes ago";
            }
            else if (timeSince.TotalHours < 24)
            {
                return $"{Math.Round(timeSince.TotalHours)} hours ago";
            }
            else if (timeSince.TotalDays < 7)
            {
                return $"{Math.Round(timeSince.TotalDays)} days ago";
            }
            else if (publishDateUtc.Year == DateTime.UtcNow.Year)
            {
                return $"{publishDateUtc.ToString("MMM dd")}";
            }
            else
            {
                return $"{publishDateUtc.ToString("MMM dd, yyyy")}";
            }
        }

        private string GetDurationStrMinutes(string duration)
        {
            return $"{Math.Round(Utilities.GetTimeSpanFromDuration(duration).TotalMinutes)} min";
        }

        private ImageSource GetImageSourceFromPath(string path)
        {
            byte[] source = null;

            if (!string.IsNullOrEmpty(path))
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        source = MediaService.ResizeImageAndroid(File.ReadAllBytes(path), 300, 300);
                        break;
                    case Device.iOS:
                        source = MediaService.ResizeImageIOS(File.ReadAllBytes(path), 300, 300);
                        break;
                    default:
                        source = null; // set to a default image that I make custom
                        break;
                }
            }

            return ImageSource.FromStream(() => new MemoryStream(source));
        }

        void OnEpisodeSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        //private void PlayPauseClicked(object sender, EventArgs e)
        //{
        //    Button btn = sender as Button;

        //    btn.Text = "||";
        //}

        string FormatTime(double time)
        {
            double minutes = time / 60;
            double seconds = time % 60;

            return string.Format("{0}:{1:D2}", (int)minutes, (int)seconds);
        }
    }
}
