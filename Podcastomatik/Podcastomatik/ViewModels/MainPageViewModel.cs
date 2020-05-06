using Podcastomatik.Shared;
using Podcastomatik.Shared.Models;
using Podcastomatik.Shared.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Environment;
using Xamarin.Essentials;
using Podcastomatik.Controls;

namespace Podcastomatik.ViewModels
{
    public class Welpers
    {
        public ImageSource ImgSrc { get; set; }
        public string Title { get; set; }
    }
    public class MainPageViewModel : BaseBindable
    {
        DbConnector db = new DbConnector();
        INavigation navigation;
        private Func<string, object> findByNameFunc;
        private Grid grdMyPodcastSubscriptions;
        private Label lblMyPodcastSubs;

        //public ObservableCollection<object> Podcasts { get; set; }
        private IEnumerable<Podcast> podcasts;
        public IEnumerable<Podcast> Podcasts
        {
            get => podcasts;
            set
            {
                podcasts = value;
                RaisePropertyChanged();
            }
        }
        private IEnumerable<Welpers> yoyo;
        public IEnumerable<Welpers> YoYo
        {
            get => yoyo;
            set
            {
                yoyo = value;
                RaisePropertyChanged();
            }
        }

        public MainPageViewModel(INavigation nav, Func<string, object> findByName)
        {
            navigation = nav;
            findByNameFunc = findByName;

            // Components...
            grdMyPodcastSubscriptions = (Grid)findByNameFunc("MyPodcastSubscriptionsGrid");
            lblMyPodcastSubs = (Label)findByNameFunc("MyPodcastSubsLabel");
            // load actual data. commented out tto mess with UI for now. uncomment later
            //GetPodcastsAndLocalizeImageUrls().ContinueWith(__result =>
            //{
            //    Podcasts = __result.Result.ToList();

            //    MainThread.BeginInvokeOnMainThread(() => InitializeMyPodcastSubscriptions());
            //});
        }

        private async Task<IEnumerable<Podcast>> GetPodcastsAndLocalizeImageUrls()
        {
            var results = await db.GetAsync<Podcast>("podcasts");
            string sandboxPath = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData), "images");
            if (!Directory.Exists(sandboxPath))
                Directory.CreateDirectory(sandboxPath);
            var fileNames = Directory.GetFiles(sandboxPath);

            var modifiedResults = results.AsParallel().Select((Podcast __item) =>
            {
                string fileAndPath = fileNames.FirstOrDefault(__fn => __fn.Split('\\').LastOrDefault() == __item.Id.ToString());

                if (!string.IsNullOrEmpty(fileAndPath))
                    __item.ImageUrl = fileAndPath;
                else
                {
                    try
                    {
                        using (var webClient = new WebClient())
                        {
                            byte[] arr = webClient.DownloadData(__item.ImageUrl);

                            Regex rx = new Regex(@"[^/\\&\?]+\.\w{3,4}(?=([\?&].*$|$))"); // Get file name and extension from url.
                            string newFileName = __item.Id.ToString() + "." + rx.Match(__item.ImageUrl).Value.Split('.')[1];
                            string newFilePath = Path.Combine(sandboxPath, newFileName);

                            File.WriteAllBytes(newFilePath, arr); 
                            
                            __item.ImageUrl = newFilePath;
                        }
                    }
                    catch
                    {
                        __item.ImageUrl = "";
                    }
                }

                return __item;
            });

            return modifiedResults;
        }

        private void InitLayout()
        {
            grdMyPodcastSubscriptions.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star),
            });
            grdMyPodcastSubscriptions.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star),
            });
            grdMyPodcastSubscriptions.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star),
            });
            grdMyPodcastSubscriptions.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star),
            });
            grdMyPodcastSubscriptions.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star),
            });

            grdMyPodcastSubscriptions.Children.Add(new BoxView { BackgroundColor = Color.Red }, 0, 0);
            grdMyPodcastSubscriptions.Children.Add(new BoxView { BackgroundColor = Color.Red }, 1, 0);
            grdMyPodcastSubscriptions.Children.Add(new BoxView { BackgroundColor = Color.Red }, 2, 0);
            grdMyPodcastSubscriptions.Children.Add(new BoxView { BackgroundColor = Color.Red }, 3, 0);
        }

        private void InitializeMyPodcastSubscriptions()
        {
            int itemsPerRow = 4;

            // 4 columns per row.
            grdMyPodcastSubscriptions.ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            };

            for (int t = 0; t < Podcasts.Count(); t++)
            {
                var podcastItem = Podcasts.ElementAt(t);

                if (t % itemsPerRow == 0)
                {
                    grdMyPodcastSubscriptions.RowDefinitions.Add(new RowDefinition
                    {
                        Height = new GridLength(1, GridUnitType.Auto),
                    });
                }

                ImageButton imgButton = new ImageButton { Source = GetImageSourceFromPath(podcastItem.ImageUrl) };
                imgButton.Clicked += (obj, eventArgs) => OnMySubscriptionsPodcastItemClicked(obj, eventArgs, podcastItem);

                grdMyPodcastSubscriptions.Children.Add(imgButton, t % itemsPerRow, grdMyPodcastSubscriptions.RowDefinitions.Count - 1);
            }

            lblMyPodcastSubs.Text = "My Podcast Subscriptions";
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

        //private ObservableCollection<object> GetPodcastsForListView()
        //{
        //    var stuff = db.Get<Podcast>("podcasts");
        //    var results = new List<object>();
        //    string sandboxPath = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData), "images");
        //    var fileNames = Directory.GetFiles(sandboxPath);

        //    foreach (var s in stuff)
        //    {
        //        string imagePath = "";
        //        string file = fileNames.FirstOrDefault(__fn => __fn.Split('\\').LastOrDefault() == s.Id.ToString());

        //        if (!string.IsNullOrEmpty(file))
        //            imagePath = file;
        //        else
        //        {
        //            try
        //            {
        //                byte[] arr = new WebClient().DownloadData(s.ImageUrl);

        //                Regex rx = new Regex(@"[^/\\&\?]+\.\w{3,4}(?=([\?&].*$|$))"); // Get file name and extension from url.
        //                string newFileName = s.id.ToString() + "." + rx.Match(s.ImageUrl).Value.Split('.')[1];
        //                string newFilePath = Path.Combine(sandboxPath, newFileName);

        //                File.WriteAllBytes(newFilePath, arr);

        //                imagePath = newFilePath;
        //            }
        //            catch { }
        //        }

        //        byte[] finalSource = null;

        //        if (!string.IsNullOrEmpty(imagePath))
        //        {
        //            switch (Device.RuntimePlatform)
        //            {
        //                case Device.Android:
        //                    finalSource = MediaService.ResizeImageAndroid(File.ReadAllBytes(imagePath), 128, 128);
        //                    break;
        //                case Device.iOS:
        //                    finalSource = MediaService.ResizeImageIOS(File.ReadAllBytes(imagePath), 128, 128);
        //                    break;
        //                default:
        //                    finalSource = null; // set to a default image that I make custom
        //                    break;
        //            } 
        //        }

        //        results.Add(new
        //        {
        //            s.title,
        //            image_url = ImageSource.FromStream(() => new MemoryStream(finalSource)),
        //        });
        //    }

        //    return new ObservableCollection<object>(results);
        //}


        void OnMySubscriptionsPodcastItemClicked(object sender, EventArgs e, Podcast podcast)
        {
            navigation.PushAsync(new PodcastPage(podcast));
        }
    }
}
