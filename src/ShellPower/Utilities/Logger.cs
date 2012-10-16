using System;
using System.Diagnostics;

namespace SSCP.ShellPower {
    /// <summary>
    /// Simple logger.
    /// 
    /// TODO: replace with standard logging.
    /// </summary>
    public static class Logger {
        private static String timestamp() {
            return DateTime.Now.ToString("yyyyMMdd HH:mm:sszzz");
        }
        public static void warn(String msg, params Object[] args) {
            Debug.WriteLine(timestamp() + "\t" + String.Format(msg, args));
        }
        public static void info(String msg, params Object[] args) {
            Debug.WriteLine(timestamp() + "\t" + String.Format(msg, args));
        }
    }
}
