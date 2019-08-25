using Podcastomatik.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Podcastomatik.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BottomSheet
    {
        //public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(BottomSheet), default(string), BindingMode.OneWay);
        //public string Title
        //{
        //    get => (string)GetValue(TitleProperty);
        //    set => SetValue(TitleProperty, value);
        //}

        public BottomSheet()
        {
            InitializeComponent();

            BindingContext = new BottomSheetViewModel();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}