using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WpfApp1.Http
{
    public static partial class HttpHelper
    {



        #region 文件下载

        public static async Task DownloadFileAsync(
            string                                    url,
            string                                    fullPathAndFileName,
            Action<DownloadProgressChangedEventArgs>? progressChanged        = null,
            Canceller?                                canceller              = null,
            string                                    oldETag                = "",
            List<(string name, string value)>?        headers                = null,
            Action<string>?                           eTagChanged            = null,
            Action<byte[]>?                           bytesWritedToLocalFile = null
        )
        {
            FileStream? localStream = null;

            var debugString = new StringBuilder();

            try
            {
                //检查是否可以续传
                var (resumeDownload, contentLength) = CanResume(
                    url,
                    fullPathAndFileName,
                    oldETag,
                    headers
                );


                debugString.Append(resumeDownload ? "可以断点续传 " : "不能断点续传 ");


                //如果下载新文件,是 Create ,如果是续传,则为 append
                var fileMode = resumeDownload ? FileMode.Append : FileMode.Create;


                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.KeepAlive = false;
                if (headers is not null)
                {
                    foreach (var (name, value) in headers)
                    {
                        httpWebRequest.Headers.Add(name, value);
                    }
                }


                var fileInfo = new FileInfo(fullPathAndFileName);

                if (fileInfo.Directory is null)
                {
                    throw new DirectoryNotFoundException($"指定的路径无效,{fullPathAndFileName}");
                }

                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                //默认从文件最开始处进行下载
                long downloadedLength = 0;

                //如果是续传,那么接着之前已下载的位置继续下载
                if (resumeDownload)
                {
                    debugString.Append($"从 {fileInfo.Length} 开始下载. ");

                    //想要断点续传,但发现已经下载完成
                    if (contentLength != 0 && contentLength == fileInfo.Length)
                    {
                        var downloadProgressChangedEventArgs =
                            new DownloadProgressChangedEventArgs(100, contentLength, contentLength);

                        progressChanged?.Invoke(downloadProgressChangedEventArgs);

                        return;
                    }

                    httpWebRequest.AddRange(fileInfo.Length); //指定接着哪个字节继续下载
                    downloadedLength = fileInfo.Length;
                }

                using var response = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using var responseStream = response.GetResponseStream();

                if (responseStream is null)
                {
                    throw new ApplicationException("文件下载失败,服务器没有返回文件流.");
                }

                //下载文件时,内容类型如果是"application/json",那么肯定是发生了错误
                if (response.ContentType == "application/json")
                {
                    using var reader = new StreamReader(responseStream, Encoding.UTF8);
                    var json = await reader.ReadToEndAsync();

                    ParesJson(json);
                }


                var newETag = response.Headers["ETag"];

                //向外传递新的etag
                if (!string.IsNullOrWhiteSpace(newETag) && oldETag != newETag)
                {
                    eTagChanged?.Invoke(newETag);
                }

                //文件的总体积.注意,如果是断点续传,那么 GetContentLength 得到的是剩余内容的长度,而不是完成内容的长度
                contentLength = downloadedLength + GetContentLength(response);

                const int bufferSize = 32768;
                var buffer = new byte[bufferSize];

                Debug.Print($"httpHelper 打开文件向其填入数据 {fullPathAndFileName}{Environment.NewLine}URL:{url}");
                Log?.Invoke($"httpHelper 打开文件向其填入数据 {fullPathAndFileName} URL:{url}", false);

                localStream = new FileStream(fullPathAndFileName, fileMode);

                using var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                //持续读取在线数据流,并写入本地
                while (true)
                {
                    //如果持续一段时间都不返回任何数据,那么就直接取消下载任务
                    cancellationTokenSource.CancelAfter(10000);

                    int blockDataLength;
                    try
                    {
                        //这里要特别注意一种情况,
                        //服务器既不返回数据也不断开链接,导致 ReadAsync 卡死,永远不返回,
                        //而且无论是在 ReadAsync 中传入 cancellationToken 或是设置 Request 的超时时间都不起作用
                        //问题 https://github.com/dotnet/runtime/issues/28404
                        //解决方案 https://devblogs.microsoft.com/pfxteam/how-do-i-cancel-non-cancelable-async-operations/
                        blockDataLength = await responseStream
                                               .ReadAsync(buffer, 0, bufferSize, cancellationToken)
                                               .WithCancellation(cancellationToken);
                    }
                    catch (Exception e)
                    {
                        throw new IOException("数据传输已中断.", 999);
                    }

                    cancellationTokenSource.CancelAfter(TimeSpan.FromHours(1));

                    if (blockDataLength <= 0)
                    {
                        break;
                    }

                    //检查是否已取消
                    if (canceller is not null && canceller.IsCanceled)
                    {
                        // 抛出取消原因
                        var msg = string.IsNullOrWhiteSpace(canceller.Reason) ? "请求被中止: 请求已被取消。" : canceller.Reason;
                        throw new WebException(msg, WebExceptionStatus.RequestCanceled);
                    }

                    //持续写入到本地文件流
                    await localStream.WriteAsync(buffer, 0, blockDataLength, cancellationToken);

                    bytesWritedToLocalFile?.Invoke(buffer);


                    //计算进度
                    downloadedLength += blockDataLength;

                    var doubleDownloadPercent = 0.0;
                    if (contentLength > 0.0)
                    {
                        doubleDownloadPercent = downloadedLength / (double)contentLength;
                        if (doubleDownloadPercent > 1.0)
                        {
                            doubleDownloadPercent = 1.0;
                        }
                    }

                    var intDownloadPercent = (int)(doubleDownloadPercent * 100);

                    var downloadProgressChangedEventArgs =
                        new DownloadProgressChangedEventArgs(intDownloadPercent, downloadedLength, contentLength);

                    progressChanged?.Invoke(downloadProgressChangedEventArgs);
                }

                response.Dispose();
                responseStream.Dispose();

                await localStream.FlushAsync(cancellationToken);
                localStream.Close();

                Debug.Print($"httpHelper 文件下载完成,已关闭文件 {fullPathAndFileName} URL:{url}");
                Log?.Invoke($"httpHelper 文件下载完成,已关闭文件 {fullPathAndFileName} URL:{url}", false);


                Debug.Print(
                    $"httpHelper 文件下载成功{Environment.NewLine}URL:{url}{Environment.NewLine}ETag:{oldETag}{Environment.NewLine}{debugString}");
            }
            catch (WebException e) when (e.Response is not null)
            {
                if (localStream is not null)
                {
                    Log?.Invoke($"httpHelper 文件下载异常,准备关闭文件 {fullPathAndFileName} URL:{url}", true);
                    localStream?.Flush();
                    localStream?.Close();

                    Debug.Print($"httpHelper 文件下载异常,已关闭文件 {fullPathAndFileName}");
                    Log?.Invoke($"httpHelper 文件下载异常,已关闭文件 {fullPathAndFileName} URL:{url}", true);
                }
                else
                {
                    Log?.Invoke($"httpHelper 文件下载异常 URL:{url}", true);
                }


                using var stream = e.Response.GetResponseStream();
                if (stream != null)
                {
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    var responseJson = await reader.ReadToEndAsync();

                    HttpResponseBase? httpResponseBase = null;
                    try
                    {
                        httpResponseBase = JsonConvert.DeserializeObject<HttpResponseBase>(responseJson);
                    }
                    catch (Exception)
                    {
                    }

                    if (httpResponseBase is not null)
                    {
                        Debug.Print(
                            $"httpHelper 文件下载失败  {fullPathAndFileName} {Environment.NewLine}URL:{url}{Environment.NewLine}ETag:{oldETag}{Environment.NewLine}{debugString}{Environment.NewLine}{e}");
                        Log?.Invoke(
                            $"httpHelper 文件下载失败  {fullPathAndFileName} URL:{url} ETag:{oldETag} {debugString} {e}",
                            true);

                        throw new HttpException(httpResponseBase.Message, httpResponseBase.Code);
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    Debug.Print(
                        $"httpHelper 文件下载失败{Environment.NewLine}URL:{url}{Environment.NewLine}ETag:{oldETag}{Environment.NewLine}{debugString}{Environment.NewLine}{e}");
                    Log?.Invoke($"httpHelper 文件下载失败  {fullPathAndFileName}  URL:{url} ETag:{oldETag} {debugString} {e}",
                                true);
                }


                throw;
            }
            catch (Exception e)
            {
                Log?.Invoke($"httpHelper 文件下载异常,准备关闭文件 {fullPathAndFileName} URL:{url}", true);
                localStream?.Flush();
                localStream?.Close();
                Debug.Print($"httpHelper 文件下载异常,已关闭文件 {fullPathAndFileName}");
                Log?.Invoke($"httpHelper 文件下载异常 {fullPathAndFileName} ,已关闭文件 {fullPathAndFileName} URL:{url}", true);


                Debug.Print(
                    $"httpHelper 文件下载失败{Environment.NewLine}URL:{url}{Environment.NewLine}ETag:{oldETag}{Environment.NewLine}{debugString}{Environment.NewLine}{e}");
                Log?.Invoke($"httpHelper 文件下载失败 {fullPathAndFileName} URL:{url} ETag:{oldETag} {debugString} {e}",
                            true);

                throw;
            }

            //finally
            //{
            //    localStream?.Flush();
            //    localStream?.Close();
            //    responseStream?.Close();
            //}
        }


        /// <summary>
        /// 判断是否可续传
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uncompletedFile">之前未完成下载的文件</param>
        /// <param name="oldETag"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        private static (bool canResume, long contentLength) CanResume(
            string url,
            string? uncompletedFile,
            string? oldETag,
            List<(string name, string value)>? headers = null)
        {
            if (string.IsNullOrWhiteSpace(uncompletedFile) || string.IsNullOrWhiteSpace(oldETag))
            {
                return (false, 0);
            }


            //如果不存在之前未完成下载的文件,那么直接认为无法续传
            if (!File.Exists(uncompletedFile))
            {
                return (false, 0);
            }

            HttpWebResponse? response = null;


            try
            {

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                
                if (headers is not null)
                {
                    foreach (var (name, value) in headers)
                    {
                        request.Headers.Add(name, value);
                    }
                }

                response = (HttpWebResponse)request.GetResponse();

                var contentLength = GetContentLength(response);

                if (CheckIfServerAcceptRanges(response))
                {
                    //=============检查新旧 Etag 标签是否一致===============
                    var newEtag = response.Headers["ETag"];

                    if (!string.IsNullOrWhiteSpace(newEtag) &&
                        !string.IsNullOrWhiteSpace(oldETag) &&
                        newEtag == oldETag
                       )
                    {
                        //新旧 Etag 标签是否一致,判定可以续传
                        return (true, contentLength);
                    }
                }
            }
            finally
            {
                response?.Close();
            }

            return (false, 0);
        }


        /// <summary>
        /// 检查服务器是否支持续传
        /// </summary>
        /// <param name="webResponse"></param>
        /// <returns></returns>
        private static bool CheckIfServerAcceptRanges(WebResponse webResponse)
        {
            if (webResponse.Headers["Accept-Ranges"] != null)
            {
                var acceptRanges = webResponse.Headers["Accept-Ranges"];
                if (acceptRanges == "none")
                {
                    return false;
                }
            }

            return true;
        }


        private static long GetContentLength(WebResponse webResponse)
        {
            long result = 0;
            if (webResponse.Headers["Content-Length"] != null)
            {
                var s = webResponse.Headers["Content-Length"];
                if (!long.TryParse(s, out result))
                {
                    result = 0;
                }
            }

            return result;
        }

        private static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using var cancellationTokenRegistration = cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true),
                tcs);

            if (task != await Task.WhenAny(task, tcs.Task))
            {
                throw new OperationCanceledException(cancellationToken);
            }

            return await task;
        }

        #endregion
    }
}
