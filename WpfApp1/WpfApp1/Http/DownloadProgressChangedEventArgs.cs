using System.ComponentModel;

namespace WpfApp1.Http;

public class DownloadProgressChangedEventArgs : ProgressChangedEventArgs
{
    /// <summary>获取收到的字节数。</summary>
    /// <returns>一个指示收到的字节数的 <see cref="T:System.Int64" /> 值。</returns>
    public long BytesReceived { get; }

    /// <summary>获取 <see cref="T:System.Net.WebClient" /> 数据下载操作中的字节总数。</summary>
    /// <returns>一个指示将要接收的字节数的 <see cref="T:System.Int64" /> 值。</returns>
    public long TotalBytesToReceive { get; }


    public DownloadProgressChangedEventArgs(int progressPercentage, long bytesReceived, long totalBytesToReceive) :
        base(progressPercentage, null)
    {
        BytesReceived       = bytesReceived;
        TotalBytesToReceive = totalBytesToReceive;
    }
}