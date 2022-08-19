using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;

namespace ShareDrawing.HttpClient.Http
{
    public class HttpRequestClient
    {
        private readonly RestClient _client;

        public HttpRequestClient(int timeout = 15000)
        {
            _client = new RestClient
            {
                Timeout = timeout
            };
            ConstHeaders = new Dictionary<string, string>();
        }

        public Dictionary<string, string> ConstHeaders { get; }

        public void SetEncoding(Encoding encoding)
        {
            _client.Encoding = encoding;
        }

        public void SetTimeout(int tiemout)
        {
            _client.Timeout = tiemout;
        }

        public async Task<T> QuestAsync<T>(string url, RequestMethod method,
            Dictionary<string, string> queryData = null, object bodyData = null,
            Dictionary<string, string> headers = null)
        {
            _client.BaseUrl = new Uri(url);
            IRestRequest request = CreateRequest(method, queryData, bodyData, headers);

#if DEBUG
            var startDateTime = DateTime.Now;


            var stringBuilder = new StringBuilder();


            try
            {
                IRestResponse response = await _client.ExecuteTaskAsync(request);
                var timeSpan = DateTime.Now - startDateTime;
                stringBuilder.AppendLine($"HTTP {method} 请求成功 耗时:{timeSpan.TotalMilliseconds}");
                stringBuilder.AppendLine($"URL:\r\n{url}");
                stringBuilder.AppendLine($"发出的json是");
                stringBuilder.AppendLine(JsonConvert.SerializeObject(bodyData));
                var result = GetResult<T>(response, stringBuilder);
                Debug.Print(stringBuilder.ToString());
                return result;
            }
            catch (Exception e)
            {
                stringBuilder.Clear();
                var timeSpan = DateTime.Now - startDateTime;
                stringBuilder.AppendLine($"HTTP {method} 请求失败 耗时:{timeSpan.TotalMilliseconds}");
                stringBuilder.AppendLine($"URL:\r\n{url}");
                stringBuilder.AppendLine($"发出的json是");
                stringBuilder.AppendLine(JsonConvert.SerializeObject(bodyData));
                stringBuilder.AppendLine("异常是");
                stringBuilder.AppendLine(e.Message);

                Debug.Print(stringBuilder.ToString());

                throw;
            }


#else
            IRestResponse response = await _client.ExecuteTaskAsync(request);
            return GetResult<T>(response);
#endif
        }

        public T Quest<T>(string url, RequestMethod method,
            Dictionary<string, string> queryData = null, object bodyData = null,
            Dictionary<string, string> headers = null)
        {
            _client.BaseUrl = new Uri(url);
            IRestRequest request = CreateRequest(method, queryData, bodyData, headers);

            return GetResult<T>(_client.Execute(request));
        }

        private IRestRequest CreateRequest(RequestMethod method,
            Dictionary<string, string> queryData, object bodyData, Dictionary<string, string> headers)
        {
            try
            {
                IRestRequest request = new RestRequest((RestSharp.Method) method)
                {
                    JsonSerializer = new NewtonsoftJsonSerializer()
                };
                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> kvp in headers)
                    {
                        request.AddHeader(kvp.Key, kvp.Value);
                    }
                }

                foreach (KeyValuePair<string, string> kvp in ConstHeaders)
                {
                    request.AddHeader(kvp.Key, kvp.Value);
                }

                if (bodyData != null)
                {
                    request.AddJsonBody(bodyData);
                }

                if (queryData != null)
                {
                    foreach (KeyValuePair<string, string> kvp in queryData)
                    {
                        request.AddQueryParameter(kvp.Key, kvp.Value);
                    }
                }

                return request;
            }
            catch
            {
                throw new Exception();
            }
        }

        public T UploadFile<T>(string url, RequestMethod method, string fileName,
            Dictionary<string, string> headers = null, Dictionary<string, string> queryData = null)
        {
            RestClient client = new RestClient(url);
            if (File.Exists(fileName))
            {
                if (headers == null)
                {
                    headers = new Dictionary<string, string>();
                }

                headers.Add("FileName", System.Web.HttpUtility.UrlEncode(Path.GetFileName(fileName)));
            }
            else
            {
                throw new FileNotFoundException($"文件:{fileName} 不存在！");
            }

            IRestRequest request = CreateFileRequest(method, fileName, headers, queryData);

            return GetResult<T>(client.Execute(request));
        }

        public async Task<T> UploadFileAsync<T>(string url, RequestMethod method, string fileName,
            Dictionary<string, string> headers = null, Dictionary<string, string> queryData = null)
        {
            RestClient client = new RestClient(url);
            if (File.Exists(fileName))
            {
                if (headers == null)
                {
                    headers = new Dictionary<string, string>();
                }

                headers.Add("FileName", System.Web.HttpUtility.UrlEncode(Path.GetFileName(fileName)));
            }
            else
            {
                throw new FileNotFoundException($"文件:{fileName} 不存在！");
            }

            IRestRequest request = CreateFileRequest(method, fileName, headers, queryData);

            return GetResult<T>(await client.ExecuteTaskAsync(request));
        }

        private IRestRequest CreateFileRequest(RequestMethod method, string fileToUpload,
            Dictionary<string, string> headers, Dictionary<string, string> queryData)
        {
            IRestRequest request = new RestRequest((RestSharp.Method) method)
            {
                Timeout = _client.Timeout
            };
            if (File.Exists(fileToUpload))
            {
                byte[] data;
                using (FileStream fs = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read,
                           FileShare.ReadWrite))
                {
                    data = fs.ReadAsBytes();
                }

                request.AddFile(Path.GetFileNameWithoutExtension(fileToUpload), data, Path.GetFileName(fileToUpload));
            }

            foreach (KeyValuePair<string, string> kvp in ConstHeaders)
            {
                request.AddHeader(kvp.Key, kvp.Value);
            }

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> kvp in headers)
                {
                    request.AddHeader(kvp.Key, kvp.Value);
                }
            }

            if (queryData != null)
            {
                foreach (KeyValuePair<string, string> kvp in queryData)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        request.AddQueryParameter(kvp.Key, kvp.Value);
                    }
                }
            }

            return request;
        }

        public void DownloadFile(string url, string fileName, Action<int> OnProgressChanged,
            Action<object, AsyncCompletedEventArgs> OnCompleted, Dictionary<string, string> headers = null)
        {
            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                foreach (KeyValuePair<string, string> kvp in ConstHeaders)
                {
                    client.Headers.Add(kvp.Key, kvp.Value);
                }

                if (headers != null)
                {
                    foreach (KeyValuePair<string, string> kvp in headers)
                    {
                        client.Headers.Add(kvp.Key, kvp.Value);
                    }
                }

                client.DownloadFileAsync(new Uri(url), fileName);
                client.DownloadProgressChanged += (s, e) => OnProgressChanged?.Invoke(e.ProgressPercentage);
                client.DownloadFileCompleted += (s, e) => OnCompleted?.Invoke(s, e);
            }

            ;
        }
        //public async Task DownloadFileAsync(string url, string savePath, Dictionary<string, string> headers)
        //{
        //    string filepath = Path.GetDirectoryName(savePath);
        //    if (!Directory.Exists(filepath))
        //    {
        //        Directory.CreateDirectory(savePath);
        //    }
        //    _client.BaseUrl = new Uri(url);
        //    var request = CreateFileRequest(Method.GET, null, headers);
        //    await Task.Factory.StartNew(() => _client.DownloadData(request).SaveAs(savePath));
        //}

        public async Task<bool> DownloadFileAsync(string url, string savePath, Dictionary<string, string> queryData,
            Dictionary<string, string> headers = null)
        {
            string filepath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            IRestRequest request = CreateFileRequest(RequestMethod.GET, null, headers, queryData);
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }

            _client.BaseUrl = new Uri(url);
            IRestResponse res = await _client.ExecuteTaskAsync(request);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                res.RawBytes.SaveAs(savePath);
                return true;
            }

            return false;
        }

        public bool DownloadFile(string url, string savePath, Dictionary<string, string> queryData,
            Dictionary<string, string> headers = null)
        {
            string filepath = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            IRestRequest request = CreateFileRequest(RequestMethod.GET, null, headers, queryData);
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }

            _client.BaseUrl = new Uri(url);
            IRestResponse res = _client.Execute(request);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                res.RawBytes.SaveAs(savePath);
                return true;
            }

            return false;
        }


        /// <summary>
        /// 发生了未授权的接口调用
        /// </summary>
        public static event EventHandler<string> UnauthorizedCall;
        private T GetResult<T>(IRestResponse response, StringBuilder stringBuilder = null)
        {
            T result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string resultJson = response.Content;

                stringBuilder?.AppendLine($"返回的JSON是\r\n{resultJson}");

                if (typeof(T) == typeof(string))
                {
                    result = (T) (object) resultJson;
                }
                else
                {
                    result = JsonConvert.DeserializeObject<T>(resultJson);
                }
            }
            else if (response.StatusCode == 0)
            {
                throw response.ErrorException;
            }
            else
            {

                if (response.StatusCode==HttpStatusCode.Unauthorized)
                {
                    UnauthorizedCall?.Invoke(this, "HttpRequestClient");
                }

                throw new Exception(response.StatusDescription + " : " + response.Content);
            }

            return result;
        }
    }
}