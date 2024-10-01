using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;

namespace DiscordWebhook {
    public static class HttpQueryUtility {
        public static string ToQueryString(NameValueCollection nvc) {
            IEnumerable<string> segments = from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}",
                    WebUtility.UrlEncode(key),
                    WebUtility.UrlEncode(value));
            return "?" + string.Join("&", segments);
        }

        public static NameValueCollection ParseQueryString(string query) {
            NameValueCollection nvc = new NameValueCollection();
            if (string.IsNullOrWhiteSpace(query)) {
                return nvc;
            }

            string[] pairs = query.TrimStart('?').Split('&');
            foreach (string pair in pairs) {
                string[] parts = pair.Split('=');
                if (parts.Length == 2) {
                    nvc.Add(WebUtility.UrlDecode(parts[0]), WebUtility.UrlDecode(parts[1]));
                }
            }

            return nvc;
        }
    }
}