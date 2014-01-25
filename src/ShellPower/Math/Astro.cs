using System;

namespace SSCP.ShellPower {
    /// <summary>
    /// Astronomy constants and calculation methods.
    /// 
    /// For example, these can tell you where the sun is in the sky at a given (lat, lon, datetime).
    /// </summary>
    class Astro {
        public const double DECLINATION_DEG = 23.44;
        public const int WINTER_SOLSTICE_DOY = 355;
        public const int SUMMER_SOLSTICE_DOY = 172;
        public const int EQUINOX_DOY = 284;

        public static double sin(double theta) {
            return Math.Sin(theta);
        }
        public static double cos(double theta) {
            return Math.Cos(theta);
        }
        public static double deg2rad(double deg) {
            return deg / 180.0 * Math.PI;
        }
        public static double rad2deg(double rad) {
            return rad * 180.0 / Math.PI;
        }
        public static void solar_declination_hour_angle(int timeofday, int dayofyear, out double h, out double delta) {
            h = deg2rad((double)((timeofday - 12 * 60 * 60) * 360 / (24 * 60 * 60)));
            delta =
                    deg2rad(DECLINATION_DEG)
                    * sin(deg2rad((double)360 / (double)365 * ((double)dayofyear + EQUINOX_DOY)));
        }
        public static double solar_elevation(int timeofday /* in seconds */, int dayofyear, double latitude) {
            double delta, h;
            solar_declination_hour_angle(timeofday, dayofyear, out h, out delta);
            return Math.Asin(cos(h) * cos(delta) * cos(deg2rad(latitude))
                        + sin(delta) * sin(deg2rad(latitude)));
        }
        public static double solar_azimuth(int timeofday /* in seconds */, int dayofyear, double latitude) {
            double delta, h;
            solar_declination_hour_angle(timeofday, dayofyear, out h, out delta);
            double phi = Math.Asin(-sin(h) * cos(delta) / cos(solar_elevation(timeofday, dayofyear, latitude)));

            //asin has two solutions. to disambiguate,
            //just check if the sun is north or south of the cur position
            if (latitude > delta)
                phi += Math.PI;
            return phi;
        }
        public static DateTime sidereal_time(DateTime utc, double longitude) {
            return utc + new TimeSpan((long)(longitude * 24.0 * 60 * 60 * 10000000 / 360));
        }


    }
}
