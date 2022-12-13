using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using static ChessAvalonia.Models.Errors;
using static ChessAvalonia.Services.MessengerService;

namespace ChessAvalonia.ViewModels.Pages.Main.Overlays;

[INotifyPropertyChanged]
public partial class MainPageErrorMessageViewModel
{
    #region Constructor
    public MainPageErrorMessageViewModel()
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

        if (ErrorReason == ErrorReason.MainPageConnectionToServerLost)
        {
            MessageMenuViewModel.MenuButtonEndOnlineGameIsVisible = false;
            MessageMenuViewModel.MenuButtonOpenLobbyIsVisible = true;

            MessageMainPageViewModel.GameState.Reset();
        }

        ErrorReason = ErrorReason.Default;
    }
    #endregion

    #region Methods
    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<MainPageErrorMessageViewModel, MainPageErrorMessageViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }

    internal void Show(ErrorReason reason)
    {
        ErrorReason = reason;
        ErrorMessageIsVisible = true;
    }
    #endregion

    #region Message Handlers
    internal class MainPageErrorMessageViewModelRequestMessage : RequestMessage<MainPageErrorMessageViewModel> { }
    #endregion
}