using ChessAvalonia.ViewModels;
using ChessAvalonia.ViewModels.Pages.Lobby;
using ChessAvalonia.ViewModels.Pages.Lobby.Overlays;
using ChessAvalonia.ViewModels.Pages.Main;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;
using ChessAvalonia.ViewModels.Windows.Main;
using CommunityToolkit.Mvvm.Messaging;

namespace ChessAvalonia.Services;

internal static class MessengerService
{
    internal static LobbyPageViewModel MessageLobbyViewModel
    { 
        get => WeakReferenceMessenger.Default.Send<LobbyPageViewModel.LobbyPageViewModelRequestMessage>();
    }
    internal static MainPageViewModel MessageMainPageViewModel
    {
        get => WeakReferenceMessenger.Default.Send<MainPageViewModel.MainPageViewModelRequestMessage>();
    }
    internal static MainWindowViewModel MessageMainWindowViewModel
    {
        get => WeakReferenceMessenger.Default.Send<MainWindowViewModel.MainWindowViewModelRequestMessage>();
    }
    internal static MenuViewModel MessageMenuViewModel
    {
        get => WeakReferenceMessenger.Default.Send<MenuViewModel.MenuViewModelRequestMessage>();
    }
    internal static PromotePawnViewModel MessagePromotePawnViewModel
    {
        get => WeakReferenceMessenger.Default.Send<PromotePawnViewModel.PromotePawnViewModelRequestMessage>();
    }
    internal static OnlineGamePlayerQuitViewModel MessageOnlineGamePlayerQuitViewModel
    {
        get => WeakReferenceMessenger.Default.Send<OnlineGamePlayerQuitViewModel.OnlineGamePlayerQuitViewModelRequestMessage>();
    }
    internal static LobbyPageErrorMessageViewModel MessageLobbyPageErrorMessageViewModel
    {
        get => WeakReferenceMessenger.Default.Send<LobbyPageErrorMessageViewModel.LobbyPageErrorMessageViewModelRequestMessage>();
    }
    internal static MainPageErrorMessageViewModel MessageMainPageErrorMessageViewModel
    {
        get => WeakReferenceMessenger.Default.Send<MainPageErrorMessageViewModel.MainPageErrorMessageViewModelRequestMessage>();
    }
    internal static OpponentLeftLobbyViewModel MessageOpponentLeftLobbyViewModel
    {
        get => WeakReferenceMessenger.Default.Send<OpponentLeftLobbyViewModel.OpponentLeftLobbyViewModelRequestMessage>();
    }
    internal static OpponentAcceptedInvitationViewModel MessageOpponentAcceptedInvitationViewModel
    {
        get => WeakReferenceMessenger.Default.Send<OpponentAcceptedInvitationViewModel.OpponentAcceptedInvitationViewModelRequestMessage>();
    }
    internal static OpponentCanceledInvitationViewModel MessageOpponentCanceledInvitationViewModel
    {
        get => WeakReferenceMessenger.Default.Send<OpponentCanceledInvitationViewModel.OpponentCanceledInvitationViewModelRequestMessage>();
    }
    internal static PlayerNameViewModel MessagePlayerNameViewModel
    {
        get => WeakReferenceMessenger.Default.Send<PlayerNameViewModel.PlayerNameViewModelViewModelRequestMessage>();
    }
    internal static WaitingForInvitationAcceptionViewModel MessageWaitingForInvitationAcceptionViewModel
    {
        get => WeakReferenceMessenger.Default.Send<WaitingForInvitationAcceptionViewModel.WaitingForInvitationAcceptionViewModelRequestMessage>();
    }
}