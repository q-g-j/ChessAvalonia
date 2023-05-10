using CommunityToolkit.Mvvm.ComponentModel;

namespace ChessAvalonia.ViewModels.Pages.Main
{
    [INotifyPropertyChanged]
    public partial class CanvasImage
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
        private int zIndex;
    }
}
