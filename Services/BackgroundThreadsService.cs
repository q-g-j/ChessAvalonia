using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

using ChessAvalonia.Models;
using ChessAvalonia.ViewModels;
using ChessAvalonia.WebApiClient;
using ChessAvalonia.Views;
using static ChessAvalonia.Services.MessengerService;
using ChessAvalonia.ViewModels.Windows.Main;
using ChessAvalonia.ViewModels.Pages.Main;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;
using ChessAvalonia.ViewModels.Pages.Lobby;
using ChessAvalonia.Helpers;
using ChessAvalonia.GameLogic;
using DynamicData;
using System.Collections.Generic;
using static ChessAvalonia.Models.Errors;
using System.Security;

namespace ChessAvalonia.Services;
internal static class BackgroundThreadsService
{
    internal static void LobbyKeepCheckingForOpponentAcception()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        MenuViewModel menuViewModel = MessageMenuViewModel;

        var threadStart = new ThreadStart(() =>
        {
            bool isException = false;

            while (
                mainPageViewModel.GameState.IsOnlineGame &&
                mainPageViewModel.GameState.CurrentOnlineGame.BlackId != mainPageViewModel.GameState.LocalPlayer.Id &&
                ! isException
                )
            {
                Task.Run(() =>
                {
                    if (mainPageViewModel.GameState.LocalPlayer != null)
                    {
                        DispatchService.Invoke(async () =>
                        {
                            try
                            {
                                mainPageViewModel.GameState.CurrentOnlineGame
                                    = await WebApiClientGamesCommands.GetNewGame(mainPageViewModel.GameState.LocalPlayer.Id);
                            }
                            catch
                            {
                                isException = true;
                            }
                        });
                    }
                });
                Thread.Sleep(1000);
            }

            if (mainPageViewModel.GameState.IsOnlineGame)
            {
                DispatchService.Invoke(() =>
                {
                    MessageWaitingForInvitationAcceptionViewModel.OverlayWaitingForInvitationAcceptionIsVisible = false;
                    MessageOpponentAcceptedInvitationViewModel.OverlayOpponentAcceptedInvitationIsVisible = true;
                });
            }
        });
        var backgroundThread = new Thread(threadStart)
        {
            IsBackground = true
        };
        backgroundThread.Start();
    }
    internal static void LobbyKeepResettingInactiveCounter()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;

        var threadStart = new ThreadStart(() =>
        {
            bool isException = false;

            while (MessageLobbyViewModel.LobbyPageIsVisible && ! isException)
            {
                Task.Run(async () =>
                {
                    if (mainPageViewModel.GameState.LocalPlayer != null)
                    {
                        try
                        {
                            await WebApiClientPlayersCommands.ResetInactiveCounterAsync(mainPageViewModel.GameState.LocalPlayer.Id);
                        }
                        catch
                        {
                            isException = true;
                            MessageLobbyPageErrorMessageViewModel.Show(ErrorReason.LobbyPageConnectionToServerLost);
                        }
                    }
                });
                Thread.Sleep(1000);
            }
        });

        var backgroundThread = new Thread(threadStart)
        {
            IsBackground = true
        };
        backgroundThread.Start();
    }
    internal static void OnlineGameKeepResettingWhiteInactiveCounter()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        GameState gameState = mainPageViewModel.GameState;

        var threadStart = new ThreadStart(() =>
        {
            bool isException = false;

            while (
                gameState.IsOnlineGame &&
                ! isException)
            {
                Task.Run(async () =>
                {
                    if (gameState.CurrentOnlineGame != null)
                    {
                        try
                        {
                            await WebApiClientGamesCommands.ResetWhiteInactiveCounterAsync(gameState.CurrentOnlineGame.Id);
                        }
                        catch
                        {
                            isException = true;
                            MessageMainPageErrorMessageViewModel.Show(ErrorReason.MainPageConnectionToServerLost);
                        }
                    }
                });
                Thread.Sleep(1000);
            }
        });

        var backgroundThread = new Thread(threadStart)
        {
            IsBackground = true
        };
        backgroundThread.Start();
    }
    internal static void OnlineGameKeepResettingBlackInactiveCounter()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        GameState gameState = mainPageViewModel.GameState;

        var threadStart = new ThreadStart(() =>
        {
            bool isException = false;

            while (
                gameState.IsOnlineGame &&
                !isException)
            {
                Task.Run(async () =>
                {
                    if (gameState.CurrentOnlineGame != null)
                    {
                        try
                        {
                            await WebApiClientGamesCommands.ResetBlackInactiveCounterAsync(gameState.CurrentOnlineGame.Id);
                        }
                        catch
                        {
                            isException = true;
                            MessageMainPageErrorMessageViewModel.Show(ErrorReason.MainPageConnectionToServerLost);
                        }
                    }
                });
                Thread.Sleep(1000);
            }
        });

        var backgroundThread = new Thread(threadStart)
        {
            IsBackground = true
        };
        backgroundThread.Start();
    }
    internal static void OnlineGameKeepCheckingForNextMove()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        GameState gameState = mainPageViewModel.GameState;
        SquareDictionary squareDict = mainPageViewModel.SquareDict;
        gameState.IsWaitingForMove = true;

        ThreadStart threadStart = new(() =>
        {
            bool doRun = true;
            while (doRun && gameState.IsOnlineGame)
            {
                DispatchService.Invoke(async () =>
                {
                    if (gameState.CurrentOnlineGame != null)
                    {
                        bool isSuccess = false;
                        try
                        {
                            gameState.CurrentOnlineGame = await WebApiClientGamesCommands.GetCurrentGame(gameState.CurrentOnlineGame.Id);
                            isSuccess = true;
                        }
                        catch
                        {
                            ;
                        }

                        if (isSuccess)
                        {
                            if (gameState.CurrentOnlineGame.HasPlayerQuit)
                            {
                                doRun = false;
                                MessageOnlineGamePlayerQuitViewModel.OpponentName = gameState.Opponent.Name;
                                MessageOnlineGamePlayerQuitViewModel.OnlineGamePlayerQuitIsVisible = true;
                            }

                            else
                            {
                                string lastMoveStart;
                                string lastMoveEnd;
                                ChessPieceColor opponentColor;

                                lastMoveStart = gameState.CurrentOnlineGame.LastMoveStart;
                                lastMoveEnd = gameState.CurrentOnlineGame.LastMoveEnd;

                                if (gameState.LocalPlayer.Color == "White")
                                {
                                    opponentColor = ChessPieceColor.Black;
                                }
                                else
                                {
                                    opponentColor = ChessPieceColor.White;
                                }

                                if (lastMoveStart != null && lastMoveEnd != null)
                                {
                                    if (squareDict[lastMoveStart].ChessPiece.ChessPieceColor == opponentColor)
                                    {
                                        Coords oldCoords = Coords.StringToCoords(lastMoveStart);
                                        Coords newCoords = Coords.StringToCoords(lastMoveEnd);

                                        bool wasSquareNewCoordsEmpty = !squareDict[newCoords.String].IsOccupied;

                                        ChessPiece chessPiece = squareDict[lastMoveStart].ChessPiece;

                                        mainPageViewModel.MoveChessPiece(oldCoords, newCoords, true, true);
                                        gameState.MoveList.Add(new Move(oldCoords, newCoords, chessPiece.ChessPieceColor, chessPiece.ChessPieceType));

                                        // block input on check mate:
                                        if (gameState.CurrentOnlineGame.IsCheckMate)
                                        {
                                            gameState.IsCheckMate = true;
                                        }
                                        // if opponent promoted a pawn:
                                        else if (gameState.CurrentOnlineGame.PromotePawnType != ' ')
                                        {
                                            if (gameState.CurrentOnlineGame.PromotePawnType == 'B')
                                            {
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Bishop, gameState.IsRotated);
                                            }
                                            else if (gameState.CurrentOnlineGame.PromotePawnType == 'K')
                                            {
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Knight, gameState.IsRotated);
                                            }
                                            else if (gameState.CurrentOnlineGame.PromotePawnType == 'R')
                                            {
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Rook, gameState.IsRotated);
                                            }
                                            else if (gameState.CurrentOnlineGame.PromotePawnType == 'Q')
                                            {
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Queen, gameState.IsRotated);
                                            }

                                            mainPageViewModel.ImageDict[newCoords.String] = ChessPieceImages.GetChessPieceImage(chessPiece.ChessPieceColor, chessPiece.ChessPieceType);
                                            squareDict[newCoords.String].ChessPiece = chessPiece;
                                        }
                                        // if opponent's pawn moved two squares:
                                        else if (gameState.CurrentOnlineGame.PawnMovedTwoSquares)
                                        {
                                            squareDict.CoordsPawnMovedTwoSquares = newCoords;
                                        }
                                        // if opponent captured own pawn en passant:
                                        else if (chessPiece.ChessPieceType == ChessPieceType.Pawn)
                                        {
                                            bool canCapture = false;
                                            Coords capturePawnCoords = null;

                                            if (wasSquareNewCoordsEmpty)
                                            {
                                                // if white pawn moved:
                                                if (chessPiece.ChessPieceColor == ChessPieceColor.White)
                                                {
                                                    // if white pawn moved up left:
                                                    if (newCoords.X < oldCoords.X)
                                                    {
                                                        canCapture = true;
                                                        capturePawnCoords = new(oldCoords.X - 1, oldCoords.Y);
                                                    }
                                                    // if white pawn moved up right:
                                                    else if (newCoords.X > oldCoords.X)
                                                    {
                                                        canCapture = true;
                                                        capturePawnCoords = new(oldCoords.X + 1, oldCoords.Y);
                                                    }
                                                }
                                                else
                                                {
                                                    // if black pawn moved up left:
                                                    if (newCoords.X > oldCoords.X)
                                                    {
                                                        canCapture = true;
                                                        capturePawnCoords = new(oldCoords.X + 1, oldCoords.Y);
                                                    }
                                                    // if black pawn moved up right:
                                                    else if (newCoords.X < oldCoords.X)
                                                    {
                                                        canCapture = true;
                                                        capturePawnCoords = new(oldCoords.X - 1, oldCoords.Y);
                                                    }
                                                }
                                            }

                                            if (canCapture)
                                            {
                                                squareDict[capturePawnCoords.String].ChessPiece = new ChessPiece();
                                                squareDict[capturePawnCoords.String].IsOccupied = false;

                                                mainPageViewModel.ImageDict[capturePawnCoords.String] = ChessPieceImages.Empty;
                                            }
                                        }
                                        // if opponent castled his king:
                                        else if (chessPiece.ChessPieceType == ChessPieceType.King)
                                        {
                                            List<Coords> rookCoordsOldNew = new();

                                            // if opponent is white:
                                            if (chessPiece.ChessPieceColor == ChessPieceColor.White)
                                            {
                                                if (newCoords.X - oldCoords.X > 1)
                                                {
                                                    rookCoordsOldNew.Add(new Coords(Columns.H, 1));
                                                    rookCoordsOldNew.Add(new Coords(Columns.F, 1));
                                                }
                                                else if (oldCoords.X - newCoords.X > 1)
                                                {
                                                    rookCoordsOldNew.Add(new Coords(Columns.A, 1));
                                                    rookCoordsOldNew.Add(new Coords(Columns.D, 1));
                                                }
                                            }
                                            // if opponent is black:
                                            else
                                            {
                                                if (newCoords.X - oldCoords.X > 1)
                                                {
                                                    rookCoordsOldNew.Add(new Coords(Columns.H, 8));
                                                    rookCoordsOldNew.Add(new Coords(Columns.F, 8));
                                                }
                                                else if (oldCoords.X - newCoords.X > 1)
                                                {
                                                    rookCoordsOldNew.Add(new Coords(Columns.A, 8));
                                                    rookCoordsOldNew.Add(new Coords(Columns.D, 8));
                                                }
                                            }

                                            if (rookCoordsOldNew.Count > 0)
                                            {
                                                mainPageViewModel.MoveChessPiece(rookCoordsOldNew[0], rookCoordsOldNew[1], false, true);
                                            }
                                        }

                                        doRun = false;
                                        gameState.IsWaitingForMove = false;
                                        mainPageViewModel.LabelMoveInfo = gameState.CurrentOnlineGame.MoveInfo;

                                        gameState.CurrentOnlineGame.LastMoveStart = null;
                                        gameState.CurrentOnlineGame.LastMoveEnd = null;
                                    }
                                }
                            }
                        }
                    }
                });

                Thread.Sleep(500);
            }
        });

        var backgroundThread = new Thread(threadStart)
        {
            IsBackground = true
        };
        backgroundThread.Start();
    }
}
