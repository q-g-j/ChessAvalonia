using ChessAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.Services
{
    internal class ChessPieceMoveService
    {
        internal void Move(SquareDictionary squareDict, ImageDictionary imageDict, Coords oldCoords, Coords newCoords, bool doChangeCounter, bool doChangeImage)
        {
            squareDict[newCoords.String].ChessPiece = squareDict[oldCoords.String].ChessPiece;
            squareDict[oldCoords.String].ChessPiece = new ChessPiece();

            if (doChangeImage)
            {
                imageDict[newCoords.String] = imageDict[oldCoords.String];
                imageDict[oldCoords.String] = ChessPieceImages.Empty;
            }

            if (doChangeCounter)
            {
                squareDict[oldCoords.String].IsOccupied = false;
                squareDict[newCoords.String].IsOccupied = true;

                squareDict[newCoords.String].ChessPiece.MoveCount++;
                squareDict[newCoords.String].ChessPiece.HasMoved = true;
            }

            if (squareDict[newCoords.String].ChessPiece.ChessPieceType == ChessPieceType.King)
            {
                if (squareDict[newCoords.String].ChessPiece.ChessPieceColor == ChessPieceColor.White)
                {
                    squareDict.WhiteKingCoords = newCoords;
                }
                else
                {
                    squareDict.BlackKingCoords = newCoords;
                }
            }
        }
    }
}
