using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

namespace ChessAvalonia.Views.Pages.Lobby.Overlays;

public partial class OpponentLeftLobby : UserControl
{
    public OpponentLeftLobby()
    {
        InitializeComponent();
        DataContext = new OpponentLeftLobbyViewModel();
    }
}