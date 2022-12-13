using System.Collections.Generic;
using ChessAvalonia.Models;

namespace ChessAvalonia.GameLogic
{
    internal static class ThreateningValidationGameLogic
    {
        internal static bool IsSquareThreatened(SquareDictionary squareDict, ChessPieceColor ownColor, Coords coordsToCheck, bool isCheckMateCheck)
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
                        if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                            && MoveValidationGameLogic.ValidatePawnThreatening(coords, coordsToCheck, chessPiece.ChessPieceColor))
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
                        if (chessPiece.ChessPieceType == ChessPieceType.King
                            && MoveValidationGameLogic.ValidateKing(coords, coordsToCheck)
                            && ! isCheckMateCheck)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        internal static List<Coords> IsSquareThreatenedList(SquareDictionary squareDict, ChessPieceColor ownColor, Coords coordsToCheck)
        {
            List<Coords> result = new List<Coords>();

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
                        if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                            && MoveValidationGameLogic.ValidatePawnThreatening(coords, coordsToCheck, chessPiece.ChessPieceColor))
                        {
                            result.Add(coords);
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Rook || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateRookAndQueenHorizontal(squareDict, coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                        if (chessPiece.ChessPieceType == ChessPieceType.Knight
                            && MoveValidationGameLogic.ValidateKnight(coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Bishop || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateBishopAndQueenDiagonal(squareDict, coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                        if (chessPiece.ChessPieceType == ChessPieceType.King
                            && MoveValidationGameLogic.ValidateKing(coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                    }
                }
            }
            return result;
        }
        internal static bool AreSquaresThreatened(SquareDictionary squareDict, ChessPieceColor ownColor, List<Coords> coordsListToCheck)
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
                        foreach (Coords coordsToCheck in coordsListToCheck)
                        {
                            if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                                && MoveValidationGameLogic.ValidatePawnThreatening(coords, coordsToCheck, chessPiece.ChessPieceColor))
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
                            if (chessPiece.ChessPieceType == ChessPieceType.King
                                && MoveValidationGameLogic.ValidateKing(coords, coordsToCheck))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
