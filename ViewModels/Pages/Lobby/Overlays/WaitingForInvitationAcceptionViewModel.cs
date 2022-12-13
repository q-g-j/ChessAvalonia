using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ChessAvalonia.ViewModels.Pages.Main;
using static ChessAvalonia.Services.MessengerService;

namespace ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

[INotifyPropertyChanged]
public partial class WaitingForInvitationAcceptionViewModel
{
    #region Constructor
    public WaitingForInvitationAcceptionViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private bool overlayWaitingForInvitationAcceptionIsVisible = false;

    [ObservableProperty]
    private string opponentName = "opponent";
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<WaitingForInvitationAcceptionViewModel, WaitingForInvitationAcceptionViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Commands
    [RelayCommand]
    private async void ButtonCancel()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        try
        {
            await WebApiClient.WebApiClientInvitationsCommands.CancelInvitationAsync(
                mainPageViewModel.GameState.Opponent.Id, mainPageViewModel.GameState.LocalPlayer);
        }
        catch
        {

        }
        mainPageViewModel.GameState.IsOnlineGame = false;
        mainPageViewModel.GameState.CurrentOnlineGame = null;

        OverlayWaitingForInvitationAcceptionIsVisible = false;
    }
    #endregion

    #region Message Handlers
    internal class WaitingForInvitationAcceptionViewModelRequestMessage : RequestMessage<WaitingForInvitationAcceptionViewModel> { }
    #endregion
}