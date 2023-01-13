using aemarcoCommons.WpfTools.BaseModels;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;

namespace aemarcoCommons.WpfTools.Services;

#nullable enable


/// <summary>
/// A class, monitoring the State of the Windows session.
/// - read the state through WindowsSessionState
/// - react through OnPropertyChanged
/// - react through WindowsSessionStateChanged message through IMessenger
/// </summary>
public class WindowsSessionStateMonitor : BaseNotifier, ISingleton, IDisposable
{
    public WindowsSessionStateMonitor()
    {
        SystemEvents.SessionSwitch += SessionSwitch;
    }

    private readonly IMessenger? _messenger;
    private readonly ILogger<WindowsSessionStateMonitor>? _logger;
    public WindowsSessionStateMonitor(
        IServiceProvider provider)
        : this()
    {
        _messenger = provider.GetService<IMessenger>();
        _logger = provider.GetService<ILogger<WindowsSessionStateMonitor>>();
    }


    internal void SessionSwitch(object? sender, SessionSwitchEventArgs e)
    {
        WindowsSessionState = e.Reason switch
        {
            SessionSwitchReason.SessionLogon => WindowsSessionState.LoggedIn,
            SessionSwitchReason.SessionLock => WindowsSessionState.Locked,
            SessionSwitchReason.SessionUnlock => WindowsSessionState.LoggedIn,
            SessionSwitchReason.SessionLogoff => WindowsSessionState.LoggedOut,
            _ => WindowsSessionState
        };
    }

    private WindowsSessionState _windowsSessionState = WindowsSessionState.Unknown;
    public WindowsSessionState WindowsSessionState
    {
        get => _windowsSessionState;
        private set
        {
            if (_windowsSessionState == value)
                return;

            var message = new WindowsSessionStateChanged(_windowsSessionState, value);
            _windowsSessionState = value;
            OnPropertyChanged();

            _messenger?.Send(message);
            _logger?.LogInformation("Windows session state changed, {@message}", message);
        }
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            SystemEvents.SessionSwitch -= SessionSwitch;

            _disposed = true;
        }
    }

    #endregion

}

public enum WindowsSessionState
{
    Unknown,
    LoggedIn,
    Locked,
    LoggedOut
}

// ReSharper disable NotAccessedPositionalProperty.Global
public record WindowsSessionStateChanged(WindowsSessionState OldState, WindowsSessionState NewState);
// ReSharper restore NotAccessedPositionalProperty.Global

