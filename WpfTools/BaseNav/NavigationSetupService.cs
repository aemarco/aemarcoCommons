
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.WpfTools.BaseNav;

#pragma warning disable CsWinRT1028
public class NavigationSetupService : BackgroundService
#pragma warning restore CsWinRT1028
{

    private readonly IServiceProvider _serviceProvider;
    public NavigationSetupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        BaseNavWindowViewModel.ServiceProvider = _serviceProvider;
        return Task.CompletedTask;
    }
}