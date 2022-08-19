using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CDIC.Tools;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace ShareDrawing.HttpClient.Http
{
    /// <summary>
    /// 调用本类的方法时,主要可能会发生以下异常,注意处理
    /// System.Net.WebException     通信异常
    /// ServerException            服务器未能如期处理异常
    /// ArgumentNullException       参数不能为null异常
    /// </summary>
    public static class HttpHelper
    {
        public static bool CanDebug = true;


        /// <summary>
        /// 发生了未授权的接口调用
        /// </summary>
        public static event EventHandler<string> UnauthorizedCall;

        #region 解决证书问题

        // 尝试解决部分客户端下载文件时发生异常  Received an unexpected EOF or 0 bytes from the transport stream.
        // 应该是 ssl 证书的问题
        // https://forums.asp.net/t/2054166.aspx?Received+an+unexpected+EOF+or+0+bytes+from+the+transport+stream
        static HttpHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
        }

        private static bool ValidateServerCertificate(object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }

        #endregion

        #region Get

//        public static Task<T> HttpGetJsonAsync<T>(
//            string url,
//            string json,
//            int timeOut = 300000
//        )
//        {
//            return HttpGetJsonAsync<T>(url, json, null, timeOut);
//        }


//        public static async Task<T> HttpGetJsonAsync<T>(
//            string url,
//            string json,
//            string token,
//            int timeOut = 300000,
//            bool debug = true
//        )
//        {
//            var data = Encoding.UTF8.GetBytes(json);

//            //#if DEBUG
//            //            if (CanDebug)
//            //                Debug.Print(
//            //                    $"HttpGetJson请求发起:{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}");

//            //#endif


//            var startDateTime = DateTime.Now;

//            string resultJson;
//            try
//            {
//                resultJson = await HttpMethodAsync(
//                    url,
//                    data,
//                    null,
//                    null,
//                    string.Empty,
//                    timeOut,
//                    token,
//                    "application/json",
//                    "Get");


//#if DEBUG
//                var timeSpan = DateTime.Now - startDateTime;
//                if (CanDebug && debug)
//                    Debug.Print(
//                        $"{Environment.NewLine}HttpGetJson请求完成: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
//#endif
//            }
//            catch (Exception exception)
//            {
//#if DEBUG
//                var timeSpan = DateTime.Now - startDateTime;
//                if (CanDebug)
//                    Debug.Print(
//                        $"{Environment.NewLine}HttpGetJson请求失败: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}错误信息是{Environment.NewLine}{exception.Message}{Environment.NewLine}");
//#endif
//                throw;
//            }


//            var result = ParesJson<T>(resultJson);


//            return result;
//        }

//        public static async Task<T> HttpGetJsonAsync<T>(
//            string url,
//            string json,
//            CookieCollection cookies,
//            NameValueCollection headers,
//            string userAgent,
//            int timeout,
//            string token
//        )
//        {
//            var data = Encoding.UTF8.GetBytes(json);

//            var resultJson = await HttpMethodAsync(url, data, cookies, headers, userAgent, timeout, token,
//                "application/json",
//                "Get");
//            return ParesJson<T>(resultJson);
//        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url)
        {
            return await HttpGetAsync<T>(url, new NameValueCollection());
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url,
            string token,
            bool debug = true
        )
        {
            return await HttpGetAsync<T>(url, new NameValueCollection(), token, debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url,
            string paramName1,
            string paramValue1
        )
        {
            return await HttpGetAsync<T>(url, new NameValueCollection
            {
                {paramName1, paramValue1}
            });
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url,
            string paramName1,
            string paramValue1,
            string token,
            bool debug = true
        )
        {
            return await HttpGetAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1}
                },
                token, debug: debug
            );
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2,
            bool debug = true
        )
        {
            return await HttpGetAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2,
            string token
        )
        {
            return await HttpGetAsync<T>(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                token
            );
        }

        public static async Task<T> HttpGetAsync<T>(string url,
            NameValueCollection @params,
            bool debug = true
        )
        {
            return await HttpGetAsync<T>(url, @params, string.Empty, debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string url,
            NameValueCollection @params,
            string token,
            bool debug = true
        )
        {
            var json = await HttpGetAsync(url, @params, token, debug: debug);

            return ParesJson<T>(json);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url)
        {
            return await HttpGetAsync(url, new NameValueCollection());
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url,
            string token
        )
        {
            return await HttpGetAsync(url, new NameValueCollection(), token);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url,
            string paramName,
            string paramValue
        )
        {
            return await HttpGetAsync(
                url,
                new NameValueCollection
                {
                    {paramName, paramValue}
                });
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url,
            string paramName,
            string paramValue,
            string token
        )
        {
            return await HttpGetAsync(
                url,
                new NameValueCollection
                {
                    {paramName, paramValue}
                },
                token);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2
        )
        {
            return await HttpGetAsync(url, new NameValueCollection
            {
                {paramName1, paramValue1},
                {paramName2, paramValue2}
            });
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="paramName1">参数名称</param>
        /// <param name="paramValue1">参数值</param>
        /// <param name="paramName2"></param>
        /// <param name="paramValue2"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2,
            string token,
            bool debug = true
        )
        {
            return await HttpGetAsync(
                url,
                new NameValueCollection
                {
                    {paramName1, paramValue1},
                    {paramName2, paramValue2}
                },
                token, debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url,
            NameValueCollection @params,
            bool debug = true
        )
        {
            return await HttpGetAsync(url, @params, string.Empty, debug: debug);
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url,
            NameValueCollection @params,
            string token,
            bool debug = true
        )
        {
            return await HttpGetAsync(url, @params, null, null, null, 300000, token, debug: debug);
        }

        public static async Task<string> HttpGetAsync(
            string url,
            NameValueCollection @params,
            CookieCollection cookies,
            NameValueCollection headers,
            string userAgent,
            int timeout,
            string token,
            bool debug = true
        )
        {
            return await HttpGetAsync(url, @params, cookies, headers, userAgent, timeout, token, ResponseConverter,
                debug);

            string ResponseConverter(HttpWebResponse response)
            {
                if (response == null)
                {
                    throw new HttpException(
                        "网络异常,请稍后再试.",
                        $"接口调用异常,服务器没有响应.{Environment.NewLine}url:{url}{Environment.NewLine}"
                    );
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                }

                throw new HttpException(
                    "网络异常,请稍后再试.",
                    $"接口调用异常{Environment.NewLine}url:{url}{Environment.NewLine}",
                    response.StatusCode
                );
            }
        }

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <param name="headers"></param>
        /// <param name="userAgent"></param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="token"></param>
        /// <param name="responseConverter">http答复转换器,通过该转换器将http结果转换为需要的结果</param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(
            string url,
            NameValueCollection @params,
            CookieCollection cookies,
            NameValueCollection headers,
            string userAgent,
            int timeout,
            string token,
            Func<HttpWebResponse, T> responseConverter,
            bool debug = true
        )
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            url += GetParamsString(@params);

            HttpWebRequest request = null;
            try
            {
                request = HttpWebRequestFactory.CreateHttpWebRequest(url, "Get",
                    "application/x-www-form-urlencoded", headers,
                    timeout, userAgent, cookies);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Add(HttpRequestHeader.Authorization, token);
                }

                try
                {
                    using var response = (HttpWebResponse) await request.GetResponseAsync();
                    var result = responseConverter.Invoke(response);

                    if (CanDebug && debug)
                        Debug.Print(
                            $"{Environment.NewLine}HttpGet请求成功:{Environment.NewLine}Url:\t{url}{Environment.NewLine}Token:\t{token}{Environment.NewLine}返回json是:{Environment.NewLine}{result}{Environment.NewLine}");

                    return result;
                }
                catch (Exception e)
                {
                    if (CanDebug)
                        Debug.Print(
                            $"{Environment.NewLine}HttpGet请求失败:{Environment.NewLine}Url:\t{url}{Environment.NewLine}Token:\t{token}{Environment.NewLine}Message:\t{token}{e.Message}");
                    throw;
                }
            }
            catch (WebException webException)
            {
                if (webException.Response is HttpWebResponse {StatusCode: HttpStatusCode.Unauthorized})
                {
                    UnauthorizedCall?.Invoke(null, token);
                }

                throw;
            }
            finally
            {
                request?.Abort();
            }
        }

        #endregion Get

        /// <summary>
        /// 将参数集合平铺为字符串
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        private static string GetParamsString(NameValueCollection @params)
        {
            var stringBuilder = new StringBuilder();

            var paramsString = string.Empty;
            if (@params != null && @params.Count > 0)
            {
                var isFirst = true;

                foreach (string key in @params.Keys)
                {
                    stringBuilder.AppendFormat(
                        isFirst ? "?{0}={1}" : "&{0}={1}",
                        key,
                        @params[key]);
                    isFirst = false;
                }
            }

            if (stringBuilder.Length > 0)
            {
                paramsString = stringBuilder.ToString();
            }

            return paramsString;
        }

        #region Post

        //public static string HttpPostUploadFile(string url,
        //    string filePath
        //)
        //{
        //    return HttpPostUploadFile(url, filePath, string.Empty);
        //}

        //public static string HttpPostUploadFile(string url,
        //    string filePath,
        //    string token
        //)
        //{
        //    var timeout = 60000;

        //    return HttpPostUploadFile(url, filePath, token, timeout);
        //}

        //public static Task<T> HttpPostUploadFile<T>(string url,
        //    string filePath,
        //    string token,
        //    int timeout
        //)
        //{
        //    if (string.IsNullOrWhiteSpace(filePath))
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(filePath), $@"没有传入有效的文件名.文件名:{filePath}");
        //    }

        //    if (!File.Exists(filePath))
        //    {
        //        throw new FileNotFoundException("文件未找到", filePath);
        //    }

        //    //请求头部信息
        //    var strFileName = string.Empty;
        //    if (!string.IsNullOrWhiteSpace(filePath))
        //    {
        //        strFileName = Path.GetFileName(filePath);
        //    }

        //    //写文件流
        //    using Stream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);


        //    return HttpPostUploadFile<T>(url, token, timeout, strFileName, fs);
        //}

        public static Task<T> HttpPostUploadFile<T>(string url, string token, int timeout, string fileName,
            NameValueCollection nameValueCollection, byte[] bytes)
        {
            var stream = new MemoryStream(bytes);

            return HttpPostUploadFile<T>(url, token, timeout, fileName, nameValueCollection, stream);
        }

        public static async Task<T> HttpPostUploadFile<T>(string url, string token, int timeout, string fileName,
            NameValueCollection nameValueCollection, Stream stream)
        {
            string returnJson;

            HttpWebRequest httpWebRequest = null;
            HttpWebResponse response = null;
            Stream requestStream = null;

            try
            {
                //边界符
                var boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                var contentType = $"multipart/form-data;boundary={boundary}";
                httpWebRequest = HttpWebRequestFactory.CreateHttpWebRequest(url, "Post", contentType, null, timeout);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, token);
                }

                //httpWebRequest.KeepAlive = true;
                httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
                httpWebRequest.AllowWriteStreamBuffering = false; //对发送的数据不使用缓存
                httpWebRequest.SendChunked = true;

                //---------------------------------开始填写请求数据流

                //------------body部分
                requestStream = await httpWebRequest.GetRequestStreamAsync();

                var boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                var formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                foreach (string key in nameValueCollection.Keys)
                {
                    await requestStream.WriteAsync(boundaryBytes, 0, boundaryBytes.Length);

                    var formItem = string.Format(formDataTemplate, key, nameValueCollection[key]);

                    var formItemBytes = Encoding.UTF8.GetBytes(formItem);

                    await requestStream.WriteAsync(formItemBytes, 0, formItemBytes.Length);
                }

                await requestStream.WriteAsync(boundaryBytes, 0, boundaryBytes.Length);


                //------------Header部分

                var header =
                    $"Content-Disposition: form-data; name=\"file\"; filename=\"{fileName}\"\r\nContent-Type: {contentType}\r\n\r\n";

                var headerBytes = Encoding.UTF8.GetBytes(header);
                await requestStream.WriteAsync(headerBytes, 0, headerBytes.Length);


                //------------ 写入要上传的文件

                long bufferLength = 4 * 1024; //每次上传4K
                if (bufferLength > stream.Length)
                {
                    bufferLength = stream.Length;
                }

                var buffer = new byte[bufferLength];
                int readBytes;
                while ((readBytes = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await requestStream.WriteAsync(buffer, 0, readBytes);
                }

                //------------写入结束符
                var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                await requestStream.WriteAsync(trailer, 0, trailer.Length);

                requestStream.Close();

                //------------发送请求 得到回复
                response = httpWebRequest.GetResponse() as HttpWebResponse;

                //------------处理回复
                if (response == null)
                {
                    throw new HttpException("网络异常,请稍后再试.",
                        $"接口调用异常,服务器没有响应.{Environment.NewLine}url:{url}{Environment.NewLine}"
                    );
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseStream = response.GetResponseStream();
                    if (responseStream == null)
                    {
                        throw new HttpException("网络异常,请稍后再试.", "接口调用异常,没有返回 responseStream");
                    }

                    using var reader = new StreamReader(responseStream, Encoding.UTF8);
                    returnJson = await reader.ReadToEndAsync();

                    Debug.Print(
                        $"httpPost 上传成功{Environment.NewLine}URL:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}结果:{returnJson}");
                    return ParesJson<T>(returnJson);
                }
                else
                {
                    throw new HttpException(
                        "网络异常,请稍后再试.",
                        $"接口调用异常{Environment.NewLine}url:{url}{Environment.NewLine}",
                        response.StatusCode
                    );
                }
            }
            catch (WebException webException)
            {
                if (webException.Response is HttpWebResponse {StatusCode: HttpStatusCode.Unauthorized})
                {
                    UnauthorizedCall?.Invoke(null, token);
                }

                throw;
            }
            catch (Exception e)
            {
                Debug.Print(
                    $"httpPost 上传失败{Environment.NewLine}URL:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}{e.Message}");
                throw;
            }
            finally
            {
                httpWebRequest?.Abort();
                response?.Close();
                requestStream?.Close();
                requestStream?.Dispose();
            }
        }

        public static async Task<T> HttpPostAsync<T>(
            string url,
            string token,
            int timeOut = 300000
        )
        {
            string resultJson;
            try
            {
                resultJson = await HttpMethodAsync(
                    url,
                    null,
                    null,
                    null,
                    string.Empty,
                    timeOut,
                    token,
                    "application/json",
                    "Post");


                var result = ParesJson<T>(resultJson);

#if DEBUG
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostAsync请求完成:{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif

                return result;
            }
            catch (Exception exception)
            {
#if DEBUG
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostAsync请求失败:{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}错误信息是{Environment.NewLine}{exception.Message}{Environment.NewLine}");
#endif
                throw;
            }
        }

        public static Task<T> HttpPostJsonAsync<T>(
            string url,
            string json,
            int timeOut = 300000
        )
        {
            return HttpPostJsonAsync<T>(url, json, null, timeOut);
        }

        public static async Task<T> HttpPostJsonAsync<T>(
            string url,
            string json,
            string token,
            int timeOut = 300000,
            bool debug = true
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            //#if DEBUG
            //            if (CanDebug)
            //                Debug.Print(
            //                    $"HttpPostJson请求发起:{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}");

            //#endif


            var startDateTime = DateTime.Now;

            string resultJson;
            try
            {
                resultJson = await HttpMethodAsync(
                    url,
                    data,
                    null,
                    null,
                    string.Empty,
                    timeOut,
                    token,
                    "application/json",
                    "Post");


                var result = ParesJson<T>(resultJson);
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug && debug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求完成: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif

                return result;
            }
            catch (Exception exception)
            {
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求失败: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}错误信息是{Environment.NewLine}{exception.Message}{Environment.NewLine}");
#endif
                throw;
            }
        }


        public static async Task HttpPostJsonAsync(
            string url,
            string json,
            string token,
            int timeOut = 300000,
            bool debug = true
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            //#if DEBUG
            //            if (CanDebug)
            //                Debug.Print(
            //                    $"HttpPostJson请求发起:{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}");

            //#endif


            var startDateTime = DateTime.Now;

            string resultJson;
            try
            {
                resultJson = await HttpMethodAsync(
                    url,
                    data,
                    null,
                    null,
                    string.Empty,
                    timeOut,
                    token,
                    "application/json",
                    "Post");


#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug && debug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求完成: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif
            }
            catch (Exception exception)
            {
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求失败: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}错误信息是{Environment.NewLine}{exception.Message}{Environment.NewLine}");
#endif
                throw;
            }

            ParesJson(resultJson);
        }

        public static async Task<T> HttpPostJsonAsync<T>(
            string url,
            string json,
            CookieCollection cookies,
            NameValueCollection headers,
            string userAgent,
            int timeout,
            string token
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            var resultJson = await HttpMethodAsync(url, data, cookies, headers, userAgent, timeout, token,
                "application/json",
                "Post");
            return ParesJson<T>(resultJson);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url)
        {
            var @params = new NameValueCollection();

            return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, string.Empty);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url,
            string token
        )
        {
            var @params = new NameValueCollection();

            return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, token);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url,
            string paramName1,
            string paramValue1
        )
        {
            return await HttpPostXWwwFormUrlencodedAsync<T>(url, paramName1, paramValue1, string.Empty);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url,
            string paramName1,
            string paramValue1,
            string token
        )
        {
            var @params = new NameValueCollection
            {
                {paramName1, paramValue1}
            };

            return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, token);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2
        )
        {
            return await HttpPostXWwwFormUrlencodedAsync<T>(url, paramName1, paramValue1, paramName2, paramValue2,
                string.Empty);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2,
            string token
        )
        {
            var @params = new NameValueCollection
            {
                {paramName1, paramValue1},
                {paramName2, paramValue2}
            };

            return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, token);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url, NameValueCollection @params
        )
        {
            return await HttpPostXWwwFormUrlencodedAsync<T>(url, @params, string.Empty);
        }

        public static async Task<T> HttpPostXWwwFormUrlencodedAsync<T>(string url,
            NameValueCollection @params,
            string token
        )
        {
            var json = string.Empty;
            try
            {
                json = await HttpPostXWwwFormUrlencodedAsync(url, @params, token);

                var result = ParesJson<T>(json);

                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPost请求成功:{Environment.NewLine}Url:{url}{Environment.NewLine}Params:{Environment.NewLine}{@params.AllKeys.ToDictionary(k => k, k => @params[k]).ToIndentedJson()}{Environment.NewLine}Token:{token}{Environment.NewLine}返回json是{Environment.NewLine}{json}{Environment.NewLine}");

                return result;
            }
            catch (Exception)
            {
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPost请求失败:{Environment.NewLine}Url:{url}{Environment.NewLine}Params:{Environment.NewLine}{@params.AllKeys.ToDictionary(k => k, k => @params[k]).ToIndentedJson()}{Environment.NewLine}Token:{token}{Environment.NewLine}返回json是{Environment.NewLine}{json}{Environment.NewLine}");
                throw;
            }
        }

        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
            string paramName,
            string paramValue
        )
        {
            return await HttpPostXWwwFormUrlencodedAsync(url, paramName, paramValue, string.Empty);
        }

        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
            string paramName,
            string paramValue,
            string token
        )
        {
            var @params = new NameValueCollection
            {
                {paramName, paramValue}
            };

            return await HttpPostXWwwFormUrlencodedAsync(url, @params, token);
        }

        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2
        )
        {
            return await HttpPostXWwwFormUrlencodedAsync(url, paramName1, paramValue1, paramName2, paramValue2,
                string.Empty);
        }

        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
            string paramName1,
            string paramValue1,
            string paramName2,
            string paramValue2,
            string token
        )
        {
            var @params = new NameValueCollection
            {
                {paramName1, paramValue1},
                {paramName2, paramValue2}
            };

            return await HttpPostXWwwFormUrlencodedAsync(url, @params, token);
        }

        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
            NameValueCollection @params
        )
        {
            return await HttpPostXWwwFormUrlencodedAsync(url, @params, string.Empty);
        }

        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(string url,
            NameValueCollection @params,
            string token
        )
        {
            return await HttpPostXWwwFormUrlencodedAsync(url, @params, null, null, null, 300000, token);
        }

        /// <summary>
        /// 创建POST方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">随同请求POST的参数名称及参数值字典</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <param name="headers">头信息</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(
            string url,
            NameValueCollection @params,
            CookieCollection cookies,
            NameValueCollection headers,
            string userAgent,
            int timeout,
            string token
        )
        {
            var data = GetRequestBytes(@params);
            var result = await HttpMethodAsync(url, data, cookies, headers, userAgent, timeout, token,
                "application/x-www-form-urlencoded", "POST");

            return result;
        }

        #endregion Post


        #region Put

        public static async Task<T> HttpPutJsonAsync<T>(
            string url,
            string json,
            string token,
            int timeOut = 300000
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            //#if DEBUG
            //            if (CanDebug)
            //                Debug.Print(
            //                    $"HttpPutJson请求发起:{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}");

            //#endif


            var startDateTime = DateTime.Now;

            string resultJson;
            try
            {
                resultJson = await HttpMethodAsync(
                    url,
                    data,
                    null,
                    null,
                    string.Empty,
                    timeOut,
                    token,
                    "application/json",
                    "Put");

                var result = ParesJson<T>(resultJson);

#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPutJson请求完成: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif


                return result;
            }
            catch (Exception exception)
            {
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPutJson请求失败: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}错误信息是{Environment.NewLine}{exception.Message}{Environment.NewLine}");
#endif
                throw;
            }
        }

        public static async Task<T> HttpPutJsonAsync<T>(
            string url,
            string json,
            CookieCollection cookies,
            NameValueCollection headers,
            string userAgent,
            int timeout,
            string token
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            var resultJson = await HttpMethodAsync(url, data, cookies, headers, userAgent, timeout, token,
                "application/json",
                "Put");
            return ParesJson<T>(resultJson);
        }

        #endregion


        #region delete

        public static async Task HttpDeleteAsync(
            string url,
            string token
        )
        {
            string resultJson;

            try
            {
                resultJson = await HttpMethodAsync(url, null, null, null, null, 300000, token,
                    "application/x-www-form-urlencoded",
                    "DELETE");

                ParesJson<object>(resultJson);

                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpDelete请求成功:{Environment.NewLine}Url:{url}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
            }
            catch
            {
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpDelete请求失败:{Environment.NewLine}Url:{url}{Environment.NewLine}");
                throw;
            }
        }

        #endregion


        /// <summary>
        /// 将名称与值的数据集转换为字节数组(UTF8编码)
        /// </summary>
        /// <param name="dicPostParameters"></param>
        /// <returns></returns>
        private static byte[] GetRequestBytes(NameValueCollection dicPostParameters)
        {
            if (dicPostParameters == null || dicPostParameters.Count == 0)
            {
                return Array.Empty<byte>();
            }

            var sbParams = new StringBuilder();
            foreach (string key in dicPostParameters.Keys)
            {
                sbParams.AppendFormat("{0}={1}&", key, dicPostParameters[key]);
            }

            sbParams.Length -= 1; //删除最后一个"&"

            return Encoding.UTF8.GetBytes(sbParams.ToString());
        }

        private static void ParesJson(string json)
        {
            var httpResponseBase = JsonConvert.DeserializeObject<HttpResponseBase>(json);

            if (httpResponseBase is null)
            {
                throw new HttpException("网络异常,请稍后再试.", "服务器返回的数据结构不符合要求.");
            }

            if (httpResponseBase.Code == 200)
            {
                return;
            }

            //根据与服务端的约定,在任何情况下 Message 都应该是友好提示,所以此处直接将 Message 包装为 HttpException,向用户展示.
            if (string.IsNullOrWhiteSpace(httpResponseBase.Message))
            {
                throw new HttpException("网络异常,请稍后再试.", httpResponseBase.Code);
            }

            throw new HttpException(httpResponseBase.Message, httpResponseBase.Code);
        }

        private static T ParesJson<T>(string json)
        {
            //由于服务在在返回错误时,有可能在失败时将data设为null而不是代表失败的false,
            //所以首先要通过返回码判断是否调用成功,只有成功时才能将data序列化为所希望的对象类型
            var httpResponseBase = JsonConvert.DeserializeObject<HttpResponseBase>(json);

            if (httpResponseBase is null)
            {
                throw new HttpException("网络异常,请稍后再试.", "服务器返回的数据结构不符合要求.");
            }

            if (httpResponseBase.Code == 200)
            {
                HttpResponseObject<T> httpResponseObject;

                try
                {
                    httpResponseObject = JsonConvert.DeserializeObject<HttpResponseObject<T>>(json);
                }
                catch (Exception e)
                {
                    if (CanDebug)
                    {
                        Debug.Print($"Json 反序列化失败.{e.Message}");
                    }

                    throw;
                }

                if (httpResponseObject is null)
                {
                    throw new HttpException("网络异常,请稍后再试.", "服务器返回的数据结构不符合要求.");
                }

                return httpResponseObject.Data;
            }

            //根据与服务端的约定,在任何情况下 Message 都应该是友好提示,所以此处直接将 Message 包装为 HttpException,向用户展示.
            if (string.IsNullOrWhiteSpace(httpResponseBase.Message))
            {
                throw new HttpException("网络异常,请稍后再试.", httpResponseBase.Code);
            }

            throw new HttpException(httpResponseBase.Message, httpResponseBase.Code);
        }

        private static string HttpMethod(
            string url,
            byte[] data,
            CookieCollection cookies,
            NameValueCollection headers,
            string userAgent,
            int timeout,
            string token,
            string contentType,
            string methodName
        )
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            var result = string.Empty;

            HttpWebRequest request = null;

            try
            {
                request = HttpWebRequestFactory.CreateHttpWebRequest(
                    url,
                    methodName,
                    contentType,
                    headers,
                    timeout,
                    userAgent,
                    cookies);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Add(HttpRequestHeader.Authorization, token);
                }

                if (data is {Length: > 0})
                {
                    using var dataStream = request.GetRequestStream();
                    dataStream.Write(data, 0, data.Length);
                }

                var response = request.GetResponse() as HttpWebResponse;

                if (response == null)
                {
                    throw new HttpException(
                        "网络异常,请稍后再试.",
                        $"接口调用异常,服务器没有响应.{Environment.NewLine}url:{url}{Environment.NewLine}"
                    );
                }


                //发送请求,并得到回复
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        result = reader.ReadToEnd();
                    }
                }
                else
                {
                    throw new HttpException(
                        "网络异常,请稍后再试.",
                        $"接口调用异常{Environment.NewLine}url:{url}{Environment.NewLine}",
                        response.StatusCode
                    );
                }
            }
            catch (WebException webException)
            {
                if (webException.Response is HttpWebResponse {StatusCode: HttpStatusCode.Unauthorized})
                {
                    UnauthorizedCall?.Invoke(null, token);
                }

                throw;
            }
            finally
            {
                request?.Abort();
            }

            return result;
        }


        private static async Task<string> HttpMethodAsync(
            string url,
            byte[] data,
            CookieCollection cookies,
            NameValueCollection headers,
            string userAgent,
            int timeout,
            string token,
            string contentType,
            string methodName
        )
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            var result = string.Empty;

            HttpWebRequest request = null;

            try
            {
                request = HttpWebRequestFactory.CreateHttpWebRequest(
                    url,
                    methodName,
                    contentType,
                    headers,
                    timeout,
                    userAgent,
                    cookies);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Add(HttpRequestHeader.Authorization, token);
                }

                if (data is {Length: > 0})
                {
                    using var dataStream = await request.GetRequestStreamAsync();
                    await dataStream.WriteAsync(data, 0, data.Length);
                }


                if (await request.GetResponseAsync() is not HttpWebResponse response)
                {
                    throw new HttpException(
                        "网络异常,请稍后再试.",
                        $"接口调用异常,服务器没有响应.{Environment.NewLine}url:{url}{Environment.NewLine}"
                    );
                }

                //发送请求,并得到回复
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        result = await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    throw new HttpException(
                        "网络异常,请稍后再试.",
                        $"接口调用异常{Environment.NewLine}url:{url}{Environment.NewLine}",
                        response.StatusCode
                    );
                }
            }
            catch (WebException webException)
            {
                if (webException.Response is HttpWebResponse {StatusCode: HttpStatusCode.Unauthorized})
                {
                    UnauthorizedCall?.Invoke(null, token);
                }

                try
                {
                    using var stream = webException.Response.GetResponseStream();
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream, Encoding.UTF8);
                        var responseJson = await reader.ReadToEndAsync();

                        var httpResponseBase = JsonConvert.DeserializeObject<HttpResponseBase>(responseJson);

                        if (httpResponseBase is not null)
                        {
                            throw new HttpException(httpResponseBase.Message, httpResponseBase.Code);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw webException;
                }


                throw;
            }

            catch (Exception ex)
            {
                //方便断点调试
                throw;
            }
            finally
            {
                request?.Abort();
            }

            return result;
        }
    }
}