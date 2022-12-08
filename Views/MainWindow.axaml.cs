using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Core;
using Avalonia.Xaml.Interactivity;
using ChessAvalonia.Models;
using ChessAvalonia.ViewModels;
using ChessAvalonia.Converters;
using static ChessAvalonia.Services.MessengerService;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ChessAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        this.AddHandler(KeyDownEvent, MessageMainWindowViewModel.KeyDownHandler, RoutingStrategies.Tunnel);
    }
}
