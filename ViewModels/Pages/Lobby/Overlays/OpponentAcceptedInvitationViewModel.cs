using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;
using ChessAvalonia.ViewModels.Pages.Main;
using ChessAvalonia.Services;
using static ChessAvalonia.Services.MessengerService;

namespace ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

[INotifyPropertyChanged]
public partial class OpponentAcceptedInvitationViewModel
{
    #region Constructor
    public OpponentAcceptedInvitationViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private string opponentName = "Opponent";

    [ObservableProperty]
    private bool overlayOpponentAcceptedInvitationIsVisible = false;
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<OpponentAcceptedInvitationViewModel, OpponentAcceptedInvitationViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Commands
    [RelayCommand]
    private void ButtonStartGame()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        MenuViewModel menuViewModel = MessageMenuViewModel;

        menuViewModel.MenuButtonOpenLobbyIsVisible = false;
        menuViewModel.MenuButtonEndOnlineGameIsVisible = true;
        MessageWaitingForInvitationAcceptionViewModel.OverlayWaitingForInvitationAcceptionIsVisible = false;

        mainPageViewModel.LobbyPage.IsVisible = false;

        MessageMainPageViewModel.StartGame(true);
        mainPageViewModel.GameState.LocalPlayer.Color = "Black";

        BackgroundThreadsService.OnlineGameKeepResettingBlackInactiveCounter();
        BackgroundThreadsService.OnlineGameKeepCheckingForNextMove();

        OverlayOpponentAcceptedInvitationIsVisible = false;
        MessageLobbyViewModel.LobbyPageIsVisible = false;
        MessageMainPageViewModel.MainPageIsVisible = true;

        MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = true;
        MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = false;
    }
    #endregion

    #region Message Handlers
    internal class OpponentAcceptedInvitationViewModelRequestMessage : RequestMessage<OpponentAcceptedInvitationViewModel> { }
    #endregion
}