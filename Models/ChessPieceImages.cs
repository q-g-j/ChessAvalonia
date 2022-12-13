namespace ChessAvalonia.Models;
internal class ChessPieceImages
{
    static ChessPieceImages()
    {
        Empty       = "Images/empty.png";

        WhitePawn   = "Images/white_pawn.png";
        WhiteRook   = "Images/white_rook.png";
        WhiteKnight = "Images/white_knight.png";
        WhiteBishop = "Images/white_bishop.png";
        WhiteQueen  = "Images/white_queen.png";
        WhiteKing   = "Images/white_king.png";

        BlackPawn   = "Images/black_pawn.png";
        BlackRook   = "Images/black_rook.png";
        BlackKnight = "Images/black_knight.png";
        BlackBishop = "Images/black_bishop.png";
        BlackQueen  = "Images/black_queen.png";
        BlackKing   = "Images/black_king.png";
    }

    public static string Empty;
    
    public static string WhitePawn;
    public static string WhiteRook;
    public static string WhiteKnight;
    public static string WhiteBishop;
    public static string WhiteQueen;
    public static string WhiteKing;

    public static string BlackPawn;
    public static string BlackRook;
    public static string BlackKnight;
    public static string BlackBishop;
    public static string BlackQueen;
    public static string BlackKing;

    internal static ChessPieceColor GetImageColor(string image)
    {
        if (image != null)
        {
            if (image.Contains("white")) return ChessPieceColor.White;
            else if (image.Contains("black")) return ChessPieceColor.Black;
            return ChessPieceColor.Empty;
        }
        
        return ChessPieceColor.Empty;
    }

    internal static string GetChessPieceImage(ChessPieceColor chessPieceColor, ChessPieceType chessPieceType)
    {
        if (chessPieceColor == ChessPieceColor.White)
        {
            if (chessPieceType == ChessPieceType.Pawn)
            {
                return ChessPieceImages.WhitePawn;
            }
            else if (chessPieceType == ChessPieceType.Rook)
            {
                return ChessPieceImages.WhiteRook;
            }
            else if (chessPieceType == ChessPieceType.Knight)
            {
                return ChessPieceImages.WhiteKnight;
            }
            else if (chessPieceType == ChessPieceType.Bishop)
            {
                return ChessPieceImages.WhiteBishop;
            }
            else if (chessPieceType == ChessPieceType.Queen)
            {
                return ChessPieceImages.WhiteQueen;
            }
            else if (chessPieceType == ChessPieceType.King)
            {
                return ChessPieceImages.WhiteKing;
            }
        }
        else
        {
            if (chessPieceType == ChessPieceType.Pawn)
            {
                return ChessPieceImages.BlackPawn;
            }
            else if (chessPieceType == ChessPieceType.Rook)
            {
                return ChessPieceImages.BlackRook;
            }
            else if (chessPieceType == ChessPieceType.Knight)
            {
                return ChessPieceImages.BlackKnight;
            }
            else if (chessPieceType == ChessPieceType.Bishop)
            {
                return ChessPieceImages.BlackBishop;
            }
            else if (chessPieceType == ChessPieceType.Queen)
            {
                return ChessPieceImages.BlackQueen;
            }
            else if (chessPieceType == ChessPieceType.King)
            {
                return ChessPieceImages.BlackKing;
            }
        }
        return ChessPieceImages.Empty;
    }
}
