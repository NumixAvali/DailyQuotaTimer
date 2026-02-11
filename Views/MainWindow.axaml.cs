using System;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Toolkit.Uwp.Notifications;

namespace DailyQuotaTimer.Views;

public partial class MainWindow : Window
{
    public int Seconds { get; set; } = 0;

    private Timer aTimer;
    
    
    private void SetTimer()
    {
        // Create a timer with a two second interval.
        aTimer = new Timer(1000);
        // Hook up the Elapsed event for the timer. 
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }
    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        Seconds--;
        
        Dispatcher.UIThread.Post(() =>
        {
            PrettyTextLabelDisplay(Seconds);
            if (Seconds <= 0)
            {
                new ToastContentBuilder()
                    .AddText("Time's up!")
                    .SetToastScenario(ToastScenario.Alarm)
                    .AddButton(new ToastButton()
                        .SetContent("Useless button that does nothing")
                        .AddArgument("action", "open")
                        .SetBackgroundActivation())
                    .Show();
                
                aTimer.Stop();
                // aTimer.Dispose();
            }
        });
        
    }

    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void PrettyTextLabelDisplay(int seconds)
    {
        if (seconds < 0) seconds = 0;
        
        HourIndicator.Text = (seconds / 3600).ToString("00");
        MinuteIndicator.Text = (seconds % 3600 / 60).ToString("00");
        SecondIndicator.Text = (seconds % 60).ToString("00");
    }

    private void StartButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Seconds <= 0) return;
        
        if (aTimer == null)
        {
            SetTimer();
        }
        else
        {
            aTimer.Start();
        }
        
        // var builder = new ToastContentBuilder()
        //     .AddText("Hello from Avalonia!")
        //     .SetToastScenario(ToastScenario.Alarm)
        //     .AddButton(new ToastButton()
        //         .SetContent("Open App")
        //         .AddArgument("action", "open")
        //         .SetBackgroundActivation());
        //
        // builder.Show();
        
        // HourIndicator.Text = "99";
        // MinuteIndicator.Text = "99";
        // SecondIndicator.Text = "99";
    }

    private void Btn5M_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(5*60);
        Seconds = 5 * 60;
    }

    private void Btn10M_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(10*60);
        Seconds = 10 * 60;
    }

    private void Btn30M_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(30*60);
        Seconds = 30 * 60;
    }

    private void Btn1H_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(60*60);
        Seconds = 60 * 60;
    }

    private void Btn2H_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(2*60*60);
        Seconds = 2 * 60 * 60;
    }
    
    private void Btn5S_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(5);
        Seconds = 5;
    }

    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(0);
        Seconds = 0;
    }

    private void StopButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (aTimer != null)
            aTimer.Stop();
    }

}