using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

namespace ChessAvalonia.Views.Pages.Lobby.Overlays;

public partial class ErrorMessage : UserControl
{
    public ErrorMessage()
    {
        InitializeComponent();
        DataContext = new LobbyPageErrorMessageViewModel();
    }
}