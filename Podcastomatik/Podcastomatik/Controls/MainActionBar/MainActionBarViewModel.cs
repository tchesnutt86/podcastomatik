using Podcastomatik.Shared;
using Podcastomatik.Shared.Models;
using Podcastomatik.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Podcastomatik.Controls
{
    public class MainActionBarViewModel : BaseBindable
    {
        private readonly INavigation _navigation;
        private DbConnector db = new DbConnector();
        private Entry entSearch;
        private ListView lvSearchResults;
        //private AbsoluteLayout alSearch;

        private IEnumerable<PodcastSearchResult> searchResults;
        public IEnumerable<PodcastSearchResult> SearchResults
        {
            get => searchResults;
            set
            {
                searchResults = value;
                RaisePropertyChanged();
            }
        }

        public MainActionBarViewModel(Func<string, object> findByName, INavigation navigation)
        {
            _navigation = navigation;
            entSearch = (Entry)findByName("entSearch");
            lvSearchResults = (ListView)findByName("lvSearchResults");
            //alSearch = (AbsoluteLayout)findByName("alSearch");

            //AbsoluteLayout.SetLayoutBounds(lvSearchResults, new Rectangle(
            //    entSearch.X,
            //    entSearch.Y + entSearch.Height,
            //    entSearch.Width,
            //    lvSearchResults.Height));

            SearchResults = new List<PodcastSearchResult>
            {
                new PodcastSearchResult { Id = 0, Title = "result 1" },
                new PodcastSearchResult { Id = 0, Title = "result 2" },
                new PodcastSearchResult { Id = 0, Title = "result 3" },
                new PodcastSearchResult { Id = 0, Title = "result 4" },
                new PodcastSearchResult { Id = 0, Title = "result 5" },
            };

            //lvSearchResults.HeightRequest = SearchResults.Count() * 35;

            //entSearch.TextChanged += entSearch_TextChanged;
        }

        private void entSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // search and modify listview
            //db.Search(e.NewTextValue).ContinueWith(result =>
            //{
            //    MainThread.BeginInvokeOnMainThread(() =>
            //    {
            //        SearchResults = result.Result;
            //    });
            //});
        }
    }
}
