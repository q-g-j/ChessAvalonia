using Avalonia.Controls;
using Avalonia.Interactivity;
using ChessAvalonia.ViewModels.Windows.Main;
using static ChessAvalonia.Services.MessengerService;

namespace ChessAvalonia.Views.Windows.Main;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        this.AddHandler(KeyDownEvent, MessageMainWindowViewModel.KeyDownHandler, RoutingStrategies.Tunnel);
    }
}
