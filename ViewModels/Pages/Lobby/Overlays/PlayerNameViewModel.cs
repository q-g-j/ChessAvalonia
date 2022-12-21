using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ChessAvalonia.Services;
using ChessAvalonia.Models;
using static ChessAvalonia.Models.Errors;
using static ChessAvalonia.Services.MessengerService;

namespace ChessAvalonia.ViewModels.Pages.Lobby.Overlays;

[INotifyPropertyChanged]
public partial class PlayerNameViewModel
{
    #region Constructor
    public PlayerNameViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private bool overlayPlayerNameIsVisible = false;

    [ObservableProperty]
    private bool buttonRefreshIsEnabled = false;

    [ObservableProperty]
    private bool buttonOkIsEnabled =  false;

    [ObservableProperty]
    private string textBoxPlayerNameText = "";
    
    [ObservableProperty]
    private string labelPlayerNameConflict = "";

    [ObservableProperty]
    private bool textBoxPlayerNameIsFocused;
    #endregion

    partial void OnTextBoxPlayerNameIsFocusedChanged(bool value)
    {
        // CHANGE:
        //Console.WriteLine($"Focus has changed to {value}");
    }

    partial void OnTextBoxPlayerNameTextChanged(string value)
    {
        ButtonOkIsEnabled = value != "";
    }

    #region Commands
    [RelayCommand]
    private async void ButtonOk()
    {
        var localPlayer = new Player()
        {
            Name = TextBoxPlayerNameText
        };
        
        MessageMainPageViewModel.GameState.LocalPlayer = localPlayer;
        MessageLobbyViewModel.LocalPlayerName = TextBoxPlayerNameText;

        ButtonOkIsEnabled = false;
        try
        {
            Player responseLocalPlayer = await WebApiClient.WebApiClientPlayersCommands.CreatePlayerAsync(localPlayer);

            if (responseLocalPlayer.Name == localPlayer.Name)
            {
                MessageMainPageViewModel.GameState.LocalPlayer = responseLocalPlayer;
                BackgroundThreadsService.LobbyKeepResettingInactiveCounter();
                MessageLobbyViewModel.ButtonRefreshIsEnabled = true;
                OverlayPlayerNameIsVisible = false;
                ButtonOkIsEnabled = true;
            }
            else
            {
                LabelPlayerNameConflict = "This name is already taken.";
            }
        }
        catch
        {
            MessageLobbyPageErrorMessageViewModel.Show(ErrorReason.LobbyPageCannotConnectToServer);
        }
    }
    #endregion

    #region Methods
    internal void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<PlayerNameViewModel, PlayerNameViewModelViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Message Handlers
    internal class PlayerNameViewModelViewModelRequestMessage : RequestMessage<PlayerNameViewModel> { }
    #endregion
}