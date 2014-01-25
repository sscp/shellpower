using System;
using OpenTK;

namespace SSCP.ShellPower {

    /// <summary>
    /// API for getting place names and timezones given lat,lon.
    /// </summary>
    public class GeoNames {

        /// <summary>
        /// Example JSON: 
        /// {"time":"2010-09-24 13:48","countryName":"Australia","sunset":"2010-09-25 18:23",
        /// "rawOffset":9.5,"dstOffset":9.5,"countryCode":"AU","gmtOffset":9.5,
        /// "lng":135.5,"sunrise":"2010-09-25 06:16","timezoneId":"Australia/Darwin","lat":-12.5}
        /// </summary>
        public class TimezoneResponse {
            public string Status { get; set; }
            public double Lng { get; set; }
            public double Lat { get; set; }
            public float GmtOffset { get; set; }

            public DateTime Time { get; set; }
            public string CountryName { get; set; }
            public DateTime Sunset { get; set; }
            public DateTime Sunrise { get; set; }
            public string TimezoneId { get; set; }
        }

        /* LRU cache */
        LruCache<TimezoneResponse, Vector2d> cache = new LruCache<TimezoneResponse, Vector2d>(tz => new Vector2d(tz.Lat, tz.Lng));

        public float GetTimezone(double lat, double lng) {
            //var tz = GetTimezoneResponse(lat, lng);
            //return tz.GmtOffset;
            return 9.5f;
        }
        public string GetTimezoneName(double lat, double lng) {
            //var tz = GetTimezoneResponse(lat, lng);
            return "Northern Territories"; // tz.TimezoneId;
        }
        public DateTime GetSunrise(double lat, double lng) {
            var tz = GetTimezoneResponse(lat, lng);
            return tz.Sunrise;
        }
        public DateTime GetSunset(double lat, double lng) {
            var tz = GetTimezoneResponse(lat, lng);
            return tz.Sunset;
        }
        public TimezoneResponse GetTimezoneResponse(double lat, double lng) {
            if (cache.ContainsKey(new Vector2d(lat, lng))) {
                return cache[new Vector2d(lat, lng)];
            } else {
                string url = "http://ws.geonames.org/timezoneJSON?lat=" + lat + "&lng=" + lng;
                var tz = Web.GetJson<TimezoneResponse>(url);
                cache.Store(tz);
                return tz;
            }
        }
    }
}
