using System;
using System.Collections.ObjectModel;
using ChessAvalonia.Helpers;
using ChessAvalonia.Models;
using static ChessAvalonia.Services.MessengerService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Avalonia.Input;

namespace ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

[INotifyPropertyChanged]
public partial class OpponentLeftLobbyViewModel
{
    #region Constructor
    public OpponentLeftLobbyViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private string opponentName = "Opponent";

    [ObservableProperty]
    private bool overlayOpponentLeftLobbyIsVisible = false;
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<OpponentLeftLobbyViewModel, OpponentLeftLobbyViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Commands
    [RelayCommand]
    private async void ButtonOk()
    {
        MessageLobbyViewModel.PlayerList = await WebApiClient.WebApiClientPlayersCommands.GetAllPlayersAsync();
        MessageLobbyViewModel.InvitationList = await WebApiClient.WebApiClientInvitationsCommands.GetPlayerInvitationsAsync(
            MessageMainPageViewModel.GameState.LocalPlayer.Id
        );

        MessageOpponentLeftLobbyViewModel.OverlayOpponentLeftLobbyIsVisible = false;
    }
    #endregion

    #region Message Handlers
    internal class OpponentLeftLobbyViewModelRequestMessage : RequestMessage<OpponentLeftLobbyViewModel> { }
    #endregion
}