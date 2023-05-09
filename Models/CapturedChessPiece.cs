using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.Models
{
    [INotifyPropertyChanged]
    public partial class CapturedChessPiece
    {
        [ObservableProperty]
        private string gridColumn;

        [ObservableProperty]
        private string gridRow;

        [ObservableProperty]
        private string image;
    }
}
