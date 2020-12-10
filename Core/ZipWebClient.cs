using System;
using System.Net;
using System.Text;
namespace AdventOfCode.Core {
    public class ZipWebClient : WebClient {
        public ZipWebClient() : base() {
            this.Encoding = Encoding.Default;
            this.Headers["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
            this.Headers["accept-encoding"] = "gzip, deflate";
            this.Headers["accept-language"] = "en-US,en";
            this.Headers["accept"] = "text/html,application/xhtml+xml,application/xml";
            this.Headers["cache-control"] = "max-age=0";
        }
        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}