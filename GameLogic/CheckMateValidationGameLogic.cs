using ChessAvalonia.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace ChessAvalonia.GameLogic
{
    internal static class CheckMateValidationGameLogic
    {
        internal static bool IsCheckMate(SquareDictionary squareDict, Coords kingCoords)
        {
            ChessPieceColor kingColor = squareDict[kingCoords.String].ChessPiece.ChessPieceColor;
            ChessPieceColor ownColor = kingColor == ChessPieceColor.White ? ChessPieceColor.Black : ChessPieceColor.White;
            List<Coords> threateningTiles = ThreateningValidationGameLogic.IsSquareThreatenedList(squareDict, kingColor, kingCoords);
            int threateningTilesNumber = threateningTiles.Count;

            if (threateningTilesNumber > 0)
            {
                // can the king escape the check mate without being in check again?
                List<Coords> coordsKingNeighbors = new List<Coords>()
                {                    
                    new Coords(kingCoords.X - 1, kingCoords.Y + 1), // top left
                    new Coords(kingCoords.X,     kingCoords.Y + 1), // top center
                    new Coords(kingCoords.X + 1, kingCoords.Y + 1), // top right
                    new Coords(kingCoords.X - 1, kingCoords.Y),     // left
                    new Coords(kingCoords.X + 1, kingCoords.Y),     // right
                    new Coords(kingCoords.X - 1, kingCoords.Y - 1), // bottom left
                    new Coords(kingCoords.X,     kingCoords.Y - 1), // bottom center
                    new Coords(kingCoords.X + 1, kingCoords.Y - 1)  // bottom right
                };

                for (int i = 0; i < coordsKingNeighbors.Count; i++)
                {
                    if (coordsKingNeighbors[i].X >= 1 && coordsKingNeighbors[i].X <= 8
                        && coordsKingNeighbors[i].Y >= 1 && coordsKingNeighbors[i].Y <= 8)
                    {
                        if (squareDict[coordsKingNeighbors[i].String].ChessPiece.ChessPieceColor != kingColor
                            && !CheckValidationGameLogic.IsCheck(squareDict, kingCoords, coordsKingNeighbors[i]))
                        {
                            return false;
                        }
                    }
                }

                // if there is no double check, test if the tiles between the king and the threatening chess piece
                // can be reached by a chess piece of the same color:
                if (threateningTilesNumber == 1)
                {
                    Square threateningSquare = squareDict[threateningTiles[0].String];
                    ChessPieceType threateningType = threateningSquare.ChessPiece.ChessPieceType;
                    //System.Diagnostics.Debug.WriteLine(threateningType.ToString());

                    // check if an opponent's queen is threatening own king:
                    if (threateningType == ChessPieceType.Queen)
                    {
                        if (CanBlockQueenAndRookHorizontally(squareDict, threateningSquare, kingCoords))
                        {
                            return false;
                        }
                        else if (CanBlockQueenAndRookVertically(squareDict, threateningSquare, kingCoords))
                        {
                            return false;
                        }
                        else if (CanBlockQueenAndBishopDiagonally(squareDict, threateningSquare, kingCoords))
                        {
                            return false;
                        }
                    }

                    // check if an opponent's rook is threatening own king:
                    else if (threateningType == ChessPieceType.Rook)
                    {
                        if (CanBlockQueenAndRookHorizontally(squareDict, threateningSquare, kingCoords))
                        {
                            return false;
                        }
                        if (CanBlockQueenAndRookVertically(squareDict, threateningSquare, kingCoords))
                        {
                            return false;
                        }
                    }

                    // check if an opponent's bishop is threatening own king:
                    else if (threateningSquare.ChessPiece.ChessPieceType == ChessPieceType.Bishop)
                    {
                        if (CanBlockQueenAndBishopDiagonally(squareDict, threateningSquare, kingCoords))
                        {
                            return false;
                        }
                    }

                    // check if threatening chess piece can be captured:
                    else if (ThreateningValidationGameLogic.IsSquareThreatened(squareDict, ownColor, threateningTiles[0], true))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
        internal static bool CanBlockQueenAndRookHorizontally(
            SquareDictionary squareDict, Square threateningTile, Coords kingCoords)
        {
            ChessPieceColor threateningColor = threateningTile.ChessPiece.ChessPieceColor;

            if (threateningTile.Coords.Y == kingCoords.Y)
            {
                if (threateningTile.Coords.X < kingCoords.X)
                {
                    for (int column = threateningTile.Coords.X + 1; column <= kingCoords.X - 1; column++)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
                else if (threateningTile.Coords.X > kingCoords.X)
                {
                    for (int column = threateningTile.Coords.X - 1; column >= kingCoords.X + 1; column--)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
                        {
                            return true;
                        }
                        //System.Diagnostics.Debug.WriteLine(coordsInBetween.ToString());
                    }
                }
            }

            return false;
        }
        internal static bool CanBlockQueenAndRookVertically(
            SquareDictionary squareDict, Square threateningTile, Coords kingCoords)
        {
            ChessPieceColor threateningColor = threateningTile.ChessPiece.ChessPieceColor;

            if (threateningTile.Coords.X == kingCoords.X)
            {
                if (threateningTile.Coords.Y < kingCoords.Y)
                {
                    for (int row = threateningTile.Coords.Y + 1; row <= kingCoords.Y - 1; row++)
                    {
                        Coords coordsInBetween = new Coords(threateningTile.Coords.X, row);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
                        {
                            System.Diagnostics.Debug.WriteLine(coordsInBetween.String);
                            return true;
                        }
                    }
                }
                else if (threateningTile.Coords.Y > kingCoords.Y)
                {
                    for (int row = threateningTile.Coords.Y - 1; row >= kingCoords.Y + 1; row--)
                    {
                        Coords coordsInBetween = new Coords(threateningTile.Coords.X, row);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
                        {
                            return true;
                        }
                        //System.Diagnostics.Debug.WriteLine(coordsInBetween.ToString());
                    }
                }
            }

            return false;
        }
        internal static bool CanBlockQueenAndBishopDiagonally(
            SquareDictionary squareDict, Square threateningTile, Coords kingCoords)
        {
            ChessPieceColor threateningColor = threateningTile.ChessPiece.ChessPieceColor;

            if (threateningTile.Coords.X < kingCoords.X)
            {
                if (threateningTile.Coords.Y < kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X + 1; column <= kingCoords.X - 1; column++)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y + (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine(coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
                else if (threateningTile.Coords.Y > kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X + 1; column <= kingCoords.X - 1; column++)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y - (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine(coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
            }

            else if (threateningTile.Coords.X > kingCoords.X)
            {
                if (threateningTile.Coords.Y < kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X - 1; column >= kingCoords.X + 1; column--)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y - (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine(coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
                else if (threateningTile.Coords.Y > kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X - 1; column >= kingCoords.X + 1; column--)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y + (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine("here" + coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachSquare(squareDict, threateningColor, coordsInBetween))
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
