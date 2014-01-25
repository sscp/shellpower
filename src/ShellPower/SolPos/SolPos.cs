using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSCP.ShellPower {

    /// <summary>
    ///     S_solpos   (computes solar position and intensity from 
    ///                 time and place.)
    /// 
    ///         INPUTS:     (via posdata struct) year, daynum, hour,
    ///                     minute, second, latitude, longitude, timezone,
    ///                     intervl
    ///         OPTIONAL:   (via posdata struct) month, day, press, temp, tilt,
    ///                     aspect, function
    ///         OUTPUTS:    EVERY variable in the struct posdata
    ///                         (defined in solpos.h)
    /// 
    ///                    NOTE: Certain conditions exist during which some of
    ///                    the output variables are undefined or cannot be
    ///                    calculated.  In these cases, the variables are
    ///                    returned with flag values indicating such.  In other
    ///                    cases, the variables may return a realistic, though
    ///                    invalid, value. These variables and the flag values
    ///                    or invalid conditions are listed below:
    /// 
    ///                    amass     -1.0 at zenetr angles greater than 93.0
    ///                              degrees
    ///                    ampress   -1.0 at zenetr angles greater than 93.0
    ///                              degrees
    ///                    azim      invalid at zenetr angle 0.0 or latitude
    ///                              +/-90.0 or at night
    ///                    elevetr   limited to -9 degrees at night
    ///                    etr       0.0 at night
    ///                    etrn      0.0 at night
    ///                    etrtilt   0.0 when cosinc is less than 0
    ///                    prime     invalid at zenetr angles greater than 93.0
    ///                              degrees
    ///                    sretr     +/- 2999.0 during periods of 24 hour sunup or
    ///                              sundown
    ///                    ssetr     +/- 2999.0 during periods of 24 hour sunup or
    ///                              sundown
    ///                    ssha      invalid at the North and South Poles
    ///                    unprime   invalid at zenetr angles greater than 93.0
    ///                              degrees
    ///                    zenetr    limited to 99.0 degrees at night
    /// 
    ///     S_init     (optional initialization for all input parameters in
    ///                 the posdata struct)
    ///        INPUTS:     struct posdata*
    ///        OUTPUTS:    struct posdata*
    /// 
    ///                  (Note: initializes the required S_solpos INPUTS above
    ///                   to out-of-bounds conditions, forcing the user to
    ///                   supply the parameters; initializes the OPTIONAL
    ///                   S_solpos inputs above to nominal values.)
    /// 
    ///    S_decode    (optional utility for decoding the S_solpos return code)
    ///        INPUTS:     long integer S_solpos return value, struct posdata*
    ///        OUTPUTS:    text to stderr
    /// 
    /// Usage:
    ///      In calling program, just after other 'includes', insert:
    /// 
    ///           #include "solpos00.h"
    /// 
    ///      Function calls:
    ///           S_init(struct posdata*)  [optional]
    ///           .
    ///           .
    ///           [set time and location parameters before S_solpos call]
    ///           .
    ///           .
    ///           int retval = S_solpos(struct posdata*)
    ///           S_decode(int retval, struct posdata*) [optional]
    ///              (Note: you should always look at the S_solpos return
    ///               value, which contains error codes. S_decode is one option
    ///               for examining these codes.  It can also serve as a
    ///               template for building your own application-specific
    ///               decoder.)
    /// 
    /// Martin Rymes
    /// National Renewable Energy Laboratory
    /// 25 March 1998
    /// 
    /// 27 April 1999 REVISION:  Corrected leap year in S_date.
    /// 13 January 2000 REVISION:  SMW converted to structure posdata parameter
    ///                            and subdivided into functions.
    /// 01 February 2001 REVISION: SMW corrected ecobli calculation 
    ///                            (changed sign). Error is small (max 0.015 deg
    ///                            in calculation of declination angle)
    /// </summary>
    public class SolPos {

        /*++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        *
        * Structures defined for this module
        *
        *++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/
        private class TrigData /* used to pass calculated values locally */
        {
            public float cd;       /* cosine of the declination */
            public float ch;       /* cosine of the hour angle */
            public float cl;       /* cosine of the latitude */
            public float sd;       /* sine of the declination */
            public float sl;       /* sine of the latitude */
        };

        /*++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        *
        * Temporary global variables used only in this file:
        *
        *++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++*/

        /* cumulative number of days prior to beginning of month */
        private static readonly int[][] month_days = { 
            new int[] { 0,   0,  31,  59,  90, 120, 151,
              181, 212, 243, 273, 304, 334 },
            new int[] { 0,   0,  31,  60,  91, 121, 152,
              182, 213, 244, 274, 305, 335 }};

        /* converts from radians to degrees */
        private const float degrad = 57.295779513f;
        /* converts from degrees to radians */
        private const float raddeg = 0.0174532925f;

        /*============================================================================
        *
        * Constants: define the function codes
        *
        *----------------------------------------------------------------------------*/
        public const int L_DOY = 0x0001;
        public const int L_GEOM = 0x0002;
        public const int L_ZENETR = 0x0004;
        public const int L_SSHA = 0x0008;
        public const int L_SBCF = 0x0010;
        public const int L_TST = 0x0020;
        public const int L_SRSS = 0x0040;
        public const int L_SOLAZM = 0x0080;
        public const int L_REFRAC = 0x0100;
        public const int L_AMASS = 0x0200;
        public const int L_PRIME = 0x0400;
        public const int L_TILT = 0x0800;
        public const int L_ETR = 0x1000;
        public const int L_ALL = 0xFFFF;

        /*============================================================================
        *
        *     Define the bit-wise masks for each function
        *
        *----------------------------------------------------------------------------*/
        public const int S_DOY = (L_DOY);
        public const int S_GEOM = (L_GEOM | S_DOY);
        public const int S_ZENETR = (L_ZENETR | S_GEOM);
        public const int S_SSHA = (L_SSHA | S_GEOM);
        public const int S_SBCF = (L_SBCF | S_SSHA);
        public const int S_TST = (L_TST | S_GEOM);
        public const int S_SRSS = (L_SRSS | S_SSHA | S_TST);
        public const int S_SOLAZM = (L_SOLAZM | S_ZENETR);
        public const int S_REFRAC = (L_REFRAC | S_ZENETR);
        public const int S_AMASS = (L_AMASS | S_REFRAC);
        public const int S_PRIME = (L_PRIME | S_AMASS);
        public const int S_TILT = (L_TILT | S_SOLAZM | S_REFRAC);
        public const int S_ETR = (L_ETR | S_REFRAC);
        public const int S_ALL = (L_ALL);


        /*============================================================================
        *
        *     Enumerate the error codes
        *     (Bit positions are from least significant to most significant)
        *
        *----------------------------------------------------------------------------*/
        /*                      Code              Bit      Parameter            Range
                                ===============   ===      ===================  =============   */
        public const int S_YEAR_ERROR = 0; /*   year                  1950 -  2050   */
        public const int S_MONTH_ERROR = 1; /*   month                    1 -    12   */
        public const int S_DAY_ERROR = 2; /*   day-of-month             1 -    31   */
        public const int S_DOY_ERROR = 3; /*   day-of-year              1 -   366   */
        public const int S_HOUR_ERROR = 4; /*   hour                     0 -    24   */
        public const int S_MINUTE_ERROR = 5; /*   minute                   0 -    59   */
        public const int S_SECOND_ERROR = 6; /*   second                   0 -    59   */
        public const int S_TZONE_ERROR = 7; /*   time zone              -12 -    12   */
        public const int S_INTRVL_ERROR = 8; /*   interval (seconds)       0 - 28800   */
        public const int S_LAT_ERROR = 9; /*   latitude               -90 -    90   */
        public const int S_LON_ERROR = 10; /*   longitude             -180 -   180   */
        public const int S_TEMP_ERROR = 11; /*   temperature (deg. C)  -100 -   100   */
        public const int S_PRESS_ERROR = 12; /*   pressure (millibars)     0 -  2000   */
        public const int S_TILT_ERROR = 13; /*   tilt                   -90 -    90   */
        public const int S_ASPECT_ERROR = 14; /*   aspect                -360 -   360   */
        public const int S_SBWID_ERROR = 15; /*   shadow band width (cm)   1 -   100   */
        public const int S_SBRAD_ERROR = 16; /*   shadow band radius (cm)  1 -   100   */
        public const int S_SBSKY_ERROR = 17; /*   shadow band sky factor  -1 -     1   */


        /*============================================================================
        *    Long integer function S_solpos, adapted from the VAX solar libraries
        *
        *    This function calculates the apparent solar position and the
        *    intensity of the sun (theoretical maximum solar energy) from
        *    time and place on Earth.
        *
        *    Requires (from the struct posdata parameter):
        *        Date and time:
        *            year
        *            daynum   (requirement depends on the S_DOY switch)
        *            month    (requirement depends on the S_DOY switch)
        *            day      (requirement depends on the S_DOY switch)
        *            hour
        *            minute
        *            second
        *            interval  DEFAULT 0
        *        Location:
        *            latitude
        *            longitude
        *        Location/time adjuster:
        *            timezone
        *        Atmospheric pressure and temperature:
        *            press     DEFAULT 1013.0 mb
        *            temp      DEFAULT 10.0 degrees C
        *        Tilt of flat surface that receives solar energy:
        *            aspect    DEFAULT 180 (South)
        *            tilt      DEFAULT 0 (Horizontal)
        *        Function Switch (codes defined in solpos.h)
        *            function  DEFAULT S_ALL
        *
        *    Returns (via the struct posdata parameter):
        *        everything defined in the struct posdata in solpos.h.
        *----------------------------------------------------------------------------*/
        public static long S_solpos(PosData pdat) {
            long retval;

            TrigData trigdat = new TrigData();

            /* initialize the trig structure */
            trigdat.sd = -999.0f; /* flag to force calculation of trig data */
            trigdat.cd = 1.0f;
            trigdat.ch = 1.0f; /* set the rest of these to something safe */
            trigdat.cl = 1.0f;
            trigdat.sl = 1.0f;

            if ((retval = validate(pdat)) != 0) { /* validate the inputs */
                return retval;
            }


            if ((pdat.function & L_DOY) != 0) {
                doy2dom(pdat);                 /* convert input doy to month-day */
            } else {
                dom2doy(pdat);                 /* convert input month-day to doy */
            }

            if ((pdat.function & L_GEOM) != 0) {
                geometry(pdat);                /* do basic geometry calculations */
            }

            if ((pdat.function & L_ZENETR) != 0) { /* etr at non-refracted zenith angle */
                zen_no_ref(pdat, trigdat);
            }

            if ((pdat.function & L_SSHA) != 0) {   /* Sunset hour calculation */
                ssha(pdat, trigdat);
            }

            if ((pdat.function & L_SBCF) != 0) {   /* Shadowband correction factor */
                sbcf(pdat, trigdat);
            }

            if ((pdat.function & L_TST) != 0) {    /* true solar time */
                tst(pdat);
            }

            if ((pdat.function & L_SRSS) != 0) {   /* sunrise/sunset calculations */
                srss(pdat);
            }

            if ((pdat.function & L_SOLAZM) != 0) { /* solar azimuth calculations */
                sazm(pdat, trigdat);
            }

            if ((pdat.function & L_REFRAC) != 0) { /* atmospheric refraction calculations */
                refrac(pdat);
            }

            if ((pdat.function & L_AMASS) != 0) {  /* airmass calculations */
                amass(pdat);
            }

            if ((pdat.function & L_PRIME) != 0) {  /* kt-prime/unprime calculations */
                prime(pdat);
            }

            if ((pdat.function & L_ETR) != 0) {    /* ETR and ETRN (refracted) */
                etr(pdat);
            }

            if ((pdat.function & L_TILT) != 0) {   /* tilt calculations */
                tilt(pdat);
            }

            return 0;
        }


        /*============================================================================
        *    Void function S_init
        *
        *    This function initiates all of the input parameters in the struct
        *    posdata passed to S_solpos().  Initialization is either to nominal
        *    values or to out of range values, which forces the calling program to
        *    specify parameters.
        *
        *    NOTE: This function is optional if you initialize ALL input parameters
        *          in your calling code.  Note that the required parameters of date
        *          and location are deliberately initialized out of bounds to force
        *          the user to enter real-world values.
        *
        *    Requires: Pointer to a posdata structure, members of which are
        *           initialized.
        *
        *    Returns: Void
        *----------------------------------------------------------------------------*/
        public static void S_init(PosData pdat) {
            pdat.day = -99;   /* Day of month (May 27 = 27, etc.) */
            pdat.daynum = -999;   /* Day number (day of year; Feb 1 = 32 ) */
            pdat.hour = -99;   /* Hour of day, 0 - 23 */
            pdat.minute = -99;   /* Minute of hour, 0 - 59 */
            pdat.month = -99;   /* Month number (Jan = 1, Feb = 2, etc.) */
            pdat.second = -99;   /* Second of minute, 0 - 59 */
            pdat.year = -99;   /* 4-digit year */
            pdat.interval = 0;   /* instantaneous measurement interval */
            pdat.aspect = 180.0f;   /* Azimuth of panel surface (direction it
                                    faces) N=0, E=90, S=180, W=270 */
            pdat.latitude = -99.0f;   /* Latitude, degrees north (south negative) */
            pdat.longitude = -999.0f;   /* Longitude, degrees east (west negative) */
            pdat.press = 1013.0f;   /* Surface pressure, millibars */
            pdat.solcon = 1367.0f;   /* Solar constant, 1367 W/sq m */
            pdat.temp = 15.0f;   /* Ambient dry-bulb temperature, degrees C */
            pdat.tilt = 0.0f;   /* Degrees tilt from horizontal of panel */
            pdat.timezone = -99.0f;   /* Time zone, east (west negative). */
            pdat.sbwid = 7.6f;   /* Eppley shadow band width */
            pdat.sbrad = 31.7f;   /* Eppley shadow band radius */
            pdat.sbsky = 0.04f;   /* Drummond factor for partly cloudy skies */
            pdat.function = S_ALL;   /* compute all parameters */
        }


        /*============================================================================
        *    Local long int function validate
        *
        *    Validates the input parameters
        *----------------------------------------------------------------------------*/
        private static long validate(PosData pdat) {

            long retval = 0;  /* start with no errors */

            /* No absurd dates, please. */
            if ((pdat.function & L_GEOM) != 0) {
                if ((pdat.year < 1950) || (pdat.year > 2050)) /* limits of algoritm */
                    retval |= (1L << S_YEAR_ERROR);
                if ((pdat.function & S_DOY) == 0 && ((pdat.month < 1) || (pdat.month > 12)))
                    retval |= (1L << S_MONTH_ERROR);
                if ((pdat.function & S_DOY) == 0 && ((pdat.day < 1) || (pdat.day > 31)))
                    retval |= (1L << S_DAY_ERROR);
                if ((pdat.function & S_DOY) != 0 && ((pdat.daynum < 1) || (pdat.daynum > 366)))
                    retval |= (1L << S_DOY_ERROR);

                /* No absurd times, please. */
                if ((pdat.hour < 0) || (pdat.hour > 24))
                    retval |= (1L << S_HOUR_ERROR);
                if ((pdat.minute < 0) || (pdat.minute > 59))
                    retval |= (1L << S_MINUTE_ERROR);
                if ((pdat.second < 0) || (pdat.second > 59))
                    retval |= (1L << S_SECOND_ERROR);
                if ((pdat.hour == 24) && (pdat.minute > 0)) /* no more than 24 hrs */
                    retval |= ((1L << S_HOUR_ERROR) | (1L << S_MINUTE_ERROR));
                if ((pdat.hour == 24) && (pdat.second > 0)) /* no more than 24 hrs */
                    retval |= ((1L << S_HOUR_ERROR) | (1L << S_SECOND_ERROR));
                if (fabs(pdat.timezone) > 12.0)
                    retval |= (1L << S_TZONE_ERROR);
                if ((pdat.interval < 0) || (pdat.interval > 28800))
                    retval |= (1L << S_INTRVL_ERROR);

                /* No absurd locations, please. */
                if (fabs(pdat.longitude) > 180.0)
                    retval |= (1L << S_LON_ERROR);
                if (fabs(pdat.latitude) > 90.0)
                    retval |= (1L << S_LAT_ERROR);
            }

            /* No silly temperatures or pressures, please. */
            if ((pdat.function & L_REFRAC) != 0 && (fabs(pdat.temp) > 100.0))
                retval |= (1L << S_TEMP_ERROR);
            if ((pdat.function & L_REFRAC) != 0 &&
              (pdat.press < 0.0) || (pdat.press > 2000.0))
                retval |= (1L << S_PRESS_ERROR);

            /* No out of bounds tilts, please */
            if ((pdat.function & L_TILT) != 0 && (fabs(pdat.tilt) > 180.0))
                retval |= (1L << S_TILT_ERROR);
            if ((pdat.function & L_TILT) != 0 && (fabs(pdat.aspect) > 360.0))
                retval |= (1L << S_ASPECT_ERROR);

            /* No oddball shadowbands, please */
            if ((pdat.function & L_SBCF) != 0 &&
                 (pdat.sbwid < 1.0) || (pdat.sbwid > 100.0))
                retval |= (1L << S_SBWID_ERROR);
            if ((pdat.function & L_SBCF) != 0 &&
                 (pdat.sbrad < 1.0) || (pdat.sbrad > 100.0))
                retval |= (1L << S_SBRAD_ERROR);
            if ((pdat.function & L_SBCF) != 0 && (fabs(pdat.sbsky) > 1.0))
                retval |= (1L << S_SBSKY_ERROR);

            return retval;
        }


        /*============================================================================
        *    Local Void function dom2doy
        *
        *    Converts day-of-month to day-of-year
        *
        *    Requires (from struct posdata parameter):
        *            year
        *            month
        *            day
        *
        *    Returns (via the struct posdata parameter):
        *            year
        *            daynum
        *----------------------------------------------------------------------------*/
        private static void dom2doy(PosData pdat) {
            pdat.daynum = pdat.day + month_days[0][pdat.month];

            /* (adjust for leap year) */
            if (((pdat.year % 4) == 0) &&
                   (((pdat.year % 100) != 0) || ((pdat.year % 400) == 0)) &&
                   (pdat.month > 2))
                pdat.daynum += 1;
        }


        /*============================================================================
        *    Local void function doy2dom
        *
        *    This function computes the month/day from the day number.
        *
        *    Requires (from struct posdata parameter):
        *        Year and day number:
        *            year
        *            daynum
        *
        *    Returns (via the struct posdata parameter):
        *            year
        *            month
        *            day
        *----------------------------------------------------------------------------*/
        private static void doy2dom(PosData pdat) {
            int imon;  /* Month (month_days) array counter */
            int leap;  /* leap year switch */

            /* Set the leap year switch */
            if (((pdat.year % 4) == 0) &&
                 (((pdat.year % 100) != 0) || ((pdat.year % 400) == 0)))
                leap = 1;
            else
                leap = 0;

            /* Find the month */
            imon = 12;
            while (pdat.daynum <= month_days[leap][imon])
                --imon;

            /* Set the month and day of month */
            pdat.month = imon;
            pdat.day = pdat.daynum - month_days[leap][imon];
        }




        /*============================================================================
        *    Local Void function geometry
        *
        *    Does the underlying geometry for a given time and location
        *----------------------------------------------------------------------------*/
        private static void geometry(PosData pdat) {
            float bottom;      /* denominator (bottom) of the fraction */
            float c2;          /* cosine of d2 */
            float cd;          /* cosine of the day angle or delination */
            float d2;          /* pdat.dayang times two */
            float delta;       /* difference between current year and 1949 */
            float s2;          /* sine of d2 */
            float sd;          /* sine of the day angle */
            float top;         /* numerator (top) of the fraction */
            int leap;        /* leap year counter */

            /* Day angle */
            /*  Iqbal, M.  1983.  An Introduction to Solar Radiation.
                  Academic Press, NY., page 3 */
            pdat.dayang = 360.0f * (pdat.daynum - 1) / 365.0f;

            /* Earth radius vector * solar constant = solar energy */
            /*  Spencer, J. W.  1971.  Fourier series representation of the
                position of the sun.  Search 2 (5), page 172 */
            sd = sin(raddeg * pdat.dayang);
            cd = cos(raddeg * pdat.dayang);
            d2 = 2.0f * pdat.dayang;
            c2 = cos(raddeg * d2);
            s2 = sin(raddeg * d2);

            pdat.erv = 1.000110f + 0.034221f * cd + 0.001280f * sd;
            pdat.erv += 0.000719f * c2 + 0.000077f * s2;

            /* Universal Coordinated (Greenwich standard) time */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.utime =
                pdat.hour * 3600.0f +
                pdat.minute * 60.0f +
                pdat.second -
                (float)pdat.interval / 2.0f;
            pdat.utime = pdat.utime / 3600.0f - pdat.timezone;

            /* Julian Day minus 2,400,000 days (to eliminate roundoff errors) */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */

            /* No adjustment for century non-leap years since this function is
               bounded by 1950 - 2050 */
            delta = pdat.year - 1949;
            leap = (int)(delta / 4.0f);
            pdat.julday =
                32916.5f + delta * 365.0f + leap + pdat.daynum + pdat.utime / 24.0f;

            /* Time used in the calculation of ecliptic coordinates */
            /* Noon 1 JAN 2000 = 2,400,000 + 51,545 days Julian Date */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.ectime = pdat.julday - 51545.0f;

            /* Mean longitude */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.mnlong = 280.460f + 0.9856474f * pdat.ectime;

            /* (dump the multiples of 360, so the answer is between 0 and 360) */
            pdat.mnlong -= 360.0f * (int)(pdat.mnlong / 360.0f);
            if (pdat.mnlong < 0.0f)
                pdat.mnlong += 360.0f;

            /* Mean anomaly */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.mnanom = 357.528f + 0.9856003f * pdat.ectime;

            /* (dump the multiples of 360, so the answer is between 0 and 360) */
            pdat.mnanom -= 360.0f * (int)(pdat.mnanom / 360.0f);
            if (pdat.mnanom < 0.0f)
                pdat.mnanom += 360.0f;

            /* Ecliptic longitude */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.eclong = pdat.mnlong + 1.915f * sin(pdat.mnanom * raddeg) +
                            0.020f * sin(2.0f * pdat.mnanom * raddeg);

            /* (dump the multiples of 360, so the answer is between 0 and 360) */
            pdat.eclong -= 360.0f * (int)(pdat.eclong / 360.0f);
            if (pdat.eclong < 0.0f)
                pdat.eclong += 360.0f;

            /* Obliquity of the ecliptic */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */

            /* 02 Feb 2001 SMW corrected sign in the following line */
            /*  pdat.ecobli = 23.439 + 4.0e-07 * pdat.ectime;     */
            pdat.ecobli = 23.439f - 4.0e-07f * pdat.ectime;

            /* Declination */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.declin = degrad * asin(sin(pdat.ecobli * raddeg) *
                                       sin(pdat.eclong * raddeg));

            /* Right ascension */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            top = cos(raddeg * pdat.ecobli) * sin(raddeg * pdat.eclong);
            bottom = cos(raddeg * pdat.eclong);

            pdat.rascen = degrad * atan2(top, bottom);

            /* (make it a positive angle) */
            if (pdat.rascen < 0.0f)
                pdat.rascen += 360.0f;

            /* Greenwich mean sidereal time */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.gmst = 6.697375f + 0.0657098242f * pdat.ectime + pdat.utime;

            /* (dump the multiples of 24, so the answer is between 0 and 24) */
            pdat.gmst -= 24.0f * (int)(pdat.gmst / 24.0f);
            if (pdat.gmst < 0.0f)
                pdat.gmst += 24.0f;

            /* Local mean sidereal time */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.lmst = pdat.gmst * 15.0f + pdat.longitude;

            /* (dump the multiples of 360, so the answer is between 0 and 360) */
            pdat.lmst -= 360.0f * (int)(pdat.lmst / 360.0f);
            if (pdat.lmst < 0.0f)
                pdat.lmst += 360.0f;

            /* Hour angle */
            /*  Michalsky, J.  1988.  The Astronomical Almanac's algorithm for
                approximate solar position (1950-2050).  Solar Energy 40 (3),
                pp. 227-235. */
            pdat.hrang = pdat.lmst - pdat.rascen;

            /* (force it between -180 and 180 degrees) */
            if (pdat.hrang < -180.0f)
                pdat.hrang += 360.0f;
            else if (pdat.hrang > 180.0f)
                pdat.hrang -= 360.0f;
        }


        /*============================================================================
        *    Local Void function zen_no_ref
        *
        *    ETR solar zenith angle
        *       Iqbal, M.  1983.  An Introduction to Solar Radiation.
        *            Academic Press, NY., page 15
        *----------------------------------------------------------------------------*/
        private static void zen_no_ref(PosData pdat, TrigData tdat) {
            float cz;          /* cosine of the solar zenith angle */

            localtrig(pdat, tdat);
            cz = tdat.sd * tdat.sl + tdat.cd * tdat.cl * tdat.ch;

            /* (watch out for the roundoff errors) */
            if (fabs(cz) > 1.0f) {
                if (cz >= 0.0f)
                    cz = 1.0f;
                else
                    cz = -1.0f;
            }

            pdat.zenetr = acos(cz) * degrad;

            /* (limit the degrees below the horizon to 9 [+90 . 99]) */
            if (pdat.zenetr > 99.0f)
                pdat.zenetr = 99.0f;

            pdat.elevetr = 90.0f - pdat.zenetr;
        }


        /*============================================================================
        *    Local Void function ssha
        *
        *    Sunset hour angle, degrees
        *       Iqbal, M.  1983.  An Introduction to Solar Radiation.
        *            Academic Press, NY., page 16
        *----------------------------------------------------------------------------*/
        private static void ssha(PosData pdat, TrigData tdat) {
            float cssha;       /* cosine of the sunset hour angle */
            float cdcl;        /* ( cd * cl ) */

            localtrig(pdat, tdat);
            cdcl = tdat.cd * tdat.cl;

            if (fabs(cdcl) >= 0.001f) {
                cssha = -tdat.sl * tdat.sd / cdcl;

                /* This keeps the cosine from blowing on roundoff */
                if (cssha < -1.0f)
                    pdat.ssha = 180.0f;
                else if (cssha > 1.0f)
                    pdat.ssha = 0.0f;
                else
                    pdat.ssha = degrad * acos(cssha);
            } else if (((pdat.declin >= 0.0f) && (pdat.latitude > 0.0f)) ||
                        ((pdat.declin < 0.0f) && (pdat.latitude < 0.0f)))
                pdat.ssha = 180.0f;
            else
                pdat.ssha = 0.0f;
        }


        /*============================================================================
        *    Local Void function sbcf
        *
        *    Shadowband correction factor
        *       Drummond, A. J.  1956.  A contribution to absolute pyrheliometry.
        *            Q. J. R. Meteorol. Soc. 82, pp. 481-493
        *----------------------------------------------------------------------------*/
        private static void sbcf(PosData pdat, TrigData tdat) {
            float p, t1, t2;   /* used to compute sbcf */

            localtrig(pdat, tdat);
            p = 0.6366198f * pdat.sbwid / pdat.sbrad * pow(tdat.cd, 3);
            t1 = tdat.sl * tdat.sd * pdat.ssha * raddeg;
            t2 = tdat.cl * tdat.cd * sin(pdat.ssha * raddeg);
            pdat.sbcf = pdat.sbsky + 1.0f / (1.0f - p * (t1 + t2));

        }


        /*============================================================================
        *    Local Void function tst
        *
        *    TST . True Solar Time = local standard time + TSTfix, time
        *      in minutes from midnight.
        *        Iqbal, M.  1983.  An Introduction to Solar Radiation.
        *            Academic Press, NY., page 13
        *----------------------------------------------------------------------------*/
        private static void tst(PosData pdat) {
            pdat.tst = (180.0f + pdat.hrang) * 4.0f;
            pdat.tstfix =
                pdat.tst -
                (float)pdat.hour * 60.0f -
                pdat.minute -
                (float)pdat.second / 60.0f +
                (float)pdat.interval / 120.0f; /* add back half of the interval */

            /* bound tstfix to this day */
            while (pdat.tstfix > 720.0f)
                pdat.tstfix -= 1440.0f;
            while (pdat.tstfix < -720.0f)
                pdat.tstfix += 1440.0f;

            pdat.eqntim =
                pdat.tstfix + 60.0f * pdat.timezone - 4.0f * pdat.longitude;

        }


        /*============================================================================
        *    Local Void function srss
        *
        *    Sunrise and sunset times (minutes from midnight)
        *----------------------------------------------------------------------------*/
        private static void srss(PosData pdat) {
            if (pdat.ssha <= 1.0f) {
                pdat.sretr = 2999.0f;
                pdat.ssetr = -2999.0f;
            } else if (pdat.ssha >= 179.0f) {
                pdat.sretr = -2999.0f;
                pdat.ssetr = 2999.0f;
            } else {
                pdat.sretr = 720.0f - 4.0f * pdat.ssha - pdat.tstfix;
                pdat.ssetr = 720.0f + 4.0f * pdat.ssha - pdat.tstfix;
            }
        }


        /*============================================================================
        *    Local Void function sazm
        *
        *    Solar azimuth angle
        *       Iqbal, M.  1983.  An Introduction to Solar Radiation.
        *            Academic Press, NY., page 15
        *----------------------------------------------------------------------------*/
        private static void sazm(PosData pdat, TrigData tdat) {
            float ca;          /* cosine of the solar azimuth angle */
            float ce;          /* cosine of the solar elevation */
            float cecl;        /* ( ce * cl ) */
            float se;          /* sine of the solar elevation */

            localtrig(pdat, tdat);
            ce = cos(raddeg * pdat.elevetr);
            se = sin(raddeg * pdat.elevetr);

            pdat.azim = 180.0f;
            cecl = ce * tdat.cl;
            if (fabs(cecl) >= 0.001) {
                ca = (se * tdat.sl - tdat.sd) / cecl;
                if (ca > 1.0f)
                    ca = 1.0f;
                else if (ca < -1.0f)
                    ca = -1.0f;

                pdat.azim = 180.0f - acos(ca) * degrad;
                if (pdat.hrang > 0.0f)
                    pdat.azim = 360.0f - pdat.azim;
            }
        }


        /*============================================================================
        *    Local Int function refrac
        *
        *    Refraction correction, degrees
        *        Zimmerman, John C.  1981.  Sun-pointing programs and their
        *            accuracy.
        *            SAND81-0761, Experimental Systems Operation Division 4721,
        *            Sandia National Laboratories, Albuquerque, NM.
        *----------------------------------------------------------------------------*/
        private static void refrac(PosData pdat) {
            float prestemp;    /* temporary pressure/temperature correction */
            float refcor;      /* temporary refraction correction */
            float tanelev;     /* tangent of the solar elevation angle */

            /* If the sun is near zenith, the algorithm bombs; refraction near 0 */
            if (pdat.elevetr > 85.0f)
                refcor = 0.0f;

            /* Otherwise, we have refraction */
            else {
                tanelev = tan(raddeg * pdat.elevetr);
                if (pdat.elevetr >= 5.0f)
                    refcor = 58.1f / tanelev -
                              0.07f / (pow(tanelev, 3)) +
                              0.000086f / (pow(tanelev, 5));
                else if (pdat.elevetr >= -0.575f)
                    refcor = 1735.0f +
                              pdat.elevetr * (-518.2f + pdat.elevetr * (103.4f +
                              pdat.elevetr * (-12.79f + pdat.elevetr * 0.711f)));
                else
                    refcor = -20.774f / tanelev;

                prestemp =
                    (pdat.press * 283.0f) / (1013.0f * (273.0f + pdat.temp));
                refcor *= prestemp / 3600.0f;
            }

            /* Refracted solar elevation angle */
            pdat.elevref = pdat.elevetr + refcor;

            /* (limit the degrees below the horizon to 9) */
            if (pdat.elevref < -9.0f)
                pdat.elevref = -9.0f;

            /* Refracted solar zenith angle */
            pdat.zenref = 90.0f - pdat.elevref;
            pdat.coszen = cos(raddeg * pdat.zenref);
        }


        /*============================================================================
        *    Local Void function  amass
        *
        *    Airmass
        *       Kasten, F. and Young, A.  1989.  Revised optical air mass
        *            tables and approximation formula.  Applied Optics 28 (22),
        *            pp. 4735-4738
        *----------------------------------------------------------------------------*/
        private static void amass(PosData pdat) {
            if (pdat.zenref > 93.0f) {
                pdat.amass = -1.0f;
                pdat.ampress = -1.0f;
            } else {
                pdat.amass =
                    1.0f / (cos(raddeg * pdat.zenref) + 0.50572f *
                    pow((96.07995f - pdat.zenref), -1.6364f));

                pdat.ampress = pdat.amass * pdat.press / 1013.0f;
            }
        }


        /*============================================================================
        *    Local Void function prime
        *
        *    Prime and Unprime
        *    Prime  converts Kt to normalized Kt', etc.
        *       Unprime deconverts Kt' to Kt, etc.
        *            Perez, R., P. Ineichen, Seals, R., & Zelenka, A.  1990.  Making
        *            full use of the clearness index for parameterizing hourly
        *            insolation conditions. Solar Energy 45 (2), pp. 111-114
        *----------------------------------------------------------------------------*/
        private static void prime(PosData pdat) {
            pdat.unprime = 1.031f * exp(-1.4f / (0.9f + 9.4f / pdat.amass)) + 0.1f;
            pdat.prime = 1.0f / pdat.unprime;
        }


        /*============================================================================
        *    Local Void function etr
        *
        *    Extraterrestrial (top-of-atmosphere) solar irradiance
        *----------------------------------------------------------------------------*/
        private static void etr(PosData pdat) {
            if (pdat.coszen > 0.0f) {
                pdat.etrn = pdat.solcon * pdat.erv;
                pdat.etr = pdat.etrn * pdat.coszen;
            } else {
                pdat.etrn = 0.0f;
                pdat.etr = 0.0f;
            }
        }


        /*============================================================================
        *    Local Void function localtrig
        *
        *    Does trig on internal variable used by several functions
        *----------------------------------------------------------------------------*/
        private static void localtrig(PosData pdat, TrigData tdat) {
            /* define masks to prevent calculation of uninitialized variables */
            const int SD_MASK = (L_ZENETR | L_SSHA | S_SBCF | S_SOLAZM);
            const int SL_MASK = (L_ZENETR | L_SSHA | S_SBCF | S_SOLAZM);
            const int CL_MASK = (L_ZENETR | L_SSHA | S_SBCF | S_SOLAZM);
            const int CD_MASK = (L_ZENETR | L_SSHA | S_SBCF);
            const int CH_MASK = (L_ZENETR);

            if (tdat.sd < -900.0f)  /* sd was initialized -999 as flag */ {
                tdat.sd = 1.0f;  /* reflag as having completed calculations */
                if ((pdat.function | CD_MASK) != 0)
                    tdat.cd = cos(raddeg * pdat.declin);
                if ((pdat.function | CH_MASK) != 0)
                    tdat.ch = cos(raddeg * pdat.hrang);
                if ((pdat.function | CL_MASK) != 0)
                    tdat.cl = cos(raddeg * pdat.latitude);
                if ((pdat.function | SD_MASK) != 0)
                    tdat.sd = sin(raddeg * pdat.declin);
                if ((pdat.function | SL_MASK) != 0)
                    tdat.sl = sin(raddeg * pdat.latitude);
            }
        }


        /*============================================================================
        *    Local Void function tilt
        *
        *    ETR on a tilted surface
        *----------------------------------------------------------------------------*/
        private static void tilt(PosData pdat) {
            float ca;          /* cosine of the solar azimuth angle */
            float cp;          /* cosine of the panel aspect */
            float ct;          /* cosine of the panel tilt */
            float sa;          /* sine of the solar azimuth angle */
            float sp;          /* sine of the panel aspect */
            float st;          /* sine of the panel tilt */
            float sz;          /* sine of the refraction corrected solar zenith angle */


            /* Cosine of the angle between the sun and a tipped flat surface,
               useful for calculating solar energy on tilted surfaces */
            ca = cos(raddeg * pdat.azim);
            cp = cos(raddeg * pdat.aspect);
            ct = cos(raddeg * pdat.tilt);
            sa = sin(raddeg * pdat.azim);
            sp = sin(raddeg * pdat.aspect);
            st = sin(raddeg * pdat.tilt);
            sz = sin(raddeg * pdat.zenref);
            pdat.cosinc = pdat.coszen * ct + sz * st * (ca * cp + sa * sp);

            if (pdat.cosinc > 0.0f)
                pdat.etrtilt = pdat.etrn * pdat.cosinc;
            else
                pdat.etrtilt = 0.0f;

        }


        /*============================================================================
        *    Void function S_decode
        *
        *    This function decodes the error codes from S_solpos return value
        *
        *    Requires the long integer return value from S_solpos
        *
        *    Returns descriptive text
        *----------------------------------------------------------------------------*/
        public static string S_decode(long code, PosData pdat) {
            string ret = "";
            if ((code & (1L << S_YEAR_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the year: {0} [1950-2050]\n",
                  pdat.year);
            if ((code & (1L << S_MONTH_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the month: {0}\n",
                  pdat.month);
            if ((code & (1L << S_DAY_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the day-of-month: {0}\n",
                  pdat.day);
            if ((code & (1L << S_DOY_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the day-of-year: {0}\n",
                  pdat.daynum);
            if ((code & (1L << S_HOUR_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the hour: {0}\n",
                  pdat.hour);
            if ((code & (1L << S_MINUTE_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the minute: {0}\n",
                  pdat.minute);
            if ((code & (1L << S_SECOND_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the second: {0}\n",
                  pdat.second);
            if ((code & (1L << S_TZONE_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the time zone: {0}\n",
                  pdat.timezone);
            if ((code & (1L << S_INTRVL_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the interval: {0}\n",
                  pdat.interval);
            if ((code & (1L << S_LAT_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the latitude: {0}\n",
                  pdat.latitude);
            if ((code & (1L << S_LON_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the longitude: {0}\n",
                  pdat.longitude);
            if ((code & (1L << S_TEMP_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the temperature: {0}\n",
                  pdat.temp);
            if ((code & (1L << S_PRESS_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the pressure: {0}\n",
                  pdat.press);
            if ((code & (1L << S_TILT_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the tilt: {0}\n",
                  pdat.tilt);
            if ((code & (1L << S_ASPECT_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the aspect: {0}\n",
                  pdat.aspect);
            if ((code & (1L << S_SBWID_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the shadowband width: {0}\n",
                  pdat.sbwid);
            if ((code & (1L << S_SBRAD_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the shadowband radius: {0}\n",
                  pdat.sbrad);
            if ((code & (1L << S_SBSKY_ERROR)) != 0)
                ret += String.Format("S_decode ==> Please fix the shadowband sky factor: {0}\n",
                  pdat.sbsky);
            return ret;
        }


        /*============================================================================
        *    Math helper functions
        *
        *    These replace the neccessary parts of <math.h>
        *----------------------------------------------------------------------------*/
        private static float sin(float theta) {
            return (float)Math.Sin(theta);
        }
        private static float cos(float theta) {
            return (float)Math.Cos(theta);
        }
        private static float tan(float theta) {
            return (float)Math.Tan(theta);
        }
        private static float asin(float v) {
            return (float)Math.Asin(v);
        }
        private static float acos(float v) {
            return (float)Math.Acos(v);
        }
        private static float atan2(float y, float x) {
            return (float)Math.Atan2(y, x);
        }
        private static float exp(float d) {
            return (float)Math.Exp(d);
        }
        private static float pow(float a, float b) {
            return (float)Math.Pow(a, b);
        }
        private static float fabs(float v) {
            return Math.Abs(v);
        }
    }
}
