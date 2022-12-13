using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ChessAvalonia.ViewModels.Pages.Main;
using ChessAvalonia.Models;
using ChessAvalonia.Services;
using static ChessAvalonia.Models.Errors;
using static ChessAvalonia.Services.MessengerService;

namespace ChessAvalonia.ViewModels.Pages.Lobby;

[INotifyPropertyChanged]
public partial class LobbyPageViewModel
{
    #region Constructors
    public LobbyPageViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Fields
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private ObservableCollection<Player> playerList;

    [ObservableProperty]
    private ObservableCollection<Player> invitationList;

    [ObservableProperty]
    private object dataGridPlayerListSelectedItem;

    [ObservableProperty]
    private object dataGridInvitationListSelectedItem;

    [ObservableProperty]
    private string localPlayerName = " ";

    [ObservableProperty]
    private bool overlayWaitingForInvitationAcceptedIsVisible = false;

    [ObservableProperty]
    private bool lobbyPageIsVisible = false;

    [ObservableProperty]
    private bool buttonInviteIsEnabled = false;

    [ObservableProperty]
    private bool buttonAcceptInvitationIsEnabled = false;

    [ObservableProperty]
    private bool buttonRefreshIsEnabled = false;
    #endregion    

    partial void OnLobbyPageIsVisibleChanged(bool value)
    {
        if (value == false)
        {
            if (MessageMainPageViewModel.GameState.LocalPlayer != null)
            {
                try
                {
                    Task.Run(async () =>
                        await WebApiClient.WebApiClientPlayersCommands.DeletePlayerAsync(
                            MessageMainPageViewModel.GameState.LocalPlayer.Id)
                        );
                }
                catch
                {

                }
            }
            MessageLobbyPageErrorMessageViewModel.ErrorMessageIsVisible = false;
        }
        else
        {
            MessagePlayerNameViewModel.LabelPlayerNameConflict = "";
        }
    }

    #region Commands
    [RelayCommand]
    private void DataGridPlayerListSelectionChanged()
    {
        if (DataGridPlayerListSelectedItem is Player selectedPlayer)
        {
            if (selectedPlayer.Name != MessageMainPageViewModel.GameState.LocalPlayer.Name)
            {
                ButtonInviteIsEnabled = true;
            }
            else
            {
                ButtonInviteIsEnabled = false;
            }
        }
        else
        {
            ButtonInviteIsEnabled = false;
        }
    }

    [RelayCommand]
    private void DataGridInvitationListSelectionChanged()
    {
        ButtonAcceptInvitationIsEnabled = DataGridInvitationListSelectedItem is Player;
    }

    [RelayCommand]
    private async void ButtonInvite()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;

        Player localPlayer = MessageMainPageViewModel.GameState.LocalPlayer;
        Player selectedPlayer = DataGridPlayerListSelectedItem as Player;
        string selectedPlayerName = selectedPlayer.Name;
        int selectedPlayerId = selectedPlayer.Id;

        mainPageViewModel.GameState.Opponent = null;
        mainPageViewModel.GameState.IsWaitingForMove = false;
        mainPageViewModel.GameState.IsOnlineGame = false;
        mainPageViewModel.GameState.CurrentOnlineGame = null;

