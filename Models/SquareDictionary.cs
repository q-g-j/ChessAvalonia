using System.Collections.Generic;

namespace ChessAvalonia.Models;
public class SquareDictionary : Dictionary<string, Square>
{
    #region Constructors
    internal SquareDictionary()
    {
        for (int col = 1; col < 9; col++)
        {
            for (int row = 8; row > 0; row--)
            {
                Coords coords = new(col, row);
                this[coords.String] = new Square(col, row, false, new ChessPiece(ChessPieceColor.Empty, ChessPieceType.Empty, false));
            }
        }

        WhiteKingCoords = new Coords(Columns.E, 1);
        BlackKingCoords = new Coords(Columns.E, 8);
    }
    #endregion Constructors

    #region Properties
    internal Coords CoordsPawnMovedTwoSquares { get; set; }
    internal Coords WhiteKingCoords { get; set; }
    internal Coords BlackKingCoords { get; set; }
    #endregion Properties

    #region Methods
    #endregion Methods
}
