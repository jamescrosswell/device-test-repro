namespace Sentry.Tests;

public class PollingNetworkStatusListenerTest
{
    [Fact]
    public async Task HostAvailable_CheckOnlyRunsOnce()
    {
        // Arrange
        var initialDelay = 100;
        var pingHost = Substitute.For<IPing>();
        pingHost
            .IsAvailableAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var pollingListener = new PollingNetworkStatusListener(pingHost, initialDelay);
        pollingListener.Online = false;

        // Act
        var waitForNetwork = pollingListener.WaitForNetworkOnlineAsync();
        var timeout = Task.Delay(1000);
        var completedTask = await Task.WhenAny(waitForNetwork, timeout);

        // Assert
        completedTask.Should().Be(waitForNetwork);
        pollingListener.Online.Should().Be(true);
        await pingHost.Received(1).IsAvailableAsync(Arg.Any<CancellationToken>());
    }
}
