using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAvalonia.ViewModels.Pages.Main
{
    [INotifyPropertyChanged]
    public partial class CanvasCell
    {
        [ObservableProperty]
        private string cellName;

        [ObservableProperty]
        private int canvasLeft;

        [ObservableProperty]
        private int canvasTop;

        [ObservableProperty]
        private string image;

        [ObservableProperty]
        private string backgroundColor;
    }
}
