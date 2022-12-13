using System.Collections.Generic;
using ChessAvalonia.Models;

namespace ChessAvalonia.GameLogic;
internal static class CastlingValidationGameLogic
{
    internal static MoveValidationData CanCastle(SquareDictionary squareDict, Coords oldCoords, Coords newCoords)
    {
        // rules for castling:
        // 1. king must not have moved ### done
        // 2. rook must not have moved ### done
        // 3. there must not be any pieces between both ### done
        // 4. the king's old and new position and the square that the king is passing may not be threatened ### done

        MoveValidationData moveValidationData = new MoveValidationData();

        // rule 1: has king moved?
        if (squareDict[oldCoords.String].ChessPiece.HasMoved)
        {
            return moveValidationData;
        }

        // check row 1:
        if (oldCoords.Y == 1)
        {
            // king wants to move left:
            if (newCoords.X == 3)
            {
                // return the coords of the associated rook:
                moveValidationData.Coords.Add(new Coords(1, 1));
                moveValidationData.Coords.Add(new Coords(4, 1));

                // rule 2: has the associated rook moved?
                if (squareDict[Coords.IntsToCoordsString(1, 1)].ChessPiece.HasMoved)
                {
                    return moveValidationData;
                }
                // rule 3: is there a chess piece in between?
                if    (squareDict[Coords.IntsToCoordsString(2, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                    || squareDict[Coords.IntsToCoordsString(3, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                    || squareDict[Coords.IntsToCoordsString(4, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                {
                    return moveValidationData;
                }
                // rule 4: is the king (old or new coords) or the crossed square threatened?
                List<Coords> coordsListToCheck = new List<Coords>
                {
                    oldCoords,
                    newCoords,
                    new Coords(4, 1)
                };
                if (ThreateningValidationGameLogic.AreSquaresThreatened(
                    squareDict,
                    squareDict[oldCoords.String].ChessPiece.ChessPieceColor,
                    coordsListToCheck))
                {
                    return moveValidationData;
                }
            }
            // king wants to move right:
            else if (newCoords.X == 7)
            {
                // return the coords of the associated rook:
                moveValidationData.Coords.Add(new Coords(8, 1));
                moveValidationData.Coords.Add(new Coords(6, 1));

                // rule 2: has the associated rook moved?
                if (squareDict[Coords.IntsToCoordsString(8, 1)].ChessPiece.HasMoved)
                {
                    return moveValidationData;
                }
                // rule 3: is there a chess piece in between?
                if (squareDict[Coords.IntsToCoordsString(6, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                    || squareDict[Coords.IntsToCoordsString(7, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                {
                    return moveValidationData;
                }
                // rule 4: is the king (old or new coords) or the crossed square threatened?
                List<Coords> coordsListToCheck = new List<Coords>
                {
                    oldCoords,
                    newCoords,
                    new Coords(6, 1)
                };
                if (ThreateningValidationGameLogic.AreSquaresThreatened(
                    squareDict,
                    squareDict[oldCoords.String].ChessPiece.ChessPieceColor,
                    coordsListToCheck))
                {
                    return moveValidationData;
                }
            }
            else
            {
                return moveValidationData;
            }
        }
        // check row 8:
        else if (oldCoords.Y == 8)
        {
            // king wants to move left:
            if (newCoords.X == 3)
            {
                // return the coords of the associated rook:
                moveValidationData.Coords.Add(new Coords(1, 8));
                moveValidationData.Coords.Add(new Coords(4, 8));

                // rule 2: has the associated rook moved?
                if (squareDict[Coords.IntsToCoordsString(1, 8)].ChessPiece.HasMoved)
                {
                    return moveValidationData;
                }
                // rule 3: is there a chess piece in between?
                if (squareDict[Coords.IntsToCoordsString(2, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                    || squareDict[Coords.IntsToCoordsString(3, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                    || squareDict[Coords.IntsToCoordsString(4, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                {
                    return moveValidationData;
                }
                // is the king or the crossed square threatened?
                List<Coords> coordsListToCheck = new List<Coords>
                {
                    oldCoords,
                    newCoords,
                    new Coords(4, 8)
                };
                if (ThreateningValidationGameLogic.AreSquaresThreatened(
                    squareDict,
                    squareDict[oldCoords.String].ChessPiece.ChessPieceColor,
                    coordsListToCheck))
                {
                    return moveValidationData;
                }
            }
            // king wants to move right:
            else if (newCoords.X == 7)
            {
                // return the coords of the associated rook:
                moveValidationData.Coords.Add(new Coords(8, 8));
                moveValidationData.Coords.Add(new Coords(6, 8));

                // has the associated rook moved?
                if (squareDict[Coords.IntsToCoordsString(8, 8)].ChessPiece.HasMoved)
                {
                    return moveValidationData;
                }
                // rule 3: is there a chess piece in between?
                if (squareDict[Coords.IntsToCoordsString(6, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                    || squareDict[Coords.IntsToCoordsString(7, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                {
                    return moveValidationData;
                }
                // is the king (old or new coords) or the crossed square threatened?
                List<Coords> coordsListToCheck = new List<Coords>
                {
                    oldCoords,
                    newCoords,
                    new Coords(6, 8)
                };
                if (ThreateningValidationGameLogic.AreSquaresThreatened(
                    squareDict,
                    squareDict[oldCoords.String].ChessPiece.ChessPieceColor,
                    coordsListToCheck))
                {
                    return moveValidationData;
                }
            }
            else
            {
                return moveValidationData;
            }
        }
        // if not in row 1 or 8, return false:
        else
        {
            return moveValidationData;
        }
        moveValidationData.IsValid = true;
        moveValidationData.CanCastle = true;
        return moveValidationData;
    }
}
