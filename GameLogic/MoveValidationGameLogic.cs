using System;
using System.Collections.Generic;
using ChessAvalonia.Models;
using static ChessAvalonia.Models.Coords;

namespace ChessAvalonia.GameLogic
{
    internal class MoveValidationData
    {
        internal bool IsValid { get; set; }
        internal bool CanCastle { get; set; }
        internal bool CanCaptureEnPassant { get; set; }
        internal bool MovedTwoSquares { get; set; }
        internal List<Coords> Coords { get; set; }

        internal MoveValidationData()
        {
            Coords = new List<Coords>();
            IsValid = false;
            CanCastle = false;
        }
    }
    internal static class MoveValidationGameLogic
    {
        internal static MoveValidationData ValidateCurrentMove(SquareDictionary squareDict, Coords oldCoords, Coords newCoords)
        {
            MoveValidationData moveValidationData = new MoveValidationData();

            ChessPieceColor oldCoordsColor = squareDict[oldCoords.String].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = squareDict[newCoords.String].ChessPiece.ChessPieceColor;

            // dont't allow to capture a piece of the same color:
            if (oldCoordsColor == newCoordsColor) return moveValidationData;

            // would own king be threatened:
            if (CheckValidationGameLogic.IsCheck(squareDict, oldCoords, newCoords))
            {
                return moveValidationData;
            }

            // validate pawn's move:
            if (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn)
            {
                moveValidationData = ValidatePawn(
                    squareDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor, false);
            }
            // validate bishop's move:
            else if (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Bishop)
            {
                moveValidationData.IsValid = ValidateBishopAndQueenDiagonal(
                    squareDict, oldCoords, newCoords);
            }
            // validate queen's move:
            else if (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Queen)
            {
                bool isValidStraight = ValidateRookAndQueenHorizontal(squareDict, oldCoords, newCoords);
                bool isValidDiagonal = ValidateBishopAndQueenDiagonal(squareDict, oldCoords, newCoords);

                moveValidationData.IsValid = isValidStraight || isValidDiagonal;
            }
            // validate king's move:
            else if (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.King)
            {
                moveValidationData = CastlingValidationGameLogic.CanCastle(squareDict, oldCoords, newCoords);
                if (! moveValidationData.CanCastle)
                {
                    moveValidationData.IsValid = ValidateKing(oldCoords, newCoords);
                }
                return moveValidationData;
            }
            // validate rook's move:
            else if (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Rook)
            {
                moveValidationData.IsValid = ValidateRookAndQueenHorizontal(
                    squareDict, oldCoords, newCoords);
            }
            // validate knight's move:
            else if (squareDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Knight)
            {
                moveValidationData.IsValid = ValidateKnight(oldCoords, newCoords);
            }

            return moveValidationData;
        }
        internal static MoveValidationData ValidatePawn(
            SquareDictionary squareDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor,
            bool onlyCheckMove)
        {
            // initialize return type:
            MoveValidationData moveValidationData = new MoveValidationData();

            // has a pawn move two squares in the last turn:
            if (squareDict.CoordsPawnMovedTwoSquares != null)
            {
                // validate if it can be captured:
                if (EnPassantValidationGameLogic.CanCaptureEnPassant(squareDict, oldCoords, newCoords))
                {
                    moveValidationData.IsValid = true;
                    moveValidationData.CanCaptureEnPassant = true;
                    return moveValidationData;
                }
            }

            // don't allow to move along the same row:
            if (oldCoords.Y == newCoords.Y) return moveValidationData;

            // validate white pawns:
            if (oldCoordsColor == ChessPieceColor.White)
            {
                // don't allow to move backwards:
                if (oldCoords.Y > newCoords.Y) return moveValidationData;
                // if it's the pawn's first move:
                if (oldCoords.Y == 2)
                {
                    if (newCoords.Y - 2 == oldCoords.Y)
                    {
                        moveValidationData.MovedTwoSquares = true;
                        moveValidationData.Coords.Add(newCoords);
                        squareDict.CoordsPawnMovedTwoSquares = null;
                    }

                    // don't allow to move forward more than 2 squares, 
                    if (newCoords.X == oldCoords.X && newCoords.Y - 2 > oldCoords.Y) return moveValidationData;
                    // don't allow to jump over another piece:
                    if (newCoords.Y == oldCoords.Y + 2
                        && squareDict[IntsToCoordsString(oldCoords.X, oldCoords.Y + 1)].IsOccupied) return moveValidationData;
                }
                // don't allow to move forward more than 1 square in any following move:
                if (oldCoords.Y != 2 && newCoords.X == oldCoords.X && newCoords.Y - 1 > oldCoords.Y) return moveValidationData;
                // only allow to capture an ememie's piece, if it's 1 diagonal square away:
                else if (newCoordsColor != ChessPieceColor.Empty && !onlyCheckMove)
                {
                    if (newCoords.Y > oldCoords.Y + 1) return moveValidationData;
                    if (oldCoords.X == newCoords.X) return moveValidationData;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return moveValidationData;
                }
                // don't allow to move other than vertically:
                else if (oldCoords.X != newCoords.X) return moveValidationData;
            }
            // validate black pawns:
            else
            {
                // don't allow to move backwards:
                if (oldCoords.Y < newCoords.Y) return moveValidationData;
                // if it's the pawn's first move:
                if (oldCoords.Y == 7)
                {
                    if (newCoords.Y + 2 == oldCoords.Y)
                    {
                        moveValidationData.MovedTwoSquares = true;
                        moveValidationData.Coords.Add(newCoords);
                        squareDict.CoordsPawnMovedTwoSquares = null;
                    }

                    // don't allow to move forward more than 2 squares, 
                    if (newCoords.X == oldCoords.X && newCoords.Y + 2 < oldCoords.Y) return moveValidationData;
                    // don't allow to jump over another piece:
                    if (newCoords.Y == oldCoords.Y - 2
                        && squareDict[IntsToCoordsString(oldCoords.X, oldCoords.Y - 1)].IsOccupied) return moveValidationData;
                }
                // don't allow to move forward more than 1 square in any following move:
                if (oldCoords.Y != 7 && newCoords.X == oldCoords.X && newCoords.Y + 1 < oldCoords.Y) return moveValidationData;
                // only allow to capture an ememie's piece, if it's 1 diagonal square away:
                else if (newCoordsColor != ChessPieceColor.Empty && !onlyCheckMove)
                {
                    if (newCoords.Y < oldCoords.Y - 1) return moveValidationData;
                    if (oldCoords.X == newCoords.X) return moveValidationData;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return moveValidationData;
                }
                // don't allow to move other than vertically:
                else if (oldCoords.X != newCoords.X) return moveValidationData;
            }

            moveValidationData.IsValid = true;
            return moveValidationData;
        }
        internal static bool ValidatePawnThreatening(
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor)
        {
            // validate white pawns:
            if (oldCoordsColor == ChessPieceColor.White)
            {
                if (newCoords.Y == oldCoords.Y + 1 && (newCoords.X == oldCoords.X + 1 || newCoords.X == oldCoords.X - 1))
                    return true;
            }
            // validate black pawns:
            else
            {
                if (newCoords.Y == oldCoords.Y - 1 && (newCoords.X == oldCoords.X + 1 || newCoords.X == oldCoords.X - 1))
                    return true;
            }

            return false;
        }
        internal static bool ValidateRookAndQueenHorizontal(
            SquareDictionary squareDict,
            Coords oldCoords,
            Coords newCoords)
        {
            // don't allow to move diagonally:
            if (oldCoords.X != newCoords.X && oldCoords.Y != newCoords.Y) return false;

            // check if the path towards top is free:
            if (newCoords.X == oldCoords.X && newCoords.Y > oldCoords.Y)
            {
                for (int i = oldCoords.Y + 1; i < newCoords.Y; i++)
                {
                    if (squareDict[IntsToCoordsString(oldCoords.X, i)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            // check if the path towards bottom is free:
            if (newCoords.X == oldCoords.X && newCoords.Y < oldCoords.Y)
            {
                for (int i = oldCoords.Y - 1; i > newCoords.Y; i--)
                {
                    if (squareDict[IntsToCoordsString(oldCoords.X, i)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            // check if the path towards right is free:
            if (newCoords.X > oldCoords.X && newCoords.Y == oldCoords.Y)
            {
                for (int i = oldCoords.X + 1; i < newCoords.X; i++)
                {
                    if (squareDict[IntsToCoordsString(i, oldCoords.Y)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            // check if the path towards left is free:
            if (newCoords.X < oldCoords.X && newCoords.Y == oldCoords.Y)
            {
                for (int i = oldCoords.X - 1; i > newCoords.X; i--)
                {
                    if (squareDict[IntsToCoordsString(i, oldCoords.Y)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            return true;
        }
        internal static bool ValidateBishopAndQueenDiagonal(
            SquareDictionary squareDict,
            Coords oldCoords,
            Coords newCoords)
        {
            if (oldCoords.X == newCoords.X || oldCoords.Y == newCoords.Y)
            {                
                return false;
            }

            // don't allow to move horizontally:
            if (newCoords.X == oldCoords.X || newCoords.Y == oldCoords.Y) return false;

            // only allow to move in a straight line:
            if (Math.Abs(newCoords.X - oldCoords.X) != Math.Abs(newCoords.Y - oldCoords.Y)) return false;

            // check if the path towards top right is free:
            if (newCoords.X > oldCoords.X && newCoords.Y > oldCoords.Y)
            {
                for (int i = oldCoords.X + 1, j = oldCoords.Y + 1; i < newCoords.X && j < newCoords.Y; i++, j++)
                {
                    if (squareDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards top left is free:
            else if (newCoords.X < oldCoords.X && newCoords.Y > oldCoords.Y)
            {
                for (int i = oldCoords.X - 1, j = oldCoords.Y + 1; i > newCoords.X && j < newCoords.Y; i--, j++)
                {
                    if (squareDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards bottom right is free:
            else if (newCoords.X > oldCoords.X && newCoords.Y < oldCoords.Y)
            {
                for (int i = oldCoords.X + 1, j = oldCoords.Y - 1; i < newCoords.X && j > newCoords.Y; i++, j--)
                {
                    if (squareDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards bottom left is free:
            else if (newCoords.X < oldCoords.X && newCoords.Y < oldCoords.Y)
            {
                for (int i = oldCoords.X - 1, j = oldCoords.Y - 1; i > newCoords.X && j > newCoords.Y; i--, j--)
                {
                    if (squareDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            return true;
        }
        internal static bool ValidateKnight(
            Coords oldCoords,
            Coords newCoords)
        {
            // check each possible move:
            if (
                   !(oldCoords.Y - newCoords.Y == -2 && oldCoords.X - newCoords.X == +1) // -2 +1
                && !(oldCoords.Y - newCoords.Y == -2 && oldCoords.X - newCoords.X == -1) // -2 -1
                && !(oldCoords.Y - newCoords.Y == +2 && oldCoords.X - newCoords.X == +1) // +2 +1
                && !(oldCoords.Y - newCoords.Y == +2 && oldCoords.X - newCoords.X == -1) // +2 -1
                && !(oldCoords.Y - newCoords.Y == -1 && newCoords.X - oldCoords.X == +2) // -1 +2
                && !(oldCoords.Y - newCoords.Y == -1 && newCoords.X - oldCoords.X == -2) // -1 -2
                && !(oldCoords.Y - newCoords.Y == +1 && newCoords.X - oldCoords.X == +2) // +1 +2
                && !(oldCoords.Y - newCoords.Y == +1 && newCoords.X - oldCoords.X == -2) // +1 -2
                )
            {
                return false;
            }

            return true;
        }
        internal static bool ValidateKing(
            Coords oldCoords,
            Coords newCoords)
        {
            // don't allow to move farther than 1 square:
            if (newCoords.X > oldCoords.X + 1
                || newCoords.X < oldCoords.X - 1
                || newCoords.Y > oldCoords.Y + 1
                || newCoords.Y < oldCoords.Y - 1) return false;

            return true;
        }
        internal static bool CanReachSquare(SquareDictionary squareDict, ChessPieceColor ownColor, Coords coordsToCheck)
        {
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    Coords coords = new Coords(i, j);
                    Square square = squareDict[coords.String];
                    ChessPiece chessPiece = square.ChessPiece;
                    if (chessPiece.ChessPieceColor != ChessPieceColor.Empty
                        && chessPiece.ChessPieceColor != ownColor)
                    {
                        MoveValidationData moveValidationData = MoveValidationGameLogic.ValidatePawn(squareDict, coords, coordsToCheck, chessPiece.ChessPieceColor, ownColor, true);
                        if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                            && moveValidationData.IsValid)
                        {
                            return true;
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Rook || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateRookAndQueenHorizontal(squareDict, coords, coordsToCheck))
                        {
                            return true;
                        }
                        if (chessPiece.ChessPieceType == ChessPieceType.Knight
                            && MoveValidationGameLogic.ValidateKnight(coords, coordsToCheck))
                        {
                            return true;
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Bishop || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateBishopAndQueenDiagonal(squareDict, coords, coordsToCheck))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
