using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

namespace ChessAvalonia.Views.Pages.Lobby.Overlays;

public partial class OpponentCanceledInvitation : UserControl
{
    public OpponentCanceledInvitation()
    {
        InitializeComponent();
        DataContext = new OpponentCanceledInvitationViewModel();
    }
}