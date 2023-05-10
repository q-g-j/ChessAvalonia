using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ChessAvalonia.Services;
using ChessAvalonia.WebApiClient;
using ChessAvalonia.Models;
using static ChessAvalonia.Services.MessengerService;
using System.Diagnostics.CodeAnalysis;

namespace ChessAvalonia.ViewModels.Pages.Main.Overlays;

[INotifyPropertyChanged]
public partial class PromotePawnViewModel
{
    #region Constructors
    public PromotePawnViewModel()
    {
        overlayPromotePawnList = new()
        {
            ChessPieceImages.WhiteBishop,
            ChessPieceImages.WhiteKnight,
            ChessPieceImages.WhiteRook,
            ChessPieceImages.WhiteQueen
        };

        InitializeMessageHandlers();
    }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private ObservableCollection<string> overlayPromotePawnList;

    [ObservableProperty]
    private bool promotePawnIsVisible = false;
    #endregion

    #region Commands
    [RelayCommand]
    [RequiresUnreferencedCode("Calls ChessAvalonia.ViewModels.Pages.Main.Overlays.PromotePawnViewModel.PromotePawn(ChessPieceType)")]
    private void OverlayPromotePawnSelectChessPiece(object chessPiece)
    {
        string chessPieceString = chessPiece as string;
        ChessPieceType chessPieceType;

        if (chessPieceString == "Queen")
        {
            chessPieceType = ChessPieceType.Queen;
        }
        else if (chessPieceString == "Rook")
        {
            chessPieceType = ChessPieceType.Rook;
        }
        else if (chessPieceString == "Bishop")
        {
            chessPieceType = ChessPieceType.Bishop;
        }
        else
        {
            chessPieceType = ChessPieceType.Knight;
        }

        PromotePawn(chessPieceType);
    }
    #endregion

    #region Methods
    internal void ChangeColor(ChessPieceColor newColor)
    {
        if (newColor == ChessPieceColor.White)
        {
            OverlayPromotePawnList = new()
            {
                ChessPieceImages.WhiteBishop,
                ChessPieceImages.WhiteKnight,
                ChessPieceImages.WhiteRook,
                ChessPieceImages.WhiteQueen
            };
        }
        else
        {
            OverlayPromotePawnList = new()
            {
                ChessPieceImages.BlackBishop,
                ChessPieceImages.BlackKnight,
                ChessPieceImages.BlackRook,
                ChessPieceImages.BlackQueen
            };
        }
    }

    [RequiresUnreferencedCode("Calls ChessAvalonia.WebApiClient.WebApiClientGamesCommands.PutCurrentOnlineGame(Int32, OnlineGame)")]
    internal async void PromotePawn(ChessPieceType chessPieceType)
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        SquareDictionary squareDict = mainPageViewModel.SquareDict;
        var chessBoard = mainPageViewModel.CanvasImages;

        MessagePromotePawnViewModel.PromotePawnIsVisible = false;
        ChessPieceColor ownColor = squareDict[mainPageViewModel.PromotePawnCoords.String].ChessPiece.ChessPieceColor;
        squareDict[mainPageViewModel.PromotePawnCoords.String].ChessPiece = new ChessPiece(ownColor, chessPieceType, mainPageViewModel.GameState.IsRotated);
        chessBoard[Coords.CoordsStringToIndex(mainPageViewModel.PromotePawnCoords.String)].Image = ChessPieceImages.GetChessPieceImage(ownColor, chessPieceType);

        if (mainPageViewModel.GameState.IsOnlineGame)
        {
            if (chessPieceType == ChessPieceType.Bishop)
            {
                mainPageViewModel.GameState.CurrentOnlineGame.PromotePawnType = 'B';
            }
            else if (chessPieceType == ChessPieceType.Knight)
            {
                mainPageViewModel.GameState.CurrentOnlineGame.PromotePawnType = 'K';
            }
            else if (chessPieceType == ChessPieceType.Rook)
            {
                mainPageViewModel.GameState.CurrentOnlineGame.PromotePawnType = 'R';
            }
            else if (chessPieceType == ChessPieceType.Queen)
            {
                mainPageViewModel.GameState.CurrentOnlineGame.PromotePawnType = 'Q';
            }
            try
            {
                await WebApiClientGamesCommands.PutCurrentOnlineGame(mainPageViewModel.GameState.CurrentOnlineGame.Id, mainPageViewModel.GameState.CurrentOnlineGame);
            }
            catch
            {

            }
            BackgroundThreadsService.OnlineGameKeepCheckingForNextMove();
        }
    }

    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<PromotePawnViewModel, PromotePawnViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Message Handlers
    internal class PromotePawnViewModelRequestMessage : RequestMessage<PromotePawnViewModel> { }
    #endregion
}