using System.Threading.Tasks;
using System.Windows.Input;
#nullable enable
namespace aemarcoCommons.WpfTools.Commands;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object? parameter);
}