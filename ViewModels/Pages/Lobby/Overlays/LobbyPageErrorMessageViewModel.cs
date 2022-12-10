using System;
using System.Collections.ObjectModel;
using ChessAvalonia.Helpers;
using ChessAvalonia.Models;
using static ChessAvalonia.Services.MessengerService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using static ChessAvalonia.Models.Errors;
using Avalonia.Input;
using System.Drawing;

namespace ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

[INotifyPropertyChanged]
public partial class LobbyPageErrorMessageViewModel
{
    #region Constructor
    public LobbyPageErrorMessageViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Properties
    private ErrorReason errorReason;
    private ErrorReason ErrorReason
    {
        get => errorReason;
        set
        {
            errorReason = value;
            LabelErrorMessage = ErrorMessages[value];
        }
    }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private bool errorMessageIsVisible = false;

    [ObservableProperty]
    private string labelErrorMessage = ErrorMessages[ErrorReason.Default];
    #endregion

    #region Commands
    [RelayCommand]
    private void ButtonOk()
    {
        ErrorMessageIsVisible = false;

        if (ErrorReason == ErrorReason.LobbyPageCannotConnectToServer)
        {
            MessageLobbyViewModel.LobbyPageIsVisible = false;
            MessageMainPageViewModel.MainPageIsVisible = true;
        }

        else if (ErrorReason == ErrorReason.LobbyPageConnectionToServerLost)
        {
            MessageLobbyViewModel.LobbyPageIsVisible = false;
            MessageMainPageViewModel.MainPageIsVisible = true;
        }

        ErrorReason = ErrorReason.Default;
    }
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<LobbyPageErrorMessageViewModel, LobbyPageErrorMessageViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }

    internal void Show(ErrorReason reason)
    {
        ErrorReason = reason;
        MessageLobbyViewModel.ButtonRefreshIsEnabled = false;
        MessageWaitingForInvitationAcceptionViewModel.OverlayWaitingForInvitationAcceptionIsVisible = false;
        ErrorMessageIsVisible = true;
    }
    #endregion

    #region Message Handlers
    internal class LobbyPageErrorMessageViewModelRequestMessage : RequestMessage<LobbyPageErrorMessageViewModel> { }
    #endregion
}