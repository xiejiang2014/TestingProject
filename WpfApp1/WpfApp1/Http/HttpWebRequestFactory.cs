using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WpfApp1.Http
{
    internal static class HttpWebRequestFactory
    {
        private static readonly string DefaultUserAgent = @"Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        private static bool CheckValidationResult(object          sender,
                                                  X509Certificate certificate,
                                                  X509Chain       chain,
                                                  SslPolicyErrors errors
        )
        {
            return true; //总是接受
        }

        public static HttpWebRequest CreateHttpWebRequest(
            string               url,
            string               httpMethod,
            string               contentType,
            NameValueCollection? headers   = null,
            int                  timeout   = 30000,
            string?              userAgent = "",
            CookieCollection?    cookies   = null
        )
        {
            HttpWebRequest request;

            //如果是发送HTTPS请求
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                request                                                 = (HttpWebRequest) WebRequest.Create(url);

                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = (HttpWebRequest) WebRequest.Create(url);
            }

            request.KeepAlive = false;
            request.Method      = httpMethod;
            request.ContentType = contentType;

            ServicePointManager.Expect100Continue       = false;
            ServicePointManager.DefaultConnectionLimit  = 200;
            ServicePointManager.MaxServicePointIdleTime = 2000;
            ServicePointManager.SetTcpKeepAlive(false, 0, 0);

            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrWhiteSpace(userAgent))
            {
                request.UserAgent = userAgent;
            }

            request.Timeout   = timeout;

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }

            if (headers is {Count: > 0})
            {
                foreach (string key in headers.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }
            }

            return request;
        }
    }
}