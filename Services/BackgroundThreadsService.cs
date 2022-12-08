using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

using ChessAvalonia.Models;
using ChessAvalonia.ViewModels;
using ChessAvalonia.WebApiClient;
using ChessAvalonia.Views;
using static ChessAvalonia.Services.MessengerService;
using ChessAvalonia.ViewModels.Pages.Main;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;
using ChessAvalonia.ViewModels.Pages.Lobby;
using ChessAvalonia.Helpers;

namespace ChessAvalonia.Services;
internal static class BackgroundThreadsService
{
    internal static void LobbyKeepCheckingForOpponentAcception()
    {
        MainPageViewModel mainPageViewModel = MessageMainPageViewModel;
        MenuViewModel menuViewModel = MessageMenuViewModel;

        var threadStart = new ThreadStart(() =>
        {
            while (
                mainPageViewModel.GameState.IsOnlineGame &&
                mainPageViewModel.GameState.CurrentOnlineGame.BlackId != mainPageViewModel.GameState.LocalPlayer.Id
            )
            {
                Task.Run(() =>
                {
                    if (mainPageViewModel.GameState.LocalPlayer != null)
                    {
                        try
                        {
                            DispatchService.Invoke(async () =>
                            {
                                mainPageViewModel.GameState.CurrentOnlineGame
                                    = await WebApiClientGamesCommands.GetNewGame(mainPageViewModel.GameState.LocalPlayer.Id);                                 
                            });
                        }
                        catch
                        {
                            ;
                        }
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
        MainPageViewModel mainWindowViewModel = MessageMainPageViewModel;

        var threadStart = new ThreadStart(() =>
        {
            while (MessageLobbyViewModel.LobbyPageIsVisible)
            {
                Task.Run(async () =>
                {
                    if (mainWindowViewModel.GameState.LocalPlayer != null)
                    {
                        try
                        {
                            await WebApiClientPlayersCommands.ResetInactiveCounterAsync(mainWindowViewModel.GameState.LocalPlayer.Id);
                        }
                        catch
                        {
                            // MessageBox.Show(mainPageViewModel.LobbyWindow, "Cannot contact server...", "Error!");
                            mainWindowViewModel.LobbyPage.IsVisible = false;
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
        MainPageViewModel mainWindowViewModel = MessageMainPageViewModel;
        GameState gameState = mainWindowViewModel.GameState;

        var threadStart = new ThreadStart(() =>
        {
            while (gameState.IsOnlineGame)
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
                            ;
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
        MainPageViewModel mainWindowViewModel = MessageMainPageViewModel;
        GameState gameState = mainWindowViewModel.GameState;

        var threadStart = new ThreadStart(() =>
        {
            while (gameState.IsOnlineGame)
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
                            ;
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

                                if (gameState.LocalPlayer.Color == "White")
                                {
                                    lastMoveStart = gameState.CurrentOnlineGame.LastMoveStartBlack;
                                    lastMoveEnd = gameState.CurrentOnlineGame.LastMoveEndBlack;
                                    opponentColor = ChessPieceColor.Black;
                                }
                                else
                                {
                                    lastMoveStart = gameState.CurrentOnlineGame.LastMoveStartWhite;
                                    lastMoveEnd = gameState.CurrentOnlineGame.LastMoveEndWhite;
                                    opponentColor = ChessPieceColor.White;
                                }


                                if (lastMoveStart != null && lastMoveEnd != null)
                                {
                                    ChessPiece chessPiece = squareDict[lastMoveStart[..2]].ChessPiece;
                                    Coords oldCoords = Coords.StringToCoords(lastMoveStart[..2]);
                                    Coords newCoords = Coords.StringToCoords(lastMoveEnd[..2]);

                                    mainPageViewModel.MoveChessPiece(oldCoords, newCoords, true, true);
                                    gameState.MoveList.Add(new Move(oldCoords, newCoords, chessPiece.ChessPieceColor, chessPiece.ChessPieceType));

                                    if (lastMoveStart.Length > 2)
                                    {
                                        if (lastMoveStart[2] == 'C')
                                        {
                                            Coords rookOldCoords = Coords.StringToCoords(lastMoveStart.Substring(3, 2));
                                            Coords rookNewCoords = Coords.StringToCoords(lastMoveEnd.Substring(3, 2));

                                            mainPageViewModel.MoveChessPiece(rookOldCoords, rookNewCoords, true, true);
                                        }
                                        else if (lastMoveStart[2] == 'T')
                                        {
                                            squareDict.CoordsPawnMovedTwoSquares = Coords.StringToCoords(lastMoveStart.Substring(3, 2));
                                        }
                                        else if (lastMoveStart[2] == 'E')
                                        {
                                            Coords capturedCoords = Coords.StringToCoords(lastMoveStart.Substring(3, 2));
                                            squareDict[capturedCoords.String].ChessPiece = new ChessPiece();
                                            squareDict[capturedCoords.String].IsOccupied = false;

                                            mainPageViewModel.ImageDict[capturedCoords.String] = ChessPieceImages.Empty;
                                        }
                                        else if (lastMoveStart[2] == 'P')
                                        {
                                            string type = lastMoveStart.Remove(0, 3);
                                            if (type == "Bishop")
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Bishop, gameState.IsRotated);
                                            else if (type == "Knight")
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Knight, gameState.IsRotated);
                                            else if (type == "Rook")
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Rook, gameState.IsRotated);
                                            else if (type == "Queen")
                                                chessPiece = new ChessPiece(opponentColor, ChessPieceType.Queen, gameState.IsRotated);


                                            squareDict[lastMoveEnd[..2]].ChessPiece = chessPiece;
                                            mainPageViewModel.ImageDict[lastMoveEnd[..2]] = ChessPieceImages.GetChessPieceImage(chessPiece.ChessPieceColor, chessPiece.ChessPieceType);
                                        }
                                    }

                                    doRun = false;
                                    gameState.IsWaitingForMove = false;
                                    mainPageViewModel.LabelMoveInfo = gameState.CurrentOnlineGame.MoveInfo;
                                }
                            }
                        }
                    }
                });

                Thread.Sleep(100);
            }
        });

        var backgroundThread = new Thread(threadStart)
        {
            IsBackground = true
        };
        backgroundThread.Start();
    }
}
