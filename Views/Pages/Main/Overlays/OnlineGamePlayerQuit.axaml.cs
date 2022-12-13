using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;

namespace ChessAvalonia.Views.Pages.Main.Overlays;

public partial class OnlineGamePlayerQuit : UserControl
{
    public OnlineGamePlayerQuit()
    {
        InitializeComponent();
        DataContext = new OnlineGamePlayerQuitViewModel();
    }
}