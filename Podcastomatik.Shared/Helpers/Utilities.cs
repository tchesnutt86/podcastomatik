using System;
using System.Collections.Generic;
using System.Text;

namespace Podcastomatik.Shared.Helpers
{
    public static class Utilities
    {
        public static TimeSpan GetTimeSpanFromDuration(string duration)
        {
            string fixedDuration = duration;
            TimeSpan results;

            if (duration.Split(':').Length == 2)
                TimeSpan.TryParse($"00:{duration}", out results);
            else if (duration.Split(':').Length == 1)
                results = TimeSpan.FromSeconds(Math.Round(Convert.ToDouble(duration)));
            //fixedDuration = $"00:{Math.Round(Convert.ToDouble(duration))}:00";

            return results;
        }
    }
}
