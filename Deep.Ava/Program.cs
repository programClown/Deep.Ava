using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Deep.Ava.Controls;
using Deep.Ava.ViewModels;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Ursa.Controls;

namespace Deep.Ava;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Configure exception dialog for unhandled exceptions
        if (!Debugger.IsAttached)
        {        
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }            

        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }
    
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseAliBabaFontFamily()
            .LogToTrace();
    }
    
    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception ex)
        {
            return;
        }

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            var dialogViewModel = new ExceptionDialogViewModel { Exception = ex };

            // Window? mainWindow = lifetime.MainWindow;
            // // We can only show dialog if main window exists, and is visible
            // if (mainWindow is { PlatformImpl: not null, IsVisible: true })
            // {
            // }

            var cts = new CancellationTokenSource();
            Dialog.ShowModal<ExceptionDialog, ExceptionDialogViewModel>(
                dialogViewModel,
                options: new DialogOptions
                {
                    Title = "异常问题", Mode = DialogMode.Error, Button = DialogButton.None, ShowInTaskBar = false
                }
            ).WaitAsync(cts.Token).ContinueWith(
                _ =>
                {
                    cts.Cancel();
                    ExitWithException(ex);
                },
                TaskScheduler.FromCurrentSynchronizationContext()
            );

            Dispatcher.UIThread.MainLoop(cts.Token);
        }
    }
    [DoesNotReturn]
    private static void ExitWithException(Exception exception)
    {
        // App.Shutdown(1);
        Dispatcher.UIThread.InvokeShutdown();
        Environment.Exit(Marshal.GetHRForException(exception));
    }

}