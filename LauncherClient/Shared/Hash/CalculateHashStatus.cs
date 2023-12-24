using System;

namespace Shared.Hash;

/// <summary>
/// Class that contains status of hash calculation operation.
/// </summary>
public class CalculateHashStatus : IDisposable
{
    #region properties

    public string Message { get; private set; } = string.Empty;
    public double Progress { get; private set; }

    #endregion

    #region attributes

    private Action<double, string>? _statusChanged;

    #endregion

    #region constructors

    public CalculateHashStatus(Action<double, string>? onStatusChanged) => _statusChanged = onStatusChanged;

    #endregion

    #region public methods

    public void Set(double progress, string message)
    {
        Message = message;
        Progress = progress;

        _statusChanged?.Invoke(progress, message);
    }

    #endregion

    #region IDisposable

    public void Dispose() => _statusChanged = null;

    #endregion
}