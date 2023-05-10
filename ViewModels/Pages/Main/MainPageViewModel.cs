using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using ChessAvalonia.GameLogic;
using ChessAvalonia.WebApiClient;
using ChessAvalonia.Services;
using ChessAvalonia.Models;
using static ChessAvalonia.Services.MessengerService;
using System.Diagnostics.CodeAnalysis;
using ChessAvalonia.Controls;
using System.Linq;
using Avalonia.VisualTree;
using Avalonia.Controls.Presenters;
using static System.Diagnostics.Debug;
using System;
using System.Xml.Linq;

namespace ChessAvalonia.ViewModels.Pages.Main;

[INotifyPropertyChanged]
public partial class MainPageViewModel
{
    #region Constructors
    public MainPageViewModel()
    {
        GameState = new();
        ClientInstance = new(@"http://qgj.myddns.me:7002");
        // ClientInstance = new(@"http://localhost:7002");

        InitializeMessageHandlers();
        StartGame(false);
    }
    #endregion

    #region Fields
    private int capturedWhiteCount = 0;
    private int capturedBlackCount = 0;
    private string currentlyDraggedImageName;
    private int currentlyDraggedImageOriginalCanvasLeft;
    private int currentlyDraggedImageOriginalCanvasTop;
    private Point pointerPosition;
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

    [ObservableProperty]
    private int slideInBoxMaxWidth = 500;

    [ObservableProperty]
    private bool slideInBoxIsVisible;


    private string position;
    public string Position
    {
        get => position;
        set
        {
            position = value;
            OnPropertyChanged();
            if (!string.IsNullOrEmpty(value))
            {
                var split = position.Split(',');
                pointerPosition = new(int.Parse(split[0]), int.Parse(split[1]));
            }
        }
    }

    [ObservableProperty]
    private CapturedChessPieces capturedWhiteChessPieces;

    [ObservableProperty]
    private CapturedChessPieces capturedBlackChessPieces;

    [ObservableProperty]
    private int testLeft;

    [ObservableProperty]
    private ObservableCollection<CanvasImage> canvasImages;

    [ObservableProperty]
    private ObservableCollection<CanvasRectangle> canvasRectangles;

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

        var control = e.Source as Control;
        var parent = control.Parent;
        bool doCloseSlideInBox = true;

        while (parent != null && doCloseSlideInBox)
        {
            if (parent.Name == "CapturedChessPiecesControlGrid")
            {
                doCloseSlideInBox = false;
            }
            parent = parent.Parent;
        }

        if (doCloseSlideInBox)
        {
            CapturedChessPiecesControl sideMenu = WeakReferenceMessenger.Default.Send<CapturedChessPiecesControl.CapturedChessPiecesControlRequestMessage>();
            if (!sideMenu.IsMouseOverArrowLabel)
            {
                if (sideMenu.SlideInBoxWidth == 500)
                {
                    sideMenu.SlideInBoxIsVisible = false;
                }
            }
        }
    }

    [RelayCommand]
    private void MenuButtonPressed()
    {
        CapturedChessPiecesControl sideMenu = WeakReferenceMessenger.Default.Send<CapturedChessPiecesControl.CapturedChessPiecesControlRequestMessage>();
        if (sideMenu.SlideInBoxWidth == 500)
        {
            sideMenu.SlideInBoxIsVisible = false;
        }
        else if (MessageMenuViewModel.MenuIsVisible)
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
    private void ChessPiecePointerPressed(string name)
    {
        if (IsInputAllowed())
        {
            if (!string.IsNullOrEmpty(name))
            {
                TestLeft = 100;
                currentlyDraggedImageName = name;
                currentlyDraggedImageOriginalCanvasLeft = -1000;
                currentlyDraggedImageOriginalCanvasTop = -1000;
                originalPointerPosition = pointerPosition;
                CanvasImages[Coords.CoordsStringToIndex(name)].ZIndex = 100;

                var chessPieceColor = SquareDict[name].ChessPiece.ChessPieceColor;
                var chessPieceType = SquareDict[name].ChessPiece.ChessPieceType;
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
                    currentlyDraggedImageOriginalCanvasLeft =
                        CanvasImages[Coords.CoordsStringToIndex(name)].CanvasLeft;
                    currentlyDraggedImageOriginalCanvasTop =
                        CanvasImages[Coords.CoordsStringToIndex(name)].CanvasTop;
                }
                else
                {
                    currentlyDraggedImageName = "";
                }
            }
        }
    }

    [RelayCommand]
    private void MainPagePointerMoved(object o)
    {
        if (o is PointerEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentlyDraggedImageName))
            {
                if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed == true)
                {
                    if (!wasSideMenuOpen)
                    {
                        isMouseMoving = true;
                        dragOverCanvasPosition = pointerPosition;
                        double offsetX = originalPointerPosition.X - currentlyDraggedImageOriginalCanvasLeft;
                        double offsetY = originalPointerPosition.Y - currentlyDraggedImageOriginalCanvasTop;

                        CanvasImages[Coords.CoordsStringToIndex(currentlyDraggedImageName)].CanvasLeft = (int)(dragOverCanvasPosition.X - offsetX);
                        CanvasImages[Coords.CoordsStringToIndex(currentlyDraggedImageName)].CanvasTop = (int)(dragOverCanvasPosition.Y - offsetY);
                    }
                }
            }
            e.Handled = true;
        }
    }

    [RelayCommand]
    [RequiresUnreferencedCode("Calls ChessAvalonia.ViewModels.Pages.Main.MainPageViewModel.ExecuteMove(Coords, Coords)")]
    private void MainPagePointerReleased(object o)
    {
        if (IsInputAllowed())
        {
            if (o is PointerEventArgs e)
            {
                if (!string.IsNullOrEmpty(currentlyDraggedImageName))
                {
                    if (isMouseMoving)
                    {
                        if (dragOverCanvasPosition.X < 0
                            || dragOverCanvasPosition.X > 480
                            || dragOverCanvasPosition.Y < 0
                            || dragOverCanvasPosition.Y > 480)
                        {
                            CanvasImages[Coords.CoordsStringToIndex(currentlyDraggedImageName)].CanvasLeft = currentlyDraggedImageOriginalCanvasLeft;
                            CanvasImages[Coords.CoordsStringToIndex(currentlyDraggedImageName)].CanvasTop = currentlyDraggedImageOriginalCanvasTop;
                            currentlyDraggedImageOriginalCanvasLeft = -1000;
                            currentlyDraggedImageOriginalCanvasTop = -1000;
                        }
                        else
                        {
                            Point oldPoint = new(currentlyDraggedImageOriginalCanvasLeft, currentlyDraggedImageOriginalCanvasTop);

                            Coords oldCoords = Coords.CanvasPositionToCoords(oldPoint);
                            Coords newCoords = Coords.CanvasPositionToCoords(dragOverCanvasPosition);

                            CanvasImages[Coords.CoordsStringToIndex(currentlyDraggedImageName)].CanvasLeft = currentlyDraggedImageOriginalCanvasLeft;
                            CanvasImages[Coords.CoordsStringToIndex(currentlyDraggedImageName)].CanvasTop = currentlyDraggedImageOriginalCanvasTop;

                            if (newCoords.X >= 1 && newCoords.X <= 8 && newCoords.Y >= 1 && newCoords.Y <= 8
                                && !(newCoords.X == oldCoords.X && newCoords.Y == oldCoords.Y))
                            {

                                ExecuteMove(oldCoords, newCoords);
                            }
                        }

                    }
                    currentlyDraggedImageOriginalCanvasLeft = -1000;
                    currentlyDraggedImageOriginalCanvasTop = -1000;
                    CanvasImages[Coords.CoordsStringToIndex(currentlyDraggedImageName)].ZIndex = 100;
                }
                currentlyDraggedImageName = "";
                e.Handled = true;
            }
        }
    }
    #endregion

    #region Methods
    [RequiresUnreferencedCode("Calls ChessAvalonia.WebApiClient.WebApiClientGamesCommands.PutCurrentOnlineGame(Int32, OnlineGame)")]
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
                AddToCapturedList(SquareDict.CoordsPawnMovedTwoSquares, currentlyDraggedChessPieceColor);
                SquareDict[SquareDict.CoordsPawnMovedTwoSquares.String].ChessPiece = new ChessPiece();
                SquareDict[SquareDict.CoordsPawnMovedTwoSquares.String].IsOccupied = false;

                CanvasImages[Coords.CoordsStringToIndex(SquareDict.CoordsPawnMovedTwoSquares.String)].Image = ChessPieceImages.Empty;
            }

            // has a pawn moved two squares at once? Store its coords for the next turn...
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
                AddToCapturedList(newCoords, currentlyDraggedChessPieceColor);

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
                AddToCapturedList(newCoords, currentlyDraggedChessPieceColor);
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

        SquareDict = new();
        FillCanvasRectangles();
        FillCanvasImages();

        CapturedWhiteChessPieces = new();
        CapturedBlackChessPieces = new();

        CreateNotation();

        capturedWhiteCount = 0;
        capturedBlackCount = 0;

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

            string coordsString = Coords.IntsToCoordsString(col, 2);
            CanvasImages[Coords.CoordsStringToIndex(coordsString)].Image = ChessPieceImages.WhitePawn;
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

        CanvasImages[Coords.CoordsStringToIndex("A1")].Image = ChessPieceImages.WhiteRook;
        CanvasImages[Coords.CoordsStringToIndex("B1")].Image = ChessPieceImages.WhiteKnight;
        CanvasImages[Coords.CoordsStringToIndex("C1")].Image = ChessPieceImages.WhiteBishop;
        CanvasImages[Coords.CoordsStringToIndex("D1")].Image = ChessPieceImages.WhiteQueen;
        CanvasImages[Coords.CoordsStringToIndex("E1")].Image = ChessPieceImages.WhiteKing;
        CanvasImages[Coords.CoordsStringToIndex("F1")].Image = ChessPieceImages.WhiteBishop;
        CanvasImages[Coords.CoordsStringToIndex("G1")].Image = ChessPieceImages.WhiteKnight;
        CanvasImages[Coords.CoordsStringToIndex("H1")].Image = ChessPieceImages.WhiteRook;

        for (int col = 1; col < 9; col++)
        {
            SquareDict[Coords.IntsToCoordsString(col, 7)].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            SquareDict[Coords.IntsToCoordsString(col, 7)].IsOccupied = true;
            string coordsString = Coords.IntsToCoordsString(col, 7);
            CanvasImages[Coords.CoordsStringToIndex(coordsString)].Image = ChessPieceImages.BlackPawn;
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

        CanvasImages[Coords.CoordsStringToIndex("A8")].Image = ChessPieceImages.BlackRook;
        CanvasImages[Coords.CoordsStringToIndex("B8")].Image = ChessPieceImages.BlackKnight;
        CanvasImages[Coords.CoordsStringToIndex("C8")].Image = ChessPieceImages.BlackBishop;
        CanvasImages[Coords.CoordsStringToIndex("D8")].Image = ChessPieceImages.BlackQueen;
        CanvasImages[Coords.CoordsStringToIndex("E8")].Image = ChessPieceImages.BlackKing;
        CanvasImages[Coords.CoordsStringToIndex("F8")].Image = ChessPieceImages.BlackBishop;
        CanvasImages[Coords.CoordsStringToIndex("G8")].Image = ChessPieceImages.BlackKnight;
        CanvasImages[Coords.CoordsStringToIndex("H8")].Image = ChessPieceImages.BlackRook;

        LabelMoveInfo = "It's white's turn...";
    }

    internal void AddToCapturedList(Coords capturedCoords, ChessPieceColor capturingColor)
    {
        if (SquareDict[capturedCoords.String].IsOccupied)
        {
            string newCoordsChessPieceImage = CanvasImages[Coords.CoordsStringToIndex(capturedCoords.String)].Image;

            if (capturingColor == ChessPieceColor.White)
            {
                CapturedBlackChessPieces[capturedBlackCount].Image = newCoordsChessPieceImage;
                capturedBlackCount++;
            }
            else
            {
                CapturedWhiteChessPieces[capturedWhiteCount].Image = newCoordsChessPieceImage;
                capturedWhiteCount++;
            }
        }
    }

    internal void MoveChessPiece(Coords oldCoords, Coords newCoords, bool doChangeCounter, bool doChangeImage)
    {
        SquareDict[newCoords.String].ChessPiece = SquareDict[oldCoords.String].ChessPiece;
        SquareDict[oldCoords.String].ChessPiece = new ChessPiece();

        if (doChangeImage)
        {
            CanvasImages[Coords.CoordsStringToIndex(newCoords.String)].Image = CanvasImages[Coords.CoordsStringToIndex(oldCoords.String)].Image;
            CanvasImages[Coords.CoordsStringToIndex(oldCoords.String)].Image = ChessPieceImages.Empty;
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

    private void FillCanvasRectangles()
    {
        CanvasRectangles = new();
        int t = 0;
        for (int col = 8; col > 0; col--, t++)
        {
            int l = 0;
            for (int row = 1; row < 9; row++, l++)
            {
                CanvasRectangle cell = new()
                {
                    CanvasLeft = l * 60,
                    CanvasTop = t * 60,
                };

                if (row % 2 != 0)
                {
                    if (col % 2 == 0)
                    {
                        cell.BackgroundColor = "#ffce9e";
                    }
                    else
                    {
                        cell.BackgroundColor = "#d18b47";
                    }
                }
                else
                {
                    if (col % 2 != 0)
                    {
                        cell.BackgroundColor = "#ffce9e";
                    }
                    else
                    {
                        cell.BackgroundColor = "#d18b47";
                    }
                }
                CanvasRectangles.Add(cell);
            }
        }
    }

    private void FillCanvasImages()
    {
        CanvasImages = new();
        int t = 0;
        for (int col = 8; col > 0; col--, t++)
        {
            int l = 0;
            for (int row = 1; row < 9; row++, l++)
            {
                CanvasImage cell = new()
                {
                    CellName = Coords.IntsToCoordsString(row, col),
                    CanvasLeft = l * 60,
                    CanvasTop = t * 60,
                    Image = ChessPieceImages.Empty,
                };

                CanvasImages.Add(cell);
            }
        }
    }
    #endregion

    #region Message Handlers:
    internal class MainPageViewModelRequestMessage : RequestMessage<MainPageViewModel> { }
    #endregion
}
