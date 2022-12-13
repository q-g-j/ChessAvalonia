using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using static ChessAvalonia.Services.MessengerService;
namespace ChessAvalonia.ViewModels.Pages.Main.Overlays;

[INotifyPropertyChanged]
public partial class MenuViewModel
{
    #region Constructors
    public MenuViewModel()
    {
        InitializeMessageHandlers();
    }
    #endregion

    #region Fields
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private bool menuIsVisible = false;

    [ObservableProperty]
    private bool menuMainIsVisible = true;

    [ObservableProperty]
    private bool menuGameModeIsVisible = false;

    [ObservableProperty]
    private bool menuLocalGameIsVisible = false;

    [ObservableProperty]
    private bool menuOnlineGameIsVisible = false;

    [ObservableProperty]
    private bool menuButtonOpenLobbyIsVisible = true;

    [ObservableProperty]
    private bool menuButtonEndOnlineGameIsVisible = false;
    #endregion

    #region Commands
    [RelayCommand]
    private void ButtonNewGame()
    {
        MenuMainIsVisible = false;
        MenuGameModeIsVisible = true;
    }

    [RelayCommand]
    private void ButtonQuit()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow.Close();
        }
    }
    
    [RelayCommand]
    private void ButtonGameModeLocalGame()
    {
        MenuGameModeIsVisible = false;
        MenuLocalGameIsVisible = true;
    }
    
    [RelayCommand]
    private void ButtonGameModeOnlineGame()
    {
        MenuGameModeIsVisible = false;
        MenuOnlineGameIsVisible = true;
    }

    [RelayCommand]
    private void ButtonGameModeBack()
    {
        MenuGameModeIsVisible = false;
        MenuMainIsVisible = true;
    }

    [RelayCommand]
    private void ButtonLocalGameWhiteBottom()
    {
        MenuIsVisible = false;
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;

        mainPageViewModel.GameState.Reset();

        MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = false;
        MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = true;

        mainPageViewModel.StartGame(false);
    }

    [RelayCommand]
    private void ButtonLocalGameBlackBottom()
    {
        MenuIsVisible = false;
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        
        mainPageViewModel.GameState.Reset();

        MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = false;
        MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = true;

        MessageMainPageViewModel.StartGame(true);
    }

    [RelayCommand]
    private void ButtonLocalGameBack()
    {
        MenuLocalGameIsVisible = false;
        MenuGameModeIsVisible = true;
    }

    [RelayCommand]
    private void ButtonOnlineGameOpenLobby()
    {
        MenuIsVisible = false;
        MessageMainPageViewModel.MainPageIsVisible = false;
        MessageLobbyViewModel.LobbyPageIsVisible = true;

        if (MessageMainPageViewModel.GameState.LocalPlayer != null)
        {
            string localPlayerName = MessageMainPageViewModel.GameState.LocalPlayer.Name;
            MessagePlayerNameViewModel.TextBoxPlayerNameText = localPlayerName;
            MessageLobbyViewModel.LocalPlayerName = localPlayerName;
        }

        MessageLobbyViewModel.PlayerList = new();
        MessageLobbyViewModel.InvitationList = new();
        MessagePlayerNameViewModel.OverlayPlayerNameIsVisible = true;
    }

    [RelayCommand]
    private void ButtonOnlineGameEndGame()
    {
        MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = false;
        MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = true;

        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        mainPageViewModel.GameState.Opponent = null;
        mainPageViewModel.GameState.IsWaitingForMove = false;
        mainPageViewModel.GameState.IsOnlineGame = false;
        mainPageViewModel.GameState.CurrentOnlineGame = null;
        mainPageViewModel.GameState.IsCheckMate = false;
    }

    [RelayCommand]
    private void ButtonOnlineGameBack()
    {
        MenuOnlineGameIsVisible = false;
        MenuGameModeIsVisible = true;
    }
    #endregion

    #region Methods
    internal void ResetMenu()
    {
        MenuOnlineGameIsVisible = false;
        MenuLocalGameIsVisible = false;
        MenuGameModeIsVisible = false;
        MenuMainIsVisible = true;
    }
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<MenuViewModel, MenuViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Message Handlers
    internal class MenuViewModelRequestMessage : RequestMessage<MenuViewModel> { }
    #endregion
}