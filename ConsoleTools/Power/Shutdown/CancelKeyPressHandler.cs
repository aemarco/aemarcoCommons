//using System;
//using System.Threading;

//namespace aemarcoCommons.ConsoleTools.Power.Shutdown;
//public class CancelKeyPressHandler
//{


//    private readonly CancellationTokenSource _cancellationTokenSource;

//    public CancelKeyPressHandler()
//    {
//        _cancellationTokenSource = new CancellationTokenSource();

//    }

//    private CancellationToken GetCancellationToken()
//    {
//        Console.CancelKeyPress += Console_CancelKeyPress;
//        return _cancellationTokenSource.Token;
//    }

//    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
//    {
//        Console.WriteLine();
//        Console.WriteLine("Canceling...");
//        _cancellationTokenSource.Cancel();
//        e.Cancel = true;
//    }


//    public static explicit operator CancellationToken(CancelKeyPressHandler handler)
//    {
//        return handler.GetCancellationToken();
//    }

//}
