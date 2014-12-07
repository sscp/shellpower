using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    public partial class ArraySimInputControl : UserControl {

        private readonly TimeZoneInfo DEFAULT_TZ = TimeZoneInfo.FindSystemTimeZoneById("AUS Central Standard Time");
        private ArraySimulationStepInput simInput;
        private bool updating;
        public ArraySimulationStepInput SimInput {
            set { simInput = value; UpdateView(); }
            get { return simInput; }
        }

        public ArraySimInputControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Updates sim controls (view) from sim state (model).
        /// </summary>
        public void UpdateView() {
            if (simInput == null) return;
            updating = true;
            try {
                /* set lat/lon */
                textBoxLat.Text = string.Format("{0:0.##########}", simInput.Latitude);
                textBoxLon.Text = string.Format("{0:0.##########}", simInput.Longitude);

                /* set heading */
                string[] headings = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
                int dirIx = (int)Math.Round(simInput.Heading / (2 * Math.PI) * 16);
                if (dirIx >= headings.Length) dirIx -= headings.Length;
                labelCarDirection.Text = string.Format("{0:0.00}° {1}",
                    Astro.rad2deg(simInput.Heading), headings[dirIx]);
                int dirIx2 = (int)Math.Round(simInput.Heading / (2 * Math.PI) * (trackBarCarDirection.Maximum+1));
                if (dirIx2 > trackBarCarDirection.Maximum) dirIx2 -= trackBarCarDirection.Maximum + 1;
                trackBarCarDirection.Value = dirIx2;

                /* set tilt */
                double tiltDeg = Astro.rad2deg(Math.Abs(simInput.Tilt));
                String tiltDir;
                if(simInput.Tilt > 0){
                    tiltDir = "right";
                } else if(simInput.Tilt < 0){
                    tiltDir = "left";
                } else {
                    tiltDir = "";
                }
                labelTilt.Text = string.Format("{0:0.00}° {1}", tiltDeg, tiltDir);

                /* set date/time */
                dateTimePicker.Value = simInput.Utc; // fix roundoff problems
                var localTime = simInput.Utc.AddHours(simInput.TimezoneOffsetHours);
                labelLocalTime.Text = localTime.ToString("HH:mm:ss");
                trackBarTimeOfDay.Value = (int)(localTime.TimeOfDay.TotalHours * (trackBarTimeOfDay.Maximum + 1) / 24);
                textBoxTimezone.Text = string.Format("{0:0.##########}", simInput.TimezoneOffsetHours);

                /* set conditions */
                textBoxIrrad.Text = string.Format("{0:0.##########}", simInput.Irradiance);
                textBoxIndirectIrrad.Text = string.Format("{0:0.##########}", simInput.IndirectIrradiance);
                textBoxEncapLoss.Text = string.Format("{0:0.##########}", simInput.Array.EncapsulationLoss * 100);
            } finally {
                updating = false;
            }
        }

        /// <summary>
        /// Updates instantaneous sim inputs (model) from the controls (view).
        /// Eg latitude, longitude, heading etc.
        /// </summary>
        public void UpdateModel() {
            if (simInput == null || updating) {
                return;
            }

            /* get location */
            var lat = double.Parse(textBoxLat.Text);
            var lon = double.Parse(textBoxLon.Text);
            if (lat < -90 || lat > 90 || lon < -180 || lon > 180) {
                throw new Exception("Latitude must be in [-90, 90] and longitude must be in [-180, 180].");
            }

            /* get time */
            double tzOffset = double.Parse(textBoxTimezone.Text);
            if (tzOffset < -15 || tzOffset > 15) {
                throw new Exception("Timezone offset must be in [-15, 15].");
            }
            DateTime utcTime = dateTimePicker.Value;

            /* get car orientation */
            double heading = 2 * Math.PI * trackBarCarDirection.Value / (trackBarCarDirection.Maximum + 1);
            double tilt = Math.PI * trackBarTilt.Value / 180.0;

            /* get conditions */
            double irrad = double.Parse(textBoxIrrad.Text);
            double indirectIrrad = double.Parse(textBoxIndirectIrrad.Text);
            double encapLoss = double.Parse(textBoxEncapLoss.Text) / 100;
            if (irrad < 0 || irrad > 2000 || indirectIrrad < 0 || indirectIrrad > 2000) {
                throw new Exception("Irradiance must be between 0 and 2000 w/m^2");
            }
            if (encapLoss < 0 || encapLoss > 1) {
                throw new Exception("Encapsulation loss must be between 0% and 100%");
            }

            /* construct sim input */
            simInput.Heading = heading;
            simInput.Tilt = tilt;
            simInput.Latitude = lat;
            simInput.Longitude = lon;
            simInput.TimezoneOffsetHours = tzOffset;
            simInput.Utc = utcTime;
            simInput.Irradiance = irrad;
            simInput.IndirectIrradiance = indirectIrrad;
            simInput.Array.EncapsulationLoss = encapLoss;

            Logger.info("sim inputs\n\t" +
                "lat {0:0.0} lon {1:0.0} heading {2:0.0} tilt {3:0.0} utc {4} sidereal {5}",
                simInput.Latitude,
                simInput.Longitude,
                Astro.rad2deg(simInput.Heading),
                Astro.rad2deg(simInput.Tilt),
                utcTime,
                Astro.sidereal_time(utcTime, simInput.Longitude));
        }

        public event EventHandler Change;

        private void trackBarTimeOfDay_Scroll(object sender, EventArgs e) {
            double hours = (double)trackBarTimeOfDay.Value / (trackBarTimeOfDay.Maximum + 1) * 24;
            var localTime = simInput.Utc.AddHours(simInput.TimezoneOffsetHours).Date.AddHours(hours);
            simInput.Utc = localTime.AddHours(-simInput.TimezoneOffsetHours);
            UpdateView();
        }

        private void anyInput_changed(object sender, EventArgs args) {
            try {
                UpdateModel();
                UpdateView();
            } catch (Exception e) {
                MessageBox.Show(e.Message);
                ((Control)sender).Focus();
            }
            if (Change != null) Change(this, args);
        }
    }
}