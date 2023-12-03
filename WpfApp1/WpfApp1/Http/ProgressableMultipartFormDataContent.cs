using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WpfApp1.Http;
//参考 https://stackoverflow.com/questions/41378457/c-httpclient-file-upload-progress-when-uploading-multiple-file-as-multipartfo

public class ProgressableStreamContent : HttpContent
{
    /// <summary>
    /// Lets keep buffer of 20kb
    /// </summary>
    private const int DefaultBufferSize = 5 * 4096;

    private readonly HttpContent _content;

    private readonly int _bufferSize;

    //private bool contentConsumed;
    public Action<long, long>? Progress { get; set; }


    public ProgressableStreamContent(HttpContent content, int bufferSize = DefaultBufferSize)
    {
        if (bufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        this._content    = content ?? throw new ArgumentNullException(nameof(content));
        this._bufferSize = bufferSize;

        foreach (var h in content.Headers)
        {
            this.Headers.Add(h.Key, h.Value);
        }
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        return Task.Run(async () =>
        {
            var buffer = new byte[_bufferSize];
            TryComputeLength(out var size);
            var uploaded = 0;

            await using var streamAsync = await _content.ReadAsStreamAsync();

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
                catch (ObjectDisposedException)
                {
                    return;
                }
            }

            await stream.FlushAsync();
        });
    }

    protected override bool TryComputeLength(out long length)
    {
        length = _content.Headers.ContentLength.GetValueOrDefault();
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _content.Dispose();
        }

        base.Dispose(disposing);
    }
}