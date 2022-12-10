using static ChessAvalonia.Services.MessengerService;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ChessAvalonia.Helpers;
using Avalonia.Input;
using System;

namespace ChessAvalonia.ViewModels.Windows.Main;

[INotifyPropertyChanged]
public partial class MainWindowViewModel
{
    #region Constructors
    public MainWindowViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Fields
    #endregion

    #region Bindable Properties
    #endregion

    #region Commands
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<MainWindowViewModel, MainWindowViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }

    internal void KeyDownHandler(object sender, KeyEventArgs e)
    {
        ChessDebug.WriteLine(Enum.GetName(e.Key));

        if (MessagePlayerNameViewModel.OverlayPlayerNameIsVisible)
        {
            if (e.Key == Key.Escape)
            {
                MessageLobbyViewModel.ButtonBackCommand.Execute(null);
                e.Handled = true;
            }
            else if (MessagePlayerNameViewModel.ButtonOkIsEnabled)
            {
                if (
                    e.Key == Key.Return ||
                    e.Key == Key.Enter)
                {
                    MessagePlayerNameViewModel.ButtonOkCommand.Execute(true);
                    e.Handled = true;
                }
            }
        }
        else if (MessageWaitingForInvitationAcceptionViewModel.OverlayWaitingForInvitationAcceptionIsVisible)
        {
            if (
                e.Key == Key.Return ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                MessageWaitingForInvitationAcceptionViewModel.ButtonCancelCommand.Execute(null);
                e.Handled = true;
            }
        }
        else if (MessageOpponentAcceptedInvitationViewModel.OverlayOpponentAcceptedInvitationIsVisible)
        {
            if (
                e.Key == Key.Return ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                MessageOpponentAcceptedInvitationViewModel.ButtonStartGameCommand.Execute(null);
                e.Handled = true;
            }
        }
        else if (MessageOpponentCanceledInvitationViewModel.OverlayOpponentCanceledInvitationIsVisible)
        {
            if (
                e.Key == Key.Return ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                MessageOpponentCanceledInvitationViewModel.OverlayOpponentCanceledInvitationIsVisible = false;
                e.Handled = true;
            }
        }
        else if (MessageOpponentLeftLobbyViewModel.OverlayOpponentLeftLobbyIsVisible)
        {
            if (
                e.Key == Key.Return ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                MessageOpponentLeftLobbyViewModel.ButtonOkCommand.Execute(null);
                e.Handled = true;
            }
        }
        else if (MessageOnlineGamePlayerQuitViewModel.OnlineGamePlayerQuitIsVisible)
        {
            if (
                e.Key == Key.Return ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                MessageOnlineGamePlayerQuitViewModel.ButtonOkCommand.Execute(null);
                e.Handled = true;
            }
        }
        else if (MessageLobbyViewModel.LobbyPageIsVisible)
        {
            if (e.Key == Key.Escape)
            {
                MessageLobbyViewModel.ButtonBackCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.F5)
            {
                if (MessageLobbyViewModel.ButtonRefreshIsEnabled)
                {
                    MessageLobbyViewModel.ButtonRefreshCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
        else if (MessageMainPageViewModel.MainPageIsVisible)
        {
            if (e.Key == Key.Escape)
            {
                MessageMainPageViewModel.MenuButtonPressedCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
    #endregion

    #region Message Handlers
    internal class MainWindowViewModelRequestMessage : RequestMessage<MainWindowViewModel> { }
    #endregion
}