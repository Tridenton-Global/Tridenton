namespace Tridenton.Core.Util;

/// <param name="IncrementTransferred">Gets the number of bytes transferred since last event </param>
/// <param name="TransferredBytes">Gets the number of bytes transferred </param>
/// <param name="TotalBytes">Gets the total number of bytes to be transferred </param>
public record StreamTransferProgressArgs(long IncrementTransferred, long TransferredBytes, long TotalBytes) : RecordEventArgs
{
    /// <summary>
    ///     Gets the percentage of transfer completed
    /// </summary>
    public int PercentDone => (int)(TransferredBytes * 100 / TotalBytes);

    /// <summary>
    ///     Returns a string representation of this object
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Transfer statistics. Percentage completed: {PercentDone}, bytes transferred: {TransferredBytes}, total bytes to transfer: {TotalBytes}";
}