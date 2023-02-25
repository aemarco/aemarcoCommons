using aemarcoCommons.WpfTools.Services;
using FluentAssertions;
using Microsoft.Win32;

namespace WpfToolsTests.Services;

[TestFixture]
public class WindowsSessionStateMonitorTests
{
    [Test]
    public void WindowsSessionState_ShouldBeUnknown_WhenInitialized()
    {
        // Arrange
        using var sut = new WindowsSessionStateMonitor();
        // Act
        var result = sut.WindowsSessionState;
        // Assert
        result.Should().Be(WindowsSessionState.Unknown);
    }

    [TestCase(SessionSwitchReason.SessionLogon, WindowsSessionState.LoggedIn)]
    [TestCase(SessionSwitchReason.SessionLock, WindowsSessionState.Locked)]
    [TestCase(SessionSwitchReason.SessionUnlock, WindowsSessionState.LoggedIn)]
    [TestCase(SessionSwitchReason.SessionLogoff, WindowsSessionState.LoggedOut)]
    public void SessionSwitch_ShouldChangeWindowsSessionState_WhenCalled(SessionSwitchReason reason, WindowsSessionState expected)
    {
        // Arrange
        using var sut = new WindowsSessionStateMonitor();
        // Act
        sut.SessionSwitch(this, new SessionSwitchEventArgs(reason));
        var result = sut.WindowsSessionState;

        // Assert
        result.Should().Be(expected);
    }

}

