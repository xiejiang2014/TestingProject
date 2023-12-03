using System;

namespace WpfApp1.Http;

public class Canceller
{
    //////#region 调试用

    //////private static readonly Random Random = new Random(2452345);

    //////public int Id { get; } = Random.Next();

    //////#endregion

    public Guid Guid { get; }

    public Canceller(Guid guid) : this()
    {
        Guid = guid;
    }


    public Canceller()
    {
    }


    public bool IsCanceled { get; private set; }

    public event EventHandler? OnCancel;

    public string? Reason { get; private set; }

    public void Cancel(string reason = "")
    {
        Reason     = reason;
        IsCanceled = true;
        OnCancel?.Invoke(this, EventArgs.Empty);
    }

    public void Reset()
    {
        IsCanceled = false;
        Reason     = "";
    }
}