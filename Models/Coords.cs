using System;
using Avalonia;

namespace ChessAvalonia.Models;
internal class Coords
{
    public Coords(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Coords(Columns col, int y)
    {
        X = (int)col;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
    public string String
    {
        get
        {
            return Enum.GetName(typeof(Columns), X)?.ToString() + Y.ToString();
        }
    }

    internal static string IntsToCoordsString(int col, int row)
    {
        return Enum.GetName(typeof(Columns), col)?.ToString() + row.ToString();
    }
    internal static Coords StringToCoords(string coordsString)
    {
        int colChar = coordsString[0];
        Columns column = (Columns)(colChar - 64);
        int row = int.Parse(coordsString[1].ToString());

        return new Coords(column, row);
    }
    internal static Coords CanvasPositionToCoords(Point point)
    {
        int col = (int)((point.X - point.X % 60) / 60) + 1;
        int row = (int)((point.Y - point.Y % 60) / 60) + 1;

        if (row == 1) row = 8;
        else if (row == 2) row = 7;
        else if (row == 3) row = 6;
        else if (row == 4) row = 5;
        else if (row == 5) row = 4;
        else if (row == 6) row = 3;
        else if (row == 7) row = 2;
        else if (row == 8) row = 1;

        return new Coords(col, row);
    }

    public override string ToString()
    {
        return String;
    }

    public static string IndexToCoordsString(int index)
    {
        index++;

        int col;
        int row;

        col = index % 8;
        if (col == 0) col = 8;

        if (index < 9) row = 8;
        else if (index < 17) row = 7;
        else if (index < 25) row = 6;
        else if (index < 33) row = 5;
        else if (index < 41) row = 4;
        else if (index < 49) row = 3;
        else if (index < 57) row = 2;
        else row = 1;

        return IntsToCoordsString(col, row);
    }

    public static int CoordsStringToIndex(string coordsString)
    {
        Coords coords = StringToCoords(coordsString);

        int row = 0;

        if (coords.Y == 8) row = 0;
        else if (coords.Y == 7) row = 1;
        else if (coords.Y == 6) row = 2;
        else if (coords.Y == 5) row = 3;
        else if (coords.Y == 4) row = 4;
        else if (coords.Y == 3) row = 5;
        else if (coords.Y == 2) row = 6;
        else if (coords.Y == 1) row = 7;

        return row * 8 + coords.X - 1;
    }
}

internal enum Columns
{
    A = 1, B = 2, C = 3, D = 4, E = 5, F = 6, G = 7, H = 8
}
