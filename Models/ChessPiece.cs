using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.Models;
internal class ChessPiece
{
    public ChessPiece(ChessPieceColor chessPieceColor, ChessPieceType chessPieceType, bool isRotated)
    {
        ChessPieceColor = chessPieceColor;
        ChessPieceType = chessPieceType;
        IsRotated = isRotated;
        HasMoved = false;
        MoveCount = 0;
    }
    internal ChessPiece()
    {
        ChessPieceColor = ChessPieceColor.Empty;
        ChessPieceType = ChessPieceType.Empty;
        HasMoved = false;
        IsRotated = false;
        MoveCount = 0;
    }
    internal ChessPiece(ChessPiece chessPiece)
    {
        ChessPieceColor = chessPiece.ChessPieceColor;
        ChessPieceType = chessPiece.ChessPieceType;
        HasMoved = chessPiece.HasMoved;
        IsRotated = chessPiece.IsRotated;
        MoveCount = chessPiece.MoveCount;
    }
    internal ChessPieceColor ChessPieceColor { get; set; }
    internal ChessPieceType ChessPieceType { get; set; }
    internal bool IsRotated { get; set; }
    internal bool HasMoved { get; set; }
    internal int MoveCount { get; set; }
}
internal enum ChessPieceColor
{
    White,
    Black,
    Empty
}
internal enum ChessPieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King,
    Empty
}
