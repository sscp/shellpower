using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml;

namespace SSCP.ShellPower {
    /// <summary>
    /// Convenience functions for accessing the web.
    /// TODO: GetHtml, overloads that take POST data
    /// </summary>
    public static class Web {
        public static string Get(string url) {
            Debug.WriteLine("Getting " + url);
            var stream = GetStream(url);
            string content = new StreamReader(stream).ReadToEnd();
            return content;
        }
        public static Stream GetStream(string url) {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            return response.GetResponseStream();
        }
        public static T GetJson<T>(string url) {
            var json = Get(url);
            var serializer = new JavaScriptSerializer();
            var ret = serializer.Deserialize<T>(json);
            return ret;
        }
        public static XmlDocument GetXml(string url) {
            var doc = new XmlDocument();
            var stream = GetStream(url);
            doc.Load(stream);
            return doc;
        }
    }
}
