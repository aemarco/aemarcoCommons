using aemarcoCommons.Extensions.TaskExtensions;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.Pipes
{
    public class MessageBasedPipeServer
    {

        private string _pipeName;
        private Action<string> _eventAction;
        public MessageBasedPipeServer Start(string pipeName, Action<string> eventAction)
        {
            _pipeName = pipeName;
            _eventAction = eventAction;

            new Thread(CreateNamedPipeServer) { IsBackground = true }.Start();
            return this;
        }

        private void CreateNamedPipeServer()
        {
            using (var pipeServer =
                new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message))
            {
                while (true)
                {
                    pipeServer.WaitForConnection();

                    var reader = new StreamReader(pipeServer);
                    var message = reader.ReadLine();
                    pipeServer.Disconnect();

                    HandleAsMessage(message);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public void HandleAsMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            Task.Run(() => _eventAction(message)).SafeFireAndForget();
        }

    }

    public static class MessageBasedPipeClient
    {

        public static void SendMessageToPipe(this string message, string targetMachine, string pipeName)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            using (var namedPipeClientStream = new NamedPipeClientStream(targetMachine, pipeName, PipeDirection.Out))
            {
                try
                {
                    namedPipeClientStream.Connect(TimeSpan.FromSeconds(3).Milliseconds);
                    using (var writer = new StreamWriter(namedPipeClientStream)
                    {
                        AutoFlush = true
                    })
                    {
                        writer.WriteLine(message);
                    }
                }
                catch
                {
                    // ignored
                }

            }

        }

    }


}
