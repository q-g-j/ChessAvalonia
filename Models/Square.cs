using System;

namespace ChessAvalonia.Models;
public class Square
{
    internal Square(int col, int row, bool occupied, ChessPiece chessPiece)
    {
        Coords = new(col, row);
        IsOccupied = occupied;
        ChessPiece = chessPiece;
    }
    internal Coords Coords { get; set; }
    internal bool IsOccupied { get; set; }
    internal ChessPiece ChessPiece { get; set; }
}
