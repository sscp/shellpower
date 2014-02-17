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
            TimeZoneInfo[] tzs = TimeZoneInfo.GetSystemTimeZones().ToArray();
            comboBoxTimezone.Items.Clear();
            comboBoxTimezone.Items.AddRange(tzs);
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
                var tzOffsetHours = simInput.Timezone.GetUtcOffset(simInput.Utc).TotalHours;
                labelTimezone.Text = string.Format("GMT{0}{1:0.0}", tzOffsetHours >= 0 ? "+" : "", tzOffsetHours);
                labelLocalTime.Text = simInput.LocalTime.ToString("HH:mm:ss");
                trackBarTimeOfDay.Value = (int)(simInput.LocalTime.TimeOfDay.TotalHours * (trackBarTimeOfDay.Maximum + 1) / 24);
                int tzIx;
                for(tzIx = 0; tzIx < comboBoxTimezone.Items.Count; tzIx++){
                    if(((TimeZoneInfo)comboBoxTimezone.Items[tzIx]).Id == simInput.Timezone.Id){
                        break;
                    }
                }
                if (tzIx > comboBoxTimezone.Items.Count) throw new Exception("can't find timezone: "+ simInput.Timezone);
                comboBoxTimezone.SelectedIndex = tzIx;
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
            TimeZoneInfo tz = (TimeZoneInfo)comboBoxTimezone.SelectedItem;
            DateTime utcTime = dateTimePicker.Value;

            /* get car orientation */
            double heading = 2 * Math.PI * trackBarCarDirection.Value / (trackBarCarDirection.Maximum + 1);
            double tilt = Math.PI * trackBarTilt.Value / 180.0;

            /* get all sim inputs */
            simInput.Heading = heading;
            simInput.Tilt = tilt;
            simInput.Latitude = lat;
            simInput.Longitude = lon;
            simInput.Timezone = tz;
            simInput.Utc = utcTime;

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
            var timeOfDay = new TimeSpan((long)(hours * 60 * 60 * 10000000) + 1);
            var localTime = simInput.LocalTime.Date + timeOfDay;
            simInput.Utc = localTime - simInput.Timezone.GetUtcOffset(localTime);
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