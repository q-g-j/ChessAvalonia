using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

namespace ChessAvalonia.Views.Pages.Lobby.Overlays;

public partial class OpponentAcceptedInvitation : UserControl
{
    public OpponentAcceptedInvitation()
    {
        InitializeComponent();
        DataContext = new OpponentAcceptedInvitationViewModel();
    }
}