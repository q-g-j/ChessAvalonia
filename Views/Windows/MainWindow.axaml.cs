using Avalonia.Controls;
using Avalonia.Interactivity;
using ChessAvalonia.ViewModels.Windows;
using static ChessAvalonia.Services.MessengerService;

namespace ChessAvalonia.Views.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        this.AddHandler(KeyDownEvent, MessageMainWindowViewModel.KeyDownHandler, RoutingStrategies.Tunnel);
    }
}
