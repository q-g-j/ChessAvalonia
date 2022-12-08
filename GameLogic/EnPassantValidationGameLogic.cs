using ChessAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.GameLogic
{
    internal static class EnPassantValidationGameLogic
    {
        internal static bool CanCaptureEnPassant(SquareDictionary squareDict, Coords oldCoords, Coords newCoords)
        {
            ChessPieceColor oldCoordsColor = squareDict[oldCoords.String].ChessPiece.ChessPieceColor;
            ChessPieceColor pawnMovedTwoTilesColor = squareDict[squareDict.CoordsPawnMovedTwoSquares.String].ChessPiece.ChessPieceColor;

            if (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn
                && oldCoordsColor != pawnMovedTwoTilesColor
                && squareDict[oldCoords.String].Coords.Y == squareDict.CoordsPawnMovedTwoSquares.Y
                && squareDict.CoordsPawnMovedTwoSquares.X == newCoords.X
                && (oldCoordsColor == ChessPieceColor.White && squareDict[newCoords.String].Coords.Y - squareDict.CoordsPawnMovedTwoSquares.Y == 1
                || oldCoordsColor == ChessPieceColor.Black && squareDict[newCoords.String].Coords.Y - squareDict.CoordsPawnMovedTwoSquares.Y == -1))
            {
                return true;
            }

            return false;
        }
    }
}
