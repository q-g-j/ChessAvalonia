using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ChessAvalonia.ViewModels.Pages.Lobby;
using ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

namespace ChessAvalonia.Views.Pages.Lobby;

public partial class LobbyPage : UserControl
{
    public LobbyPage()
    {
        InitializeComponent();
        DataContext = new LobbyPageViewModel();
    }
}