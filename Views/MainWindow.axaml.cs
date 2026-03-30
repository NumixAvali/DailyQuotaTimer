using System;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace DailyQuotaTimer.Views;

public partial class MainWindow : Window
{
    public int Seconds { get; set; } = 0;
    
    private Timer _aTimer;
    private bool _timerRunning = false;
    
    private ButtonMode _buttonMode = ButtonMode.Set;
    
    
    private void SetTimer()
    {
        // Create a timer with a two second interval.
        _aTimer = new Timer(1000);
        // Hook up the Elapsed event for the timer. 
        _aTimer.Elapsed += OnTimedEvent;
        _aTimer.AutoReset = true;
        _aTimer.Enabled = true;
        
        _timerRunning = true;
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
                
                _aTimer.Stop();
                // aTimer.Dispose();
                
                _timerRunning = false;
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

    private void TimeButtonUniversal_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string timeValue)
        {
            int seconds = int.Parse(timeValue);

            switch (_buttonMode)
            {
                case ButtonMode.Add:
                    PrettyTextLabelDisplay(Seconds+seconds);
                    Seconds += seconds;
                    break;
                case ButtonMode.Set:
                    PrettyTextLabelDisplay(seconds);
                    Seconds = seconds;
                    break;
                case ButtonMode.Remove:
                    PrettyTextLabelDisplay(Seconds-seconds);
                    Seconds -= seconds;
                    if (seconds <= 0) Seconds = 0;
                    break;
            }
        }
    }

    private void ToggleTimeModeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        switch (_buttonMode)
        {
            case ButtonMode.Add:
                _buttonMode = ButtonMode.Remove;
                ToggleTimeModeButton.Content = "-";
                ToolTip.SetTip(ToggleTimeModeButton,"Subtractive time mode.");

                break;
            case ButtonMode.Set:
                _buttonMode = ButtonMode.Add;
                ToggleTimeModeButton.Content = "+";
                ToolTip.SetTip(ToggleTimeModeButton,"Additive time mode.");

                break;
            case ButtonMode.Remove:
                _buttonMode = ButtonMode.Set;
                ToggleTimeModeButton.Content = "=";
                ToolTip.SetTip(ToggleTimeModeButton,"Set time mode.");

                break;
            default:
                _buttonMode = ButtonMode.Set;
                ToggleTimeModeButton.Content = "=";
                ToolTip.SetTip(ToggleTimeModeButton,"Set time mode.");

                break;
        }
    }

    private void ToggleTopmostModeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Topmost)
        {
            TopmostToggleButton.ClearValue(Button.BackgroundProperty);
            Topmost = false;
        }
        else
        {
            Topmost = true;
            if (Application.Current!.TryFindResource("SystemControlHighlightAccentBrush", out var accentBrush))
            {
                TopmostToggleButton.Background = (IBrush)accentBrush!;
            }
        }
    }

    private void StartButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Seconds <= 0) return;
        
        if (_aTimer == null)
        {
            SetTimer();
        }
        else
        {
            _aTimer.Start();
        }
        
        _timerRunning = true;
    }
    
    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        PrettyTextLabelDisplay(0);
        Seconds = 0;
    }

    private void StopButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_aTimer != null)
        {
            _aTimer.Stop();
            _timerRunning = false;
        }
    }



    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (!_timerRunning)
        {
            base.OnClosing(e);
            return;
        }
        
        e.Cancel = true;
        
        var box = MessageBoxManager
            .GetMessageBoxStandard("Quit?", "Are you sure you would like to quit the application?" +
                                              "\nThe timer is still running!",
                ButtonEnum.YesNo);

        var result = await box.ShowAsync();

        if (result == ButtonResult.Yes)
        {
            // This is required.
            // Dialog reopens in a loop otherwise.
            _aTimer.Stop();
            _aTimer.Dispose();
            _timerRunning = false;
            
            e.Cancel = false;
            base.OnClosing(e);
            this.Close();
        }
    }
}

enum ButtonMode
{
    Set,
    Add,
    Remove
}