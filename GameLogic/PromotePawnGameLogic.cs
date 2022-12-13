using ChessAvalonia.Models;

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
