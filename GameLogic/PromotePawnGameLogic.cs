using ChessAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.GameLogic
{
    internal static class PromotePawnGameLogic
    {
        internal static bool CanPromote(SquareDictionary squareDict, Coords oldCoords, Coords newCoords)
        {
            if ((squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn && newCoords.Y == 8)
                || (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn && newCoords.Y == 1))
            {
                return true;
            }

            return false;
        }
    }
}
