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

namespace ChessAvalonia.ViewModels.Pages.Main.Overlays;

[INotifyPropertyChanged]
public partial class ErrorMessageViewModel
{
    #region Constructor
    public ErrorMessageViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Properties
    private ErrorReason reason { get; set; }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private bool errorMessageIsVisible = false;

    [ObservableProperty]
    private string message = "Error Message";
    #endregion

    #region Commands
    [RelayCommand]
    private void ButtonOk()
    {
        ErrorMessageIsVisible = false;

        if (reason is ErrorReason.ConnectionLost)
        {
            MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = false;
            MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = true;

            MessageMainPageViewModel.GameState.Reset();
        }

        //reason = null;
    }
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<ErrorMessageViewModel, ErrorMessageViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }

    internal void Show(ErrorReason reason)
    {
        this.reason = reason;
        ErrorMessageIsVisible = true;
    }
    #endregion

    #region Message Handlers
    internal class ErrorMessageViewModelRequestMessage : RequestMessage<ErrorMessageViewModel> { }
    #endregion
}