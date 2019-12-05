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
        public IEnumerable<Welpers> yoyo { get; set; }
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

            GetPodcastsAndLocalizeImageUrls().ContinueWith(__result =>
            {
                Podcasts = __result.Result;

                //InitializeMyPodcastSubscriptions();
                InitLayout();
            });
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
            StackLayout sl = (StackLayout)findByNameFunc("MyPodcastSubscriptionsLayout");

            // add kiddos
            StackLayout sl1 = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 100,
                BackgroundColor = Color.Brown,
            };
            sl1.Children.Add(new ImageButton { Source = GetImageSourceFromPath(Podcasts.ElementAt(0).ImageUrl) });
            sl1.Children.Add(new ImageButton { Source = GetImageSourceFromPath(Podcasts.ElementAt(1).ImageUrl) });

            StackLayout sl2 = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
            };
            sl2.Children.Add(new ImageButton { Source = GetImageSourceFromPath(Podcasts.ElementAt(2).ImageUrl) });
            sl2.Children.Add(new ImageButton { Source = GetImageSourceFromPath(Podcasts.ElementAt(3).ImageUrl) });

            sl.Children.Add(sl1);
            sl.Children.Add(sl2);
        }

        private void InitializeMyPodcastSubscriptions()
        {
            // Hard coding for now, but later need to calculate rows/cols.
            int rows = 2;
            int columns = 4;
            
            Grid grid = (Grid)findByNameFunc("MyPodcastSubscriptionsGrid");
            for (int x = 0; x < rows; x++)
                grid.RowDefinitions.Add(new RowDefinition());
            for (int y = 0; y < columns; y++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.BackgroundColor = Color.AliceBlue;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int index = ((row + 1) * (col + 1)) - 1;
                    var podcastItem = Podcasts.ElementAt(index);
                    var src = GetImageSourceFromPath(podcastItem.ImageUrl);
                    ImageButton imgButton = new ImageButton { Source = src };
                    imgButton.Clicked += (obj, eventArgs) => OnMySubscriptionsPodcastItemClicked(obj, eventArgs, podcastItem);
                    //Image image = new Image() { Source = src };

                    grid.Children.Add(imgButton, col, row);
                }
            }
            //StackLayout stackLayout = (StackLayout)findByNameFunc("MyPodcastSubscriptionsLayout");

            //for (int i = 0; i < 4; i++)
            //{
            //    var src = GetImageSourceFromPath(Podcasts.ElementAt(i).ImageUrl);
            //    Image image = new Image() { Source = src };

            //    stackLayout.Children.Add(image);
            //}

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
