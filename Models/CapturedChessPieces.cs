using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.Models
{
    public class CapturedChessPieces : ObservableCollection<CapturedChessPiece>
    {
        public CapturedChessPieces()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Add(new()
                    {
                        GridColumn = j.ToString(),
                        GridRow = i.ToString(),
                        Image = ChessPieceImages.Empty
                    });
                }
            }
        }
    }
}
