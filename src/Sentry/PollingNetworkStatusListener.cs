namespace Sentry;

internal class PollingNetworkStatusListener : INetworkStatusListener
{
    internal int _delayInMilliseconds;
    private readonly int _maxDelayInMilliseconds;
    private readonly Func<int, int> _backoffFunction;

    /// <summary>
    /// Overload for testing
    /// </summary>
    internal PollingNetworkStatusListener(IPing ping, int initialDelayInMilliseconds = 500,
        int maxDelayInMilliseconds = 32_000, Func<int, int>? backoffFunction = null)
    {
        Ping = ping;
        _delayInMilliseconds = initialDelayInMilliseconds;
        _maxDelayInMilliseconds = maxDelayInMilliseconds;
        _backoffFunction = backoffFunction ?? (x => x * 2);
    }

    private IPing Ping { get; }

    private volatile bool _online = true;
    public bool Online
    {
        get => _online;
        set => _online = value;
    }

    public async Task WaitForNetworkOnlineAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_delayInMilliseconds, cancellationToken).ConfigureAwait(false);
                var checkResult = await Ping.IsAvailableAsync(cancellationToken).ConfigureAwait(false);
                if (checkResult)
                {
                    Online = true;
                    return;
                }
                if (_delayInMilliseconds < _maxDelayInMilliseconds)
                {
                    _delayInMilliseconds = _backoffFunction(_delayInMilliseconds);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
