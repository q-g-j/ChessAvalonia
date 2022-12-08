using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

namespace ChessAvalonia.Views.Pages.Lobby.Overlays;

public partial class WaitingForInvitationAcception : UserControl
{
    public WaitingForInvitationAcception()
    {
        InitializeComponent();
        DataContext = new WaitingForInvitationAcceptionViewModel();
    }
}