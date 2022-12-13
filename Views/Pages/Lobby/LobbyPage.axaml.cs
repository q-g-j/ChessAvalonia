using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Lobby;

namespace ChessAvalonia.Views.Pages.Lobby;

public partial class LobbyPage : UserControl
{
    public LobbyPage()
    {
        InitializeComponent();
        DataContext = new LobbyPageViewModel();
    }
}