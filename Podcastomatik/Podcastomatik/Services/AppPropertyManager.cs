using Newtonsoft.Json;
using Podcastomatik.MessageMarkers;
using Podcastomatik.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Podcastomatik.Services
{
    public static class AppPropertyManager
    {
        enum ResourceKeys
        {
            EpisodeState,
        }

        public static PropertyEpisodeState EpisodeState
        {
            get
            {
                object storedObj;

                if (!Application.Current.Properties.TryGetValue(nameof(ResourceKeys.EpisodeState), out storedObj))
                    return null;

                return JsonConvert.DeserializeObject<PropertyEpisodeState>(storedObj.ToString());
            }
            set
            {
                Application.Current.Properties[nameof(ResourceKeys.EpisodeState)] = JsonConvert.SerializeObject(value);

                MessagingCenter.Send(new ResourcePropertyChangedMessage(), App.RESOURCE_PROPERTY_CHANGED);
            }
        }
    }
}
