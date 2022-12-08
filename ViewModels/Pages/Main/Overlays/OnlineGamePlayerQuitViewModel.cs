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
public partial class OnlineGamePlayerQuitViewModel
{
    #region Constructor
    public OnlineGamePlayerQuitViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private bool onlineGamePlayerQuitIsVisible = false;

    [ObservableProperty]
    private string opponentName = "Opponent";
    #endregion

    #region Commands
    [RelayCommand]
    private void ButtonOk()
    {
        OnlineGamePlayerQuitIsVisible = false;

        MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = false;
        MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = true;
        
        MessageMainPageViewModel.GameState.Reset();
    }
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<OnlineGamePlayerQuitViewModel, OnlineGamePlayerQuitViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Message Handlers
    internal class OnlineGamePlayerQuitViewModelRequestMessage : RequestMessage<OnlineGamePlayerQuitViewModel> { }
    #endregion
    
}