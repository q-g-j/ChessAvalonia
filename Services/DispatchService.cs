using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.Services;

internal static class DispatchService
{
    public static void Invoke(Action action)
    {
        Dispatcher dispatchObject = Dispatcher.UIThread;
        if (dispatchObject == null || dispatchObject.CheckAccess())
        {
            action();
        }
        else
        {
            dispatchObject.InvokeAsync(action, DispatcherPriority.Normal).Wait();                
        }
    }
}
