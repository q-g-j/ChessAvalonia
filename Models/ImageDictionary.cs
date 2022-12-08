namespace ChessAvalonia.Models;

public class ImageDictionary : ObservableDictionary<string, string>
{
    public ImageDictionary()
    {        
        for (int col = 1; col < 9; col++)
        {
            for (int row = 8; row > 0; row--)
            {
                Coords coords = new(col, row);
                this[coords.String] = ChessPieceImages.Empty;
            }
        }
    }
}