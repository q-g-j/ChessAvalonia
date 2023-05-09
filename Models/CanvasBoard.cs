using ChessAvalonia.ViewModels.Pages.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.Models
{
    public class CanvasBoard : ObservableCollection<CanvasCell>
    {
        public CanvasBoard()
        {
            int t = 0;
            for (int col = 8; col > 0; col--, t++)
            {
                int l = 0;
                for (int row = 1; row < 9; row++, l++)
                {
                    CanvasCell cell = new()
                    {
                        CellName = Coords.IntsToCoordsString(row, col),
                        CanvasLeft = l * 60,
                        CanvasTop = t * 60,
                        Image = ChessPieceImages.Empty,
                    };

                    if (row % 2 != 0)
                    {
                        if (col % 2 == 0)
                        {
                            cell.BackgroundColor = "#ffce9e";
                        }
                        else
                        {
                            cell.BackgroundColor = "#d18b47";
                        }
                    }
                    else
                    {
                        if (col % 2 != 0)
                        {
                            cell.BackgroundColor = "#ffce9e";
                        }
                        else
                        {
                            cell.BackgroundColor = "#d18b47";
                        }
                    }
                    Add(cell);
                }
            }
        }
    }
}
