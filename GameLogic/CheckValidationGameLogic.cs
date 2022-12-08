using ChessAvalonia.Models;
using static ChessAvalonia.Services.MessengerService;
using ChessAvalonia.ViewModels.Pages.Main;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.GameLogic
{
    internal static class CheckValidationGameLogic
    {
        internal static bool IsCheck(SquareDictionary squareDict, Coords oldCoords, Coords newCoords)
        {
            MainPageViewModel mainWindowViewModel = MessageMainPageViewModel;
            ChessPieceColor kingColor = squareDict[oldCoords.String].ChessPiece.ChessPieceColor;
            Coords kingCoords;
            bool isCheck = false;

            ChessPiece chessPieceOnNewCoordsBackup = new ChessPiece(squareDict[newCoords.String].ChessPiece);
            
            mainWindowViewModel.MoveChessPiece(oldCoords, newCoords, false, false);

            if (kingColor == ChessPieceColor.White)
            {
                kingCoords = squareDict.WhiteKingCoords;
            }
            else
            {
                kingCoords = squareDict.BlackKingCoords;
            }

            if (ThreateningValidationGameLogic.IsSquareThreatened(
                squareDict,
                squareDict[newCoords.String].ChessPiece.ChessPieceColor,
                kingCoords,
                false))
            {
                isCheck = true;
            }

            mainWindowViewModel.MoveChessPiece(newCoords, oldCoords, false, false);
                
            squareDict[newCoords.String].ChessPiece = chessPieceOnNewCoordsBackup;


            return isCheck;
        }
    }
}
