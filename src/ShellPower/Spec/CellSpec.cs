using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public class CellSpec {
        public const double STC_TEMP = 25.0;
        public const double STC_INSOLATION = 1000.0;

        /// <summary>
        /// Open-circuit voltage at Standard Test Conditions.
        /// </summary>
        public double VocStc { get; set; }
        /// <summary>
        /// Short-circuit current at Standard Test Conditions.
        /// </summary>
        public double IscStc { get; set; }
        /// <summary>
        /// Change in Voc per degree increase in temperature.
        /// </summary>
        public double DVocDT { get; set; }
        /// <summary>
        /// Change in Isc per degree increase in temperature.
        /// </summary>
        public double DIscDT { get; set; }
        /// <summary>
        /// Cell area in square meters.
        /// </summary>
        public double Area { get; set; }
        /// <summary>
        /// Diode ideality constant. 1.0 = ideal diode, larger = worse.
        /// </summary>
        public double NIdeal { get; set; }
        /// <summary>
        /// Cell series resistance in Ohms. Usually ~10 milliohms for a silicon pv cell.
        /// </summary>
        public double SeriesR { get; set; }

        /// <summary>
        /// Computes temperature in deg kelvin.
        /// </summary>
        private double CalcTempK(double tempC) {
            return tempC + Constants.C_IN_KELVIN;
        }
        /// <summary>
        /// Open-circuit voltage.
        /// </summary>
        public double CalcVoc(double insolationW, double tempC) {
            // TODO: adjust for insolation
            return VocStc + (tempC - STC_TEMP) * DVocDT;
        }
        /// <summary>
        /// Short-circuit current.
        /// </summary>
        public double CalcIsc(double insolationW, double tempC) {
            return insolationW / STC_INSOLATION * (IscStc + (tempC - STC_TEMP) * DIscDT);
        }
        /// <summary>
        /// Reverse saturation current for a cell.
        /// </summary>
        public double CalcI0(double insolationW, double tempC) {
            double voc = CalcVoc(insolationW, tempC);
            double isc = CalcIsc(insolationW, tempC);
            double t = CalcTempK(tempC);
            double k = Constants.BOLTZMANN_K;
            double q = Constants.ELECTRON_CHARGE_Q;
            double i0 = isc / (Math.Exp((q * voc) / (NIdeal * k * t)) - 1.0);
            return i0;
        }
    }
}
