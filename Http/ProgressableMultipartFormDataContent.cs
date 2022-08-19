using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShareDrawing.HttpClient.Http
{
    //参考 https://stackoverflow.com/questions/41378457/c-httpclient-file-upload-progress-when-uploading-multiple-file-as-multipartfo

    public class ProgressableStreamContent : HttpContent
    {
        /// <summary>
        /// Lets keep buffer of 20kb
        /// </summary>
        private const int defaultBufferSize = 5 * 4096;

        private HttpContent content;

        private int bufferSize;

        //private bool contentConsumed;
        public Action<long, long> Progress { get; set; }

        public ProgressableStreamContent(HttpContent content) : this(content, defaultBufferSize)
        {
        }


        public ProgressableStreamContent(HttpContent content, int bufferSize)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            this.content = content ?? throw new ArgumentNullException(nameof(content));
            this.bufferSize = bufferSize;

            foreach (var h in content.Headers)
            {
                this.Headers.Add(h.Key, h.Value);
            }
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.Run(async () =>
            {
                var buffer = new byte[bufferSize];
                TryComputeLength(out var size);
                var uploaded = 0;

                using var streamAsync = await content.ReadAsStreamAsync();

                while (true)
                {
                    //实测此处得到的 length 和 size 会不准, 特别是在发送小文件时.
                    var length = await streamAsync.ReadAsync(buffer, 0, buffer.Length);
                    if (length <= 0) break;

                    uploaded += length;
                    Progress?.Invoke(uploaded, size);

                    //System.Diagnostics.Debug.WriteLine($"Bytes sent {uploaded} of {size}");
                    try
                    {
                        // 取消上传时，这里会报ObjectDisposedException异常（可能是stream在线程外被Dispose）
                        await stream.WriteAsync(buffer, 0, length);
                        await stream.FlushAsync();
                    }
                    catch(ObjectDisposedException)
                    {
                        return;
                    }
                }

                await stream.FlushAsync();
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Headers.ContentLength.GetValueOrDefault();
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}