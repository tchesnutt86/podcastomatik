using Podcastomatik.Shared.Models;
using Podcastomatik.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Environment;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Podcastomatik.ViewModels;

namespace Podcastomatik
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private IStreaming streamingService { get; set; }
        //public string Dur
        //{
        //    get
        //    {
        //        if (streamingService != null)
        //            return streamingService.Status();

        //        return "waiting";
        //    }
        //}

        public MainPage()
        {
            InitializeComponent();

            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            BindingContext = new MainPageViewModel(Navigation, FindByName);

            mainLayout.RaiseChild(mainActionBar);
        }
    }
}