        try
        {
            IEnumerable<Player> playerListTemp = await WebApiClient.WebApiClientPlayersCommands.GetAllPlayersAsync();
            Player opponentInPlayerListTemp = playerListTemp.Where(a => a.Name == selectedPlayerName).FirstOrDefault();

            if (opponentInPlayerListTemp is not null)
            {
                MessageMainPageViewModel.GameState.IsOnlineGame = true;
                MessageMainPageViewModel.GameState.Opponent = opponentInPlayerListTemp;
                MessageWaitingForInvitationAcceptionViewModel.OpponentName = selectedPlayerName;
                MessageWaitingForInvitationAcceptionViewModel.OverlayWaitingForInvitationAcceptionIsVisible = true;
                await WebApiClient.WebApiClientInvitationsCommands.InvitePlayerAsync(selectedPlayerId, localPlayer);

                MessageMainPageViewModel.GameState.CurrentOnlineGame = new();

                BackgroundThreadsService.LobbyKeepCheckingForOpponentAcception();
            }
            else
            {
                MessageOpponentLeftLobbyViewModel.OpponentName = selectedPlayerName;
                MessageOpponentLeftLobbyViewModel.OverlayOpponentLeftLobbyIsVisible = true;
            }
        }
        catch
        {
            MessageLobbyPageErrorMessageViewModel.Show(ErrorReason.LobbyPageConnectionToServerLost);
        }
    }

    [RelayCommand]
    private async void ButtonAcceptInvitation()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        Player localPlayer = mainPageViewModel.GameState.LocalPlayer;
        Player selectedPlayer = DataGridInvitationListSelectedItem as Player;
        string selectedPlayerName = selectedPlayer.Name;
        int selectedPlayerId = selectedPlayer.Id;

        try
        {
            IEnumerable<Player> invitationListTemp = await WebApiClient.WebApiClientInvitationsCommands.GetPlayerInvitationsAsync(localPlayer.Id);
            Player opponentInInvitationListTemp = invitationListTemp.Where(a => a.Name == selectedPlayerName).FirstOrDefault();

            if (opponentInInvitationListTemp is not null)
            {
                mainPageViewModel.GameState.Opponent = null;
                mainPageViewModel.GameState.IsWaitingForMove = false;
                mainPageViewModel.GameState.IsOnlineGame = false;
                mainPageViewModel.GameState.CurrentOnlineGame = null;

                OnlineGame newOnlineGame = new(localPlayer.Id, selectedPlayerId);

                mainPageViewModel.GameState.LocalPlayer.Color = "White";

                mainPageViewModel.GameState.IsOnlineGame = true;
                mainPageViewModel.GameState.Opponent = opponentInInvitationListTemp;
                MessageOpponentAcceptedInvitationViewModel.OpponentName = selectedPlayerName;

                mainPageViewModel.GameState.CurrentOnlineGame =
                    await WebApiClient.WebApiClientGamesCommands.StartNewOnlineGameAsync(newOnlineGame);

                BackgroundThreadsService.OnlineGameKeepResettingWhiteInactiveCounter();

                LobbyPageIsVisible = false;
                MessageMainPageViewModel.MainPageIsVisible = true;

                MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = true;
                MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = false;

                mainPageViewModel.StartGame(false);
            }
            else
            {
                MessageOpponentCanceledInvitationViewModel.OpponentName = selectedPlayerName;
                MessageOpponentCanceledInvitationViewModel.OverlayOpponentCanceledInvitationIsVisible = true;
            }
        }
        catch
        {
            MessageLobbyPageErrorMessageViewModel.Show(ErrorReason.LobbyPageConnectionToServerLost);
        }
    }

    [RelayCommand]
    private async void ButtonRefresh()
    {
        try
        {
            PlayerList = await WebApiClient.WebApiClientPlayersCommands.GetAllPlayersAsync();
            InvitationList = await WebApiClient.WebApiClientInvitationsCommands.GetPlayerInvitationsAsync(
                MessageMainPageViewModel.GameState.LocalPlayer.Id
                );
        }
        catch
        {
            MessageLobbyPageErrorMessageViewModel.Show(ErrorReason.LobbyPageCannotConnectToServer);
        }
    }

    [RelayCommand]
    private void ButtonBack()
    {
        MessagePlayerNameViewModel.OverlayPlayerNameIsVisible = false;
        LobbyPageIsVisible = false;
        MessageMainPageViewModel.MainPageIsVisible = true;
    }
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<LobbyPageViewModel, LobbyPageViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Message Handlers
    internal class LobbyPageViewModelRequestMessage : RequestMessage<LobbyPageViewModel> { }
    #endregion
}