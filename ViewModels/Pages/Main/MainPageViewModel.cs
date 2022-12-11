using ReactiveUI;
using Avalonia.Input;
using System.Reactive;
using System.ComponentModel;
using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using static ChessAvalonia.Services.MessengerService;
using ChessAvalonia.Models;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Avalonia.Media;
using Avalonia;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using ChessAvalonia.Views;
using ChessAvalonia.Helpers;
using ChessAvalonia.GameLogic;
using System.Reflection;
using ChessAvalonia.WebApiClient;
using ChessAvalonia.Services;
using System.Threading;

namespace ChessAvalonia.ViewModels.Pages.Main;

[INotifyPropertyChanged]
public partial class MainPageViewModel
{
    #region Constructors
    public MainPageViewModel()
    {
        GameState = new();
        ClientInstance = new(@"http://qgj.myddns.me:7002");
        //ClientInstance = new(@"http://localhost:7002");

        InitializeMessageHandlers();
        StartGame(false);
    }
    #endregion

    #region Fields
    private Image currentlyDraggedImage = null;
    private int currentlyDraggedImageOriginalCanvasLeft;
    private int currentlyDraggedImageOriginalCanvasTop;
    private Point originalPointerPosition;
    private Point dragOverCanvasPosition;
    private bool isMouseMoving = false;
    private readonly bool wasSideMenuOpen = false;
    #endregion

    #region Properties
    internal Client ClientInstance { get; set; }
    internal SquareDictionary SquareDict { get; set; } = new();
    internal GameState GameState { get; set; }
    internal UserControl LobbyPage { get; set; } = new();
    internal Coords PromotePawnCoords { get; set; }
    #endregion

    #region Bindable Properties
    [ObservableProperty]
    private ImageDictionary imageDict = new();

    [ObservableProperty]
    private double rotationAngle = 0.0;

    [ObservableProperty]
    private double chessPieceRotationRate = 0.0;

    [ObservableProperty]
    private string chessCanvasRotationCenterX = "0";

    [ObservableProperty]
    private string chessCanvasRotationCenterY = "-200";

    [ObservableProperty]
    private string canvasPointerPosition;

    [ObservableProperty]
    private ObservableCollection<string> horizontalNotationList;

    [ObservableProperty]
    private ObservableCollection<string> verticalNotationList;

    [ObservableProperty]
    private string labelMoveInfo = "";

    [ObservableProperty]
    private bool mainPageIsVisible = true;
    #endregion

    #region Commands
    [RelayCommand]
    private void MainPagePointerPressed(PointerEventArgs e)
    {
        if (MessageMenuViewModel.MenuIsVisible)
        {
            if (e.Source is not Border)
            {
                MessageMenuViewModel.MenuIsVisible = false;
            }
        }
    }

    [RelayCommand]
    private void MenuButtonPressed()
    {
        if (MessageMenuViewModel.MenuIsVisible)
        {
           MessageMenuViewModel.MenuIsVisible = false;
        }
        else
        {
           MessageMenuViewModel.ResetMenu();
           MessageMenuViewModel.MenuIsVisible = true;
        }
    }

    [RelayCommand]
    private void ChessPiecePointerPressed(object o)
    {
        if (IsInputAllowed())
        {
            if (o is PointerEventArgs e)
            {
                if (e.Source is Image image)
                {
                    var chessCanvas = image.Parent as Canvas;

                    if (e.GetCurrentPoint(chessCanvas).Properties.IsLeftButtonPressed == true)
                    {
                        chessCanvas = image.Parent as Canvas;
                        currentlyDraggedImage = image;
                        currentlyDraggedImageOriginalCanvasLeft = -1000;
                        currentlyDraggedImageOriginalCanvasTop = -1000;
                        originalPointerPosition = e.GetPosition(chessCanvas);
                        //originalPointerPosition = currentPointerPosition;

                        var chessPieceColor = SquareDict[image.Name].ChessPiece.ChessPieceColor;
                        var chessPieceType = SquareDict[image.Name].ChessPiece.ChessPieceType;
                        bool isFirstTurn = GameState.MoveList.Count == 0;
                        bool isInputAllowed = true;

                        if (isFirstTurn)
                        {
                        if (chessPieceColor == ChessPieceColor.Black)
                        {
                            isInputAllowed = false;
                        }
                        }
                        else
                        {
                        ChessPieceColor lastMoveColor = GameState.MoveList[^1].ChessPieceColor;
                        if (chessPieceColor == lastMoveColor)
                        {
                            isInputAllowed = false;
                        }
                        }

                        if (isInputAllowed && chessPieceType != ChessPieceType.Empty)
                        {
                            currentlyDraggedImageOriginalCanvasLeft = int.Parse(
                                currentlyDraggedImage.GetValue(Canvas.LeftProperty).ToString());
                            currentlyDraggedImageOriginalCanvasTop = int.Parse(
                                currentlyDraggedImage.GetValue(Canvas.TopProperty).ToString());
                        }
                        else
                        {
                            currentlyDraggedImage = null;
                        }
                    }
                }
            }
        }
    }

    [RelayCommand]
    private void MainPagePointerMoved(object o)
    {
        if (o is PointerEventArgs e)
        {
            if (currentlyDraggedImage != null)
            {
                var chessCanvas = (e.Source as Image).Parent as Canvas;

                if (e.GetCurrentPoint(chessCanvas).Properties.IsLeftButtonPressed == true)
                {
                    if (!wasSideMenuOpen)
                    {
                        if (!isMouseMoving)
                        {
                            dragOverCanvasPosition = e.GetPosition(chessCanvas);
                        }
                        isMouseMoving = true;
                        dragOverCanvasPosition = e.GetPosition(chessCanvas);
                        currentlyDraggedImage.ZIndex = 100;
                        double offsetX = originalPointerPosition.X - currentlyDraggedImageOriginalCanvasLeft;
                        double offsetY = originalPointerPosition.Y - currentlyDraggedImageOriginalCanvasTop;
                        Canvas.SetLeft(currentlyDraggedImage, dragOverCanvasPosition.X - offsetX);
                        Canvas.SetTop(currentlyDraggedImage, dragOverCanvasPosition.Y - offsetY);
                    }
                }
            }
            e.Handled = true;
        }
    }

    [RelayCommand]
    private void MainPagePointerReleased(object o)
    {
        if (IsInputAllowed())
        {
            if (o is PointerEventArgs e)
            {
                if (currentlyDraggedImage != null)
                {
                    if (isMouseMoving)
                    {
                        if (dragOverCanvasPosition.X < 0
                            || dragOverCanvasPosition.X > 480
                            || dragOverCanvasPosition.Y < 0
                            || dragOverCanvasPosition.Y > 480)
                        {
                            Canvas.SetLeft(currentlyDraggedImage, currentlyDraggedImageOriginalCanvasLeft);
                            Canvas.SetTop(currentlyDraggedImage, currentlyDraggedImageOriginalCanvasTop);
                            currentlyDraggedImageOriginalCanvasLeft = -1000;
                            currentlyDraggedImageOriginalCanvasTop = -1000;
                        }
                        else
                        {
                            Point oldPoint = new(currentlyDraggedImageOriginalCanvasLeft, currentlyDraggedImageOriginalCanvasTop);

                            Coords oldCoords = Coords.CanvasPositionToCoords(oldPoint);
                            Coords newCoords = Coords.CanvasPositionToCoords(dragOverCanvasPosition);

                            Canvas.SetLeft(currentlyDraggedImage, currentlyDraggedImageOriginalCanvasLeft);
                            Canvas.SetTop(currentlyDraggedImage, currentlyDraggedImageOriginalCanvasTop);

                            if (newCoords.X >= 1 && newCoords.X <= 8 && newCoords.Y >= 1 && newCoords.Y <= 8
                                && !(newCoords.X == oldCoords.X && newCoords.Y == oldCoords.Y))
                            {

                                ExecuteMove(oldCoords, newCoords);
                            }
                        }

                    }
                    currentlyDraggedImageOriginalCanvasLeft = -1000;
                    currentlyDraggedImageOriginalCanvasTop = -1000;
                    currentlyDraggedImage.SetValue(Panel.ZIndexProperty, 10);
                }
                currentlyDraggedImage = null;
                e.Handled = true;
            }
        }
    }
    #endregion

    #region Methods
    private async void ExecuteMove(Coords oldCoords, Coords newCoords)
    {
        ChessPiece currentlyDraggedChessPiece = SquareDict[oldCoords.String].ChessPiece;
        ChessPieceColor currentlyDraggedChessPieceColor = currentlyDraggedChessPiece.ChessPieceColor;
        ChessPieceType currentlyDraggedChessPieceType = currentlyDraggedChessPiece.ChessPieceType;

        MoveValidationData moveValidationData = MoveValidationGameLogic.ValidateCurrentMove(
                                    SquareDict, oldCoords, newCoords);

        bool isFirstMoveValid = true;
        if (GameState.MoveList.Count == 0)
        {
            isFirstMoveValid = currentlyDraggedChessPieceColor == ChessPieceColor.White;
        }

        bool isTurnCorrectColor = true;
        if (GameState.MoveList.Count > 0)
        {
            isTurnCorrectColor = GameState.MoveList[^1].ChessPieceColor != currentlyDraggedChessPieceColor;
        }

        if (isFirstMoveValid
            && isTurnCorrectColor
            && moveValidationData.IsValid)
        {
            // can an opponent's pawn be captured en passant?
            if (moveValidationData.CanCaptureEnPassant)
            {
                SquareDict[SquareDict.CoordsPawnMovedTwoSquares.String].ChessPiece = new ChessPiece();
                SquareDict[SquareDict.CoordsPawnMovedTwoSquares.String].IsOccupied = false;
                ImageDict[SquareDict.CoordsPawnMovedTwoSquares.String] = ChessPieceImages.Empty;
            }

            // has a pawn moved two tiles at once? Store its coords for the next turn...
            if (moveValidationData.MovedTwoSquares)
            {
                SquareDict.CoordsPawnMovedTwoSquares = moveValidationData.Coords[0];
            }
            else if (!GameState.IsOnlineGame)
            {
                SquareDict.CoordsPawnMovedTwoSquares = null;
            }

            // promote your pawn if it is on the opposite of the field:
            bool canPromote = PromotePawnGameLogic.CanPromote(SquareDict, oldCoords, newCoords);
            if (canPromote)
            {
                MoveChessPiece(oldCoords, newCoords, true, true);
                // GameState.MoveList.Add(new Move(oldCoords, newCoords, currentlyDraggedChessPieceColor, currentlyDraggedChessPieceType));
                PromotePawnCoords = newCoords;

                MessagePromotePawnViewModel.ChangeColor(currentlyDraggedChessPieceColor);
                MessagePromotePawnViewModel.PromotePawnIsVisible = true;
            }

            // check if a king tries to castle:
            else if (moveValidationData.CanCastle)
            {
                MoveChessPiece(oldCoords, newCoords, true, true);
                Coords rookOldCoords = moveValidationData.Coords[0];
                Coords rookNewCoords = moveValidationData.Coords[1];
                MoveChessPiece(rookOldCoords, rookNewCoords, true, true);
            }
            else
            {
                MoveChessPiece(oldCoords, newCoords, true, true);
            }

            string labelMoveInfoText = oldCoords.String + " -> " + newCoords.String;

            if (ThreateningValidationGameLogic.IsSquareThreatened(
                SquareDict, ChessPieceColor.White, SquareDict.WhiteKingCoords, false))
            {
                labelMoveInfoText += ", White king is in check!";
            }
            else if (ThreateningValidationGameLogic.IsSquareThreatened(
                SquareDict, ChessPieceColor.Black, SquareDict.BlackKingCoords, false))
            {
                labelMoveInfoText += ", Black king is in check!";
            }

            if (currentlyDraggedChessPieceColor == ChessPieceColor.Black)
            {
                labelMoveInfoText += " - It's white's turn...";
            }
            else
            {
                labelMoveInfoText += " - It's black's turn...";
            }

            if (currentlyDraggedChessPieceColor == ChessPieceColor.Black)
            {
                if (CheckMateValidationGameLogic.IsCheckMate(SquareDict, SquareDict.WhiteKingCoords))
                {
                    labelMoveInfoText = oldCoords.String + " -> " + newCoords.String + ", White is check mate!";
                    GameState.IsCheckMate = true;
                }
            }
            else if (currentlyDraggedChessPieceColor == ChessPieceColor.White)
            {
                if (CheckMateValidationGameLogic.IsCheckMate(SquareDict, SquareDict.BlackKingCoords))
                {
                    labelMoveInfoText = oldCoords.String + " -> " + newCoords.String + ", Black is check mate!";
                    GameState.IsCheckMate = true;
                }
            }

            LabelMoveInfo = labelMoveInfoText;

            GameState.MoveList.Add(new Move(oldCoords, newCoords, currentlyDraggedChessPieceColor, currentlyDraggedChessPieceType));

            if (GameState.IsOnlineGame)
            {
                GameState.CurrentOnlineGame.LastMoveStart = oldCoords.String;
                GameState.CurrentOnlineGame.LastMoveEnd = newCoords.String;
                GameState.CurrentOnlineGame.MoveInfo = LabelMoveInfo;

                if (GameState.IsCheckMate)
                {
                    GameState.CurrentOnlineGame.IsCheckMate = true;
                }
                else if (moveValidationData.MovedTwoSquares)
                {
                    GameState.CurrentOnlineGame.PawnMovedTwoSquares = true;
                    SquareDict.CoordsPawnMovedTwoSquares = null;
                }
                else
                {
                    GameState.CurrentOnlineGame.PawnMovedTwoSquares = false;
                }

                if (!canPromote)
                {
                    try
                    {
                        await WebApiClientGamesCommands.PutCurrentOnlineGame(GameState.CurrentOnlineGame.Id, GameState.CurrentOnlineGame);
                        Thread.Sleep(100);
                        BackgroundThreadsService.OnlineGameKeepCheckingForNextMove();

                        GameState.CurrentOnlineGame.PromotePawnType = ' ';
                    }
                    catch
                    {

                    }
                }
            }
        }
    }

    private void CreateNotation()
    {
        HorizontalNotationList = new();
        VerticalNotationList = new();

        if (GameState.IsRotated)
        {
            HorizontalNotationList.Add("H");
            HorizontalNotationList.Add("G");
            HorizontalNotationList.Add("F");
            HorizontalNotationList.Add("E");
            HorizontalNotationList.Add("D");
            HorizontalNotationList.Add("C");
            HorizontalNotationList.Add("B");
            HorizontalNotationList.Add("A");

            for (int i = 1; i < 9; i++)
            {
                VerticalNotationList.Add(i.ToString());
            }
        }

        else
        {            
            HorizontalNotationList.Add("A");
            HorizontalNotationList.Add("B");
            HorizontalNotationList.Add("C");
            HorizontalNotationList.Add("D");
            HorizontalNotationList.Add("E");
            HorizontalNotationList.Add("F");
            HorizontalNotationList.Add("G");
            HorizontalNotationList.Add("H");
            
            for (int i = 8; i > 0; i--)
            {
                VerticalNotationList.Add(i.ToString());
            }
        }
    }

    internal void StartGame(bool doRotate)
    {
        GameState.IsRotated = doRotate;
        GameState.IsCheckMate = false;
        GameState.MoveList = new();

        SquareDict = new SquareDictionary();
        ImageDict = new();

        CreateNotation();

        if (GameState.IsRotated)
        {
            RotationAngle = 180;
            ChessPieceRotationRate = 180;
            ChessCanvasRotationCenterX = "0";
            ChessCanvasRotationCenterY = "0";
        }
        else
        {
            RotationAngle = 0;
            ChessPieceRotationRate = 0;
            ChessCanvasRotationCenterX = "0";
            ChessCanvasRotationCenterY = " -240";
        }

        for (int col = 1; col < 9; col++)
        {
            SquareDict[Coords.IntsToCoordsString(col, 2)].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Pawn, doRotate);
            SquareDict[Coords.IntsToCoordsString(col, 2)].IsOccupied = true;
            ImageDict[Coords.IntsToCoordsString(col, 2)] = ChessPieceImages.WhitePawn;
        }
        SquareDict["A1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);
        SquareDict["B1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight, doRotate);
        SquareDict["C1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
        SquareDict["D1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Queen, doRotate);
        SquareDict["E1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.King, doRotate);
        SquareDict["F1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
        SquareDict["G1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight, doRotate);
        SquareDict["H1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);

        SquareDict["A1"].IsOccupied = true;
        SquareDict["B1"].IsOccupied = true;
        SquareDict["C1"].IsOccupied = true;
        SquareDict["D1"].IsOccupied = true;
        SquareDict["E1"].IsOccupied = true;
        SquareDict["F1"].IsOccupied = true;
        SquareDict["G1"].IsOccupied = true;
        SquareDict["H1"].IsOccupied = true;
        
        ImageDict["A1"] = ChessPieceImages.WhiteRook;
        ImageDict["B1"] = ChessPieceImages.WhiteKnight;
        ImageDict["C1"] = ChessPieceImages.WhiteBishop;
        ImageDict["D1"] = ChessPieceImages.WhiteQueen;
        ImageDict["E1"] = ChessPieceImages.WhiteKing;
        ImageDict["F1"] = ChessPieceImages.WhiteBishop;
        ImageDict["G1"] = ChessPieceImages.WhiteKnight;
        ImageDict["H1"] = ChessPieceImages.WhiteRook;

        for (int col = 1; col < 9; col++)
        {
            SquareDict[Coords.IntsToCoordsString(col, 7)].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            SquareDict[Coords.IntsToCoordsString(col, 7)].IsOccupied = true;
            ImageDict[Coords.IntsToCoordsString(col, 7)] = ChessPieceImages.BlackPawn;
        }

        SquareDict["A8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);
        SquareDict["B8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
        SquareDict["C8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
        SquareDict["D8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Queen, doRotate);
        SquareDict["E8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.King, doRotate);
        SquareDict["F8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
        SquareDict["G8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
        SquareDict["H8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);

        SquareDict["A8"].IsOccupied = true;
        SquareDict["B8"].IsOccupied = true;
        SquareDict["C8"].IsOccupied = true;
        SquareDict["D8"].IsOccupied = true;
        SquareDict["E8"].IsOccupied = true;
        SquareDict["F8"].IsOccupied = true;
        SquareDict["G8"].IsOccupied = true;
        SquareDict["H8"].IsOccupied = true;
        
        ImageDict["A8"] = ChessPieceImages.BlackRook;
        ImageDict["B8"] = ChessPieceImages.BlackKnight;
        ImageDict["C8"] = ChessPieceImages.BlackBishop;
        ImageDict["D8"] = ChessPieceImages.BlackQueen;
        ImageDict["E8"] = ChessPieceImages.BlackKing;
        ImageDict["F8"] = ChessPieceImages.BlackBishop;
        ImageDict["G8"] = ChessPieceImages.BlackKnight;
        ImageDict["H8"] = ChessPieceImages.BlackRook;

        LabelMoveInfo = "It's white's turn...";
    }

    internal void MoveChessPiece(Coords oldCoords, Coords newCoords, bool doChangeCounter, bool doChangeImage)
    {
        SquareDict[newCoords.String].ChessPiece = SquareDict[oldCoords.String].ChessPiece;
        SquareDict[oldCoords.String].ChessPiece = new ChessPiece();

        if (doChangeImage)
        {
            ImageDict[newCoords.String] = ImageDict[oldCoords.String];
            ImageDict[oldCoords.String] = ChessPieceImages.Empty;
        }

        if (doChangeCounter)
        {
            SquareDict[oldCoords.String].IsOccupied = false;
            SquareDict[newCoords.String].IsOccupied = true;

            SquareDict[newCoords.String].ChessPiece.MoveCount++;
            SquareDict[newCoords.String].ChessPiece.HasMoved = true;
        }

        if (SquareDict[newCoords.String].ChessPiece.ChessPieceType == ChessPieceType.King)
        {
            if (SquareDict[newCoords.String].ChessPiece.ChessPieceColor == ChessPieceColor.White)
            {
                SquareDict.WhiteKingCoords = newCoords;
            }
            else
            {
                SquareDict.BlackKingCoords = newCoords;
            }
        }
    }
    private bool IsInputAllowed()
    {
        if (
            GameState.IsCheckMate
            || MessageMenuViewModel.MenuIsVisible == true
            || MessagePromotePawnViewModel.PromotePawnIsVisible == true
            || GameState.IsWaitingForMove)
        {
            return false;
        }

        return true;
    }

    private void InitializeMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<MainPageViewModel, MainPageViewModelRequestMessage>(this, (r, m) =>
        {
            m.Reply(r);
        });
    }
    #endregion

    #region Message Handlers:
    internal class MainPageViewModelRequestMessage : RequestMessage<MainPageViewModel> { }
    #endregion
}
