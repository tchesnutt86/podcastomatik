using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Podcastomatik.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainActionBar
    {
        public MainActionBar()
        {
            InitializeComponent();

            BindingContext = new MainActionBarViewModel(FindByName, Navigation);
        }

        private void lvSearchResults_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Debug.WriteLine("Tapped: " + e.Item);
        }
    }
}