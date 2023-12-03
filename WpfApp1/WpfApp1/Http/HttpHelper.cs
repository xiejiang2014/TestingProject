using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace WpfApp1.Http
{
    /// <summary>
    /// 调用本类的方法时,主要可能会发生以下异常,注意处理
    /// System.Net.WebException     通信异常
    /// ServerException            服务器未能如期处理异常
    /// ArgumentNullException       参数不能为null异常
    /// </summary>
    public static partial class HttpHelper
    {
        public static bool CanDebug = true;

        public static NameValueCollection DefaultHeaders = new NameValueCollection();

        /// <summary>
        /// 发生了未授权的接口调用
        /// </summary>
        public static event EventHandler<string> UnauthorizedCall;

        public static Action<string, bool> Log;

        #region 解决证书问题

        // 尝试解决部分客户端下载文件时发生异常  Received an unexpected EOF or 0 bytes from the transport stream.
        // 应该是 ssl 证书的问题
        // https://forums.asp.net/t/2054166.aspx?Received+an+unexpected+EOF+or+0+bytes+from+the+transport+stream
        static HttpHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
        }

        private static bool ValidateServerCertificate(object          sender,
                                                      X509Certificate certificate,
                                                      X509Chain       chain,
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
        /// <param name="debug">是否输出调试信息</param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(
            string                    url,
            NameValueCollection       @params,
            CookieCollection?         cookies,
            NameValueCollection?      headers,
            string?                   userAgent,
            int                       timeout,
            string                    token,
            Func<string, T>           responseConverter,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            var startDateTime = DateTime.Now;

            url += GetParamsString(@params);

            HttpWebRequest? request = null;
            try
            {
                try
                {
                    var json = await GetHttpResponseTextAsync(url, null, cookies, headers, userAgent, timeout, token,
                                                              "application/x-www-form-urlencoded",
                                                              "Get");

                    var result = responseConverter.Invoke(json);

                    var timeSpan = DateTime.Now - startDateTime;
                    if (CanDebug && debug)
                        Debug.Print(
                            $"{Environment.NewLine}HttpGet请求成功: 耗时:{timeSpan.TotalMilliseconds}  funcName:{funcName}{Environment.NewLine}Url:\t{url}{Environment.NewLine}Token:\t{token}{Environment.NewLine}返回json是:{Environment.NewLine}{result}{Environment.NewLine}");

                    return result;
                }
                catch (Exception e)
                {
                    var timeSpan = DateTime.Now - startDateTime;
                    if (CanDebug)
                        Debug.Print(
                            $"{Environment.NewLine}HttpGet请求失败:耗时:{timeSpan.TotalMilliseconds}  funcName:{funcName}{Environment.NewLine}Url:\t{url}{Environment.NewLine}Token:\t{token}{Environment.NewLine}Message:\t{token}{e.Message}");
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

        #region Post

        //可用 但也无法解决日新上传文件时出现的速度波动问题

        public static async Task<T> HttpPostUploadFileV3<T>(
            string              url,
            string              token,
            int                 timeout,
            string              fileName,
            NameValueCollection nameValueCollection,
            Stream              stream,
            Action<long>?       sendProgressChanged = null,
            Canceller?          canceller           = null)
        {
            using var progressMessageHandler = new System.Net.Http.Handlers.ProgressMessageHandler();
            using var httpClient             = HttpClientFactory.Create(progressMessageHandler);

            progressMessageHandler.HttpSendProgress += (s, e) => { sendProgressChanged?.Invoke(e.BytesTransferred); };

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", token);
            }

            if (DefaultHeaders is {Count: > 0})
            {
                foreach (string key in DefaultHeaders.Keys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        httpClient.DefaultRequestHeaders.Add(key, DefaultHeaders[key]);
                    }
                }
            }

            var content = new MultipartFormDataContent();

            foreach (string key in nameValueCollection.Keys)
            {
                content.Add(new StringContent(nameValueCollection[key]), key);
            }

            content.Add(new StreamContent(stream), "file", fileName);

            await content.LoadIntoBufferAsync();

            var cts = new CancellationTokenSource();

            if (canceller is not null)
            {
                canceller.OnCancel += OnCancellerOnOnCancel;
            }

            try
            {
                var httpResponseMessage = await httpClient.PostAsync(url, content, cts.Token);

                var responeText = httpResponseMessage.Content.ReadAsStringAsync().Result;
                return ParesJson<T>(responeText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (canceller is not null)
                {
                    canceller.OnCancel -= OnCancellerOnOnCancel;
                }
            }


            void OnCancellerOnOnCancel(object o, EventArgs args)
            {
                cts.Cancel();
            }
        }

        public static async Task<T> HttpPostUploadFile<T>(
            string              url,
            string              token,
            int                 timeout,
            string              fileName,
            NameValueCollection nameValueCollection,
            Stream              stream,
            Action<long>?       sendProgressChanged = null,
            Canceller?          canceller           = null)
        {
            string returnJson;

            HttpWebRequest?  httpWebRequest = null;
            HttpWebResponse? response       = null;
            Stream?          requestStream  = null;

            try
            {
                //边界符
                var boundary    = "----------" + DateTime.Now.Ticks.ToString("x");
                var contentType = $"multipart/form-data;boundary={boundary}";
                httpWebRequest = HttpWebRequestFactory.CreateHttpWebRequest(
                    url,
                    "POST",
                    contentType,
                    null,
                    timeout
                );


                if (!string.IsNullOrWhiteSpace(token))
                {
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, token);
                }

                if (DefaultHeaders is {Count: > 0})
                {
                    foreach (string key in DefaultHeaders.Keys)
                    {
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            httpWebRequest.Headers.Add(key, DefaultHeaders[key]);
                        }
                    }
                }


                httpWebRequest.ProtocolVersion           = HttpVersion.Version11;
                httpWebRequest.Credentials               = CredentialCache.DefaultCredentials;
                httpWebRequest.AllowWriteStreamBuffering = false; //对发送的数据不使用缓存
                httpWebRequest.SendChunked               = true;
                //httpWebRequest.Proxy                     = null;


                //---------------------------------开始填写请求数据流

                //------------
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


                //------------

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

                var  buffer = new byte[bufferLength];
                int  readBytes;
                long sendLength = 0;
                while ((readBytes = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    if (canceller is not null && canceller.IsCanceled)
                    {
                        throw new OperationCanceledException(canceller.Reason);
                    }

                    await requestStream.WriteAsync(buffer, 0, readBytes);
                    sendLength += readBytes;
                    sendProgressChanged?.Invoke(sendLength);

                    //Debug.Print($"httpPost 文件上传进度 {sendLength}.");
                }

                //------------写入结束符
                var trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                await requestStream.WriteAsync(trailer, 0, trailer.Length);

                requestStream.Close();

                //------------发送请求 得到回复
                Debug.Print($"httpPost 等待上传回复.");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                //var result =  httpWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);

                //httpWebRequest.EndGetResponse(result);

                response = httpWebRequest.GetResponse() as HttpWebResponse;
                stopwatch.Stop();
                Debug.Print($"httpPost 收到上传回复.等待响应耗时:{stopwatch.ElapsedMilliseconds} 毫秒");
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


        public static async Task HttpPostJsonAsync(
            string                    url,
            string                    json,
            string                    token,
            int                       timeOut  = 300000,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            var startDateTime = DateTime.Now;

            string resultJson;

            try
            {
                resultJson = await GetHttpResponseTextAsync(
                    url,
                    data,
                    null,
                    null,
                    null,
                    timeOut,
                    token,
                    "application/json",
                    "Post");

#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug && debug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求成功: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求失败: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}错误信息是{Environment.NewLine}{e.Message}{Environment.NewLine}");
#endif
                throw;
            }


            ParesJson(resultJson);
        }

        public static async Task<T> HttpPostJsonAsync<T>(
            string                    url,
            string                    json,
            CookieCollection?         cookies,
            NameValueCollection?      headers,
            string?                   userAgent,
            int                       timeout,
            string                    token,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            var startDateTime = DateTime.Now;

            string resultJson;

            try
            {
                resultJson = await GetHttpResponseTextAsync(
                    url,
                    data,
                    cookies,
                    headers,
                    userAgent,
                    timeout,
                    token,
                    "application/json",
                    "Post");

#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug && debug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求成功: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostJson请求失败: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}错误信息是{Environment.NewLine}{e.Message}{Environment.NewLine}");
#endif
                throw;
            }

            return ParesJson<T>(resultJson);
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
        /// <param name="debug"></param>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostXWwwFormUrlencodedAsync(
            string                    url,
            NameValueCollection       @params,
            CookieCollection?         cookies,
            NameValueCollection?      headers,
            string?                   userAgent,
            int                       timeout,
            string                    token,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            var data = GetRequestBytes(@params);

            var startDateTime = DateTime.Now;
            var resultJson    = string.Empty;
            try
            {
                resultJson = await GetHttpResponseTextAsync(
                    url,
                    data,
                    cookies,
                    headers,
                    userAgent,
                    timeout,
                    token,
                    "application/x-www-form-urlencoded",
                    "POST");

#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug && debug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostForm请求成功: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif
            }
            catch (Exception e)
            {
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPostForm请求失败: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}错误信息是{Environment.NewLine}{resultJson}{Environment.NewLine}{e.Message}{Environment.NewLine}");
#endif
                throw;
            }

            return resultJson;
        }

        #endregion Post


        #region Put

        public static async Task<string> HttpPutStreamData(string        url,
                                                           string        token,
                                                           int           timeout,
                                                           Stream        stream,
                                                           Action<long>? sendProgressChanged = null,
                                                           Canceller?    canceller           = null)
        {
            string returnJson;

            HttpWebRequest?  httpWebRequest = null;
            HttpWebResponse? response       = null;
            Stream?          requestStream  = null;

            ////强行使用内网地址进行上传
            //url = url.Replace("https://wetest.cdi.citic", "http://10.37.14.111:20000");


            try
            {
                httpWebRequest = HttpWebRequestFactory.CreateHttpWebRequest(
                    url,
                    "PUT",
                    string.Empty,
                    null,
                    timeout
                );

                if (!string.IsNullOrWhiteSpace(token))
                {
                    httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, token);
                }

                if (DefaultHeaders is {Count: > 0})
                {
                    foreach (string key in DefaultHeaders.Keys)
                    {
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            httpWebRequest.Headers.Add(key, DefaultHeaders[key]);
                        }
                    }
                }

                httpWebRequest.ContentLength             = stream.Length;
                httpWebRequest.ProtocolVersion           = HttpVersion.Version11;
                httpWebRequest.Credentials               = CredentialCache.DefaultCredentials;
                httpWebRequest.AllowWriteStreamBuffering = false; //对发送的数据不使用缓存
                httpWebRequest.SendChunked               = true;

                //------------ 写入要传输的数据
                requestStream = await httpWebRequest.GetRequestStreamAsync();
                long bufferLength = 4 * 1024; //每次上传4K
                if (bufferLength > stream.Length)
                {
                    bufferLength = stream.Length;
                }

                var  buffer = new byte[bufferLength];
                int  readBytes;
                long sendLength = 0;
                while ((readBytes = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    if (canceller is not null && canceller.IsCanceled)
                    {
                        throw new OperationCanceledException(canceller.Reason);
                    }

                    await requestStream.WriteAsync(buffer, 0, readBytes);
                    sendLength += readBytes;
                    sendProgressChanged?.Invoke(sendLength); //文件上传进度变化
                }

                requestStream.Close();
                //------------

                //------------发送请求 得到回复
                Debug.Print($"httpPut 等待上传回复.");
                var stopwatch = new Stopwatch();
                stopwatch.Start();


                response = httpWebRequest.GetResponse() as HttpWebResponse;
                stopwatch.Stop();
                Debug.Print($"httpPut 收到上传回复.等待响应耗时:{stopwatch.ElapsedMilliseconds} 毫秒");
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
                    return response.Headers["ETag"].Trim('\"');
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


        /// <summary>
        /// 创建PUT方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="params">参数</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <param name="headers"></param>
        /// <param name="userAgent"></param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="token"></param>
        /// <param name="responseConverter">http答复转换器,通过该转换器将http结果转换为需要的结果</param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static async Task<T> HttpPutAsync<T>(
            string               url,
            NameValueCollection  @params,
            CookieCollection?    cookies,
            NameValueCollection? headers,
            string?              userAgent,
            int                  timeout,
            string               token,
            Func<string, T>      responseConverter,
            bool                 debug = true
        )
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            url += GetParamsString(@params);


            try
            {
                var json = await GetHttpResponseTextAsync(url,
                                                          null,
                                                          cookies,
                                                          headers,
                                                          userAgent,
                                                          timeout,
                                                          token,
                                                          "application/x-www-form-urlencoded",
                                                          "Put");

                var result = responseConverter.Invoke(json);

                if (CanDebug && debug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPutAsync 请求成功:{Environment.NewLine}Url:\t{url}{Environment.NewLine}Token:\t{token}{Environment.NewLine}返回json是:{Environment.NewLine}{result}{Environment.NewLine}");


                return result;
            }
            catch (Exception e)
            {
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpPut请求失败:{Environment.NewLine}Url:\t{url}{Environment.NewLine}Token:\t{token}{Environment.NewLine}Message:\t{token}{e.Message}");
                throw;
            }
        }

        public static async Task<T> HttpPutJsonAsync<T>(
            string url,
            string json,
            string token,
            int    timeOut = 300000
        )
        {
            var data = Encoding.UTF8.GetBytes(json);

            var startDateTime = DateTime.Now;

            string resultJson;
            try
            {
                resultJson = await GetHttpResponseTextAsync(
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
                        $"{Environment.NewLine}HttpPutJson请求成功: 耗时:{timeSpan.TotalMilliseconds}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}发出的json是{Environment.NewLine}{json}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
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

        #endregion


        #region delete

        public static async Task HttpDeleteAsync(
            string                    url,
            string                    token,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            await HttpDeleteAsync<object>(url, token, debug, funcName);
        }

        public static async Task<T> HttpDeleteAsync<T>(
            string                    url,
            string                    token,
            bool                      debug    = true,
            [CallerMemberName] string funcName = ""
        )
        {
            var    startDateTime = DateTime.Now;
            string resultJson;

            try
            {
                resultJson = await GetHttpResponseTextAsync(url, null, null, null, null, 300000, token,
                                                            "application/x-www-form-urlencoded",
                                                            "DELETE");


#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug && debug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpDeleteAsync请求成功: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}返回json是{Environment.NewLine}{resultJson}{Environment.NewLine}");
#endif
            }
            catch (Exception exception)
            {
#if DEBUG
                var timeSpan = DateTime.Now - startDateTime;
                if (CanDebug)
                    Debug.Print(
                        $"{Environment.NewLine}HttpDeleteAsync请求失败: 耗时:{timeSpan.TotalMilliseconds}    funcName:{funcName}{Environment.NewLine}Url:{url}{Environment.NewLine}Token:{token}{Environment.NewLine}错误信息是{Environment.NewLine}{exception.Message}{Environment.NewLine}");
#endif
                throw;
            }

            return ParesJson<T>(resultJson);
        }

        #endregion


        /// <summary>
        /// 将参数集合平铺为字符串
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        private static string GetParamsString(NameValueCollection @params)
        {
            var stringBuilder = new StringBuilder();

            var paramsString = string.Empty;
            if (@params is {Count: > 0})
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

        /// <summary>
        /// 将名称与值的数据集转换为字节数组(UTF8编码)
        /// </summary>
        /// <param name="dicPostParameters"></param>
        /// <returns></returns>
        private static byte[] GetRequestBytes(NameValueCollection dicPostParameters)
        {
            if (dicPostParameters.Count == 0)
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
                HttpResponseObject<T>? httpResponseObject;

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

        private static async Task<string> GetHttpResponseTextAsync(
            string               url,
            byte[]?              data,
            CookieCollection?    cookies,
            NameValueCollection? headers,
            string?              userAgent,
            int                  timeout,
            string               token,
            string               contentType,
            string               methodName
        )
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            HttpWebRequest? request = null;

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

                if (DefaultHeaders is {Count: > 0})
                {
                    foreach (string key in DefaultHeaders.Keys)
                    {
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            request.Headers.Add(key, DefaultHeaders[key]);
                        }
                    }
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
                    if (stream == null)
                    {
                        throw new HttpException(
                            "网络异常,请稍后再试.",
                            $"接口调用异常,没有返回数据流.{Environment.NewLine}url:{url}{Environment.NewLine}"
                        );
                    }

                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    return await reader.ReadToEndAsync();
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
                if (webException.Response is null)
                {
                    ////if (webException.InnerException is SocketException se)
                    ////{
                    ////    if (se.SocketErrorCode==SocketError.HostUnreachable)
                    ////    {


                    ////    }
                    ////}

                    throw;
                }

                if (webException.Response is HttpWebResponse {StatusCode: HttpStatusCode.Unauthorized})
                {
                    UnauthorizedCall?.Invoke(null, token);
                }


                using var stream = webException.Response.GetResponseStream();
                if (stream != null)
                {
                    using var reader       = new StreamReader(stream, Encoding.UTF8);
                    var       responseJson = await reader.ReadToEndAsync();

                    HttpResponseBase? httpResponseBase = null;
                    try
                    {
                        httpResponseBase = JsonConvert.DeserializeObject<HttpResponseBase>(responseJson);
                    }
                    catch (Exception e)
                    {
                    }

                    if (httpResponseBase is not null)
                    {
                        throw new HttpException(
                            string.IsNullOrWhiteSpace(httpResponseBase.Message)
                                ? webException.Message
                                : httpResponseBase.Message,
                            httpResponseBase.Code,
                            webException
                        );
                    }
                    else
                    {
                        throw;
                    }
                }

                throw;
            }
            catch (System.UriFormatException ex)
            {
                //方便断点调试
                throw new System.UriFormatException($"{ex.Message}\r\n{url}", ex);
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
        }
    }
}