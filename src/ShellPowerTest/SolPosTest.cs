using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSCP.ShellPower;
using OpenTK;
using System.Diagnostics;

namespace SSCP.ShellPower.Test {

    /// <summary>
    ///    NAME:  stest00.c
    ///
    ///    PURPOSE:  Exercises the solar position algorithms in 'solpos.c'.
    ///
    ///        S_solpos
    ///            INPUTS:     year, daynum, hour, minute, second, latitude,
    ///                        longitude, timezone
    ///
    ///            OPTIONAL:   press   DEFAULT 1013.0 (standard pressure)
    ///                        temp    DEFAULT   10.0 (standard temperature)
    ///                        tilt    DEFAULT    0.0 (horizontal panel)
    ///                        aspect  DEFAULT  180.0 (South-facing panel)
    ///                        month   (if the S_DOY function is turned off)
    ///                        day     ( "             "             "     )
    ///
    ///            OUTPUTS:    amass, ampress, azim, cosinc, coszen, day, daynum,
    ///                        elevref, etr, etrn, etrtilt, month, prime,
    ///                        sbcf, sretr, ssetr, unprime, zenref
    ///
    ///       S_init        (optional initialization for all input parameters in
    ///                      the posdata struct)
    ///           INPUTS:     struct posdata*
    ///           OUTPUTS:    struct posdata*
    ///
    ///                     (Note: initializes the required S_solpos INPUTS above
    ///                      to out-of-bounds conditions, forcing the user to
    ///                      supply the parameters; initializes the OPTIONAL
    ///                      S_solpos inputs above to nominal values.)
    ///
    ///      S_decode       (optional utility for decoding the S_solpos return code)
    ///           INPUTS:     long int S_solpos return value, struct posdata*
    ///           OUTPUTS:    text to stderr
    ///
    ///
    ///        All variables are defined as members of the struct posdata
    ///        in 'solpos00.h'.
    ///
    ///    Usage:
    ///         In calling program, along with other 'includes', insert:
    ///
    ///              #include "solpos00.h"
    ///
    ///    Martin Rymes
    ///    National Renewable Energy Laboratory
    ///    25 March 1998
    ///
    ///    28 March 2001 REVISION:  SMW changed benchmark numbers to reflect the
    ///                             February 2001 changes to solpos00.c
    ///</summary>
    [TestClass]
    public class SolPosTest {

        private void SetInputAtlanta(PosData pdat) {
            /* Initialize structure to default values. (Optional only if ALL input
   parameters are initialized in the calling code, which they are not
   in this example.) */
            SolPos.S_init(pdat);

            /* I use Atlanta, GA for this example */

            pdat.longitude = -84.43f;  /* Note that latitude and longitude are  */
            pdat.latitude = 33.65f;  /*   in DECIMAL DEGREES, not Deg/Min/Sec */
            pdat.timezone = -5.0f;   /* Eastern time zone, even though longitude would
                                  suggest Central.  We use what they use.
                                  DO NOT ADJUST FOR DAYLIGHT SAVINGS TIME. */

            pdat.year = 1999;    /* The year is 1999. */
            pdat.daynum = 203;    /* July 22nd, the 203'rd day of the year (the
                                  algorithm will compensate for leap year, so
                                  you just count days). S_solpos can be
                                  configured to accept month-day dates; see
                                  examples below.) */

            /* The time of day (STANDARD time) is 9:45:37 */

            pdat.hour = 9;
            pdat.minute = 45;
            pdat.second = 37;

            /* Let's assume that the temperature is 27 degrees C and that
               the pressure is 1006 millibars.  The temperature is used for the
               atmospheric refraction correction, and the pressure is used for the
               refraction correction and the pressure-corrected airmass. */

            pdat.temp = 27.0f;
            pdat.press = 1006.0f;

            /* Finally, we will assume that you have a flat surface facing southeast,
               tilted at latitude. */

            pdat.tilt = pdat.latitude;  /* Tilted at latitude */
            pdat.aspect = 135.0f;       /* 135 deg. = SE */
        }

        public void VerifyOutputAtlanta(PosData pdat) {
            /* Now look at the results and compare with NREL benchmark */
            printf("Note that your final decimal place values may vary\n");
            printf("based on your computer's floating-point storage and your\n");
            printf("compiler's mathematical algorithms.  If you agree with\n");
            printf("NREL's values for at least 5 significant digits, assume it works.\n\n");

            printf("Note that S_solpos has returned the day and month for the\n");
            printf("input daynum.  When configured to do so, S_solpos will reverse\n");
            printf("this input/output relationship, accepting month and day as\n");
            printf("input and returning the day-of-year in the daynum variable.\n");
            printf("\n");
            printf("NREL    . 1999.07.22, daynum 203, retval 0, amass 1.335752, ampress 1.326522\n");
            printf("SOLTEST . {0}.{1:00}.{2:00}, daynum {3}, mass {4}, ampress {5}\n",
              pdat.year, pdat.month, pdat.day, pdat.daynum,
              pdat.amass, pdat.ampress);
            Assert.AreEqual(1999, pdat.year);
            Assert.AreEqual(7, pdat.month);
            Assert.AreEqual(22, pdat.day);
            Assert.AreEqual(203, pdat.daynum);
            Assert.AreEqual(1.335752, pdat.amass, 1e-6);
            Assert.AreEqual(1.326522, pdat.ampress, 1e-6);
            printf("NREL    . azim 97.032875, cosinc 0.912569, elevref 48.409931\n");
            printf("SOLTEST . azim {0}, cosinc {1}, elevref {2}\n",
              pdat.azim, pdat.cosinc, pdat.elevref);
            Assert.AreEqual(97.032875, pdat.azim, 1e-4);
            Assert.AreEqual(0.912569, pdat.cosinc, 1e-6);
            Assert.AreEqual(48.409931, pdat.elevref, 1e-4);
            printf("NREL    . etr 989.668518, etrn 1323.239868, etrtilt 1207.547363\n");
            printf("SOLTEST . etr {0}, etrn {1}, etrtilt {2}\n",
              pdat.etr, pdat.etrn, pdat.etrtilt);
            Assert.AreEqual(989.668518, pdat.etr, 1e-3);
            Assert.AreEqual(1323.239868, pdat.etrn, 1e-3);
            Assert.AreEqual(1207.547363, pdat.etrtilt, 1e-3);
            printf("NREL    . prime 1.037040, sbcf 1.201910, sunrise 347.173431\n");
            printf("SOLTEST . prime {0}, sbcf {1}, sunrise {2}\n",
              pdat.prime, pdat.sbcf, pdat.sretr);
            Assert.AreEqual(1.037040, pdat.prime, 1e-6);
            Assert.AreEqual(1.201910, pdat.sbcf, 1e-6);
            Assert.AreEqual(347.173431, pdat.sretr, 1e-3);
            printf("NREL    . sunset 1181.111206, unprime 0.964283, zenref 41.590069\n");
            printf("SOLTEST . sunset {0}, unprime {1}, zenref {2}\n",
              pdat.ssetr, pdat.unprime, pdat.zenref);
            Assert.AreEqual(1181.111206, pdat.ssetr, 1e-3);
            Assert.AreEqual(0.964283, pdat.unprime, 1e-6);
            Assert.AreEqual(41.590069, pdat.zenref, 1e-4);
        }

        [TestMethod]
        public void TestSolPos() {
            PosData pdat = new PosData();
            SetInputAtlanta(pdat);

            printf("\n***** TEST S_solpos: *****\n");
            long retval = SolPos.S_solpos(pdat);  /* S_solpos function call */
            printf(SolPos.S_decode(retval, pdat));    /* ALWAYS look at the return code! */
            Assert.AreEqual(0, retval);

            VerifyOutputAtlanta(pdat);
        }

        [TestMethod]
        public void TestFunctions() {
            PosData pdat = new PosData();
            SetInputAtlanta(pdat);

            /**********************************************************************/
            /* S_solpos configuration examples using the function parameter.

               Selecting a minimum of functions to meet your needs may result in
               faster execution.  A factor of two difference in execution speed
               exists between S_GEOM (the minimum configuration) and S_ALL (all
               variables calculated).  [S_DOY is actually the simplest and fastest
               configuration by far, but it only does the date conversions and bypasses
               all solar geometry.] If speed is not a consideration, use the default
               S_ALL configuration implemented by the call to S_init.

               The bitmasks are defined in S_solpos.h. */

            /* 1) Calculate the refraction corrected solar position variables */
            pdat.function = SolPos.S_REFRAC;
            /* 2) Calculate the shadow band correction factor */
            pdat.function = SolPos.S_SBCF;
            /* 3) Select both of the above functions (Note that the two bitmasks
                  are 'or-ed' together to produce the desired results): */
            pdat.function = (SolPos.S_REFRAC | SolPos.S_SBCF);

            long retval = SolPos.S_solpos(pdat);  /* S_solpos function call */
            printf(SolPos.S_decode(retval, pdat));    /* ALWAYS look at the return code! */
            Assert.AreEqual(0, retval);
            Assert.AreEqual(1999, pdat.year);
            Assert.AreEqual(7, pdat.month);
            Assert.AreEqual(22, pdat.day);
            Assert.AreEqual(203, pdat.daynum);
            Assert.AreEqual(1.201910, pdat.sbcf, 1e-6);
            Assert.AreEqual(48.409931, pdat.elevref, 1e-4);

            /* 4) Modify the above configuration for accepting month and day rather
                  than day-of-year.  Note that S_DOY (which controls on the day-of-year
                  interpretation) must be inverted, then 'and-ed' with the other
                  function codes to turn the day-of-year OFF.  With the day-of-year
              bit off, S_solpos expects date input in the form of month and day. */

            pdat.function = ((SolPos.S_REFRAC | SolPos.S_SBCF) & ~SolPos.S_DOY);
            pdat.month = 7;
            pdat.day = 22;
            pdat.daynum = -999;
            
            retval = SolPos.S_solpos(pdat);  /* S_solpos function call */
            printf(SolPos.S_decode(retval, pdat));    /* ALWAYS look at the return code! */
            Assert.AreEqual(0, retval);
            Assert.AreEqual(1999, pdat.year);
            Assert.AreEqual(7, pdat.month);
            Assert.AreEqual(22, pdat.day);
            Assert.AreEqual(203, pdat.daynum);
            Assert.AreEqual(1.201910, pdat.sbcf, 1e-6);
            Assert.AreEqual(48.409931, pdat.elevref, 1e-4);

            /*    Also note that S_DOY is the only function that you should attempt
                  to clear in this manner: Other function bitmasks are a composite
                  of more than one mask, which represents an interdependency among
                  functions. Turning off unexpected bits will produce unexpected
                  results.  If in the course of your program you need fewer
                  parameters calculated, you should rebuild the function mask
                  from zero using only the required function bitmasks. */
        }

        [TestMethod]
        public void TestInputValidation() {
            PosData pdat = new PosData();
            SetInputAtlanta(pdat);

            /**********************************************************************/
            /* Looking at the S_solpos return code

               In the return code, each bit represents an error in the range of
               individual input parameters.  See the bit definition in S_solpos.h
               for the position of each error flag.

               To assure that none of your input variables are out of bounds, the
               calling program should always look at the S_solpos return code.  In
               this example, the function S_decode fulfills that mandate by examining
               the code and writing an interpretation to the standard error output.

               To see the effect of an out of bounds parameter, move the following
               line to just before the call to S_solpos: */

            pdat.year = 99;  /* will S_solpos accept a two-digit year? */

            /* This causes S_decode to output a descriptive line regarding the value
               of the input year. [This algorithm is valid only between years 1950 and
               2050; hence, explicit four-digit years are required. If your dates are
               in a two-digit format, S_solpos requires that you make a conversion
               to an explicit four-digit year.]

               S_decode (located in the solpos.c file) can serve as a template for
               building your own decoder to handle errors according to the
               requirements of your calling application. */

            long retval = SolPos.S_solpos(pdat);  /* S_solpos function call */
            printf(SolPos.S_decode(retval, pdat));    /* ALWAYS look at the return code! */
            Assert.AreEqual(1<<SolPos.S_YEAR_ERROR, retval);
        }
            
        [TestMethod]
        public void TestRawAirmass() {
            PosData pdat = new PosData();

            /***********************************************************************/
            /* Accessing the individual functions */

            /* S_solpos was designed to calculate the output variables using the
               documented input variables.  However, as a matter of advanced
               programming convenience, the individual functions within S_solpos
               are accessible to the calling program through the use of the primative
               L_ masks (these are different from the composite S_ masks used
               above).  However, USE THESE WTTH CAUTION since the calling program
               must supply ALL parameters required by the function.  Because many of
               these variables are otherwise carefully created internally by
               S_solpos, the individual functions may not have bounds checking;
               hence your calling program must do all validation on the function
               input parameters. By the same reasoning, the return error code
               (retval) may not have considered all relevant input values, leaving
               the function vulnerable to computational errors or an abnormal end
               condition.

               As with the S_ masks above, the function variable is set to the
               L_ mask.  L_ masks may be ORed if desired.

               The solpos.h file contains a list of all output and transition
               variables, the reqired L_ mask, and all variables necessary for the
               calculation within individual functions.

               For example, the following code seeks only the amass value.  It calls
               only the airmass routine, which requires nothing but refracted zenith
               angle and pressure. Normally, the refracted zenith angle is a
               calculation that depends on many other functions within S_solpos.  But
               here using the L_ mask, we can simply set the refracted zenith angle
               externally and call the airmass function. */

            pdat.function = SolPos.L_AMASS;  /* call only the airmass function */
            pdat.press = 1013.0f;   /* set your own pressure          */

            /* set up for the output of this example */
            printf("Raw airmass loop:\n");
            printf("NREL    . 37.92  5.59  2.90  1.99  1.55  1.30  1.15  1.06  1.02  1.00\n");
            printf("SOLTEST . ");

            /* loop through a number of externally-set refracted zenith angles */
            float[] expectedOutput = {37.92f, 5.59f, 2.90f, 1.99f, 1.55f, 1.30f, 1.15f, 1.06f, 1.02f, 1.00f};
            for (int i = 0; i < 10; i++){
                pdat.zenref = 90.0f - i*10.0f;
                long retval = SolPos.S_solpos(pdat);   /* call solpos */
                SolPos.S_decode(retval, pdat);         /* retval may not be valid */
                printf("{0:00000.00} ", pdat.amass);   /* print out the airmass */

                Assert.AreEqual(0, retval);
                Assert.AreEqual(expectedOutput[i], pdat.amass, 1e-2);
            }
            printf("\n");
        }

        private static void printf(String format, params Object[] args) {
            Debug.WriteLine(String.Format(format, args));
        }
    }
}