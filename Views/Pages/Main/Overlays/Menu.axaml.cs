using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;

namespace ChessAvalonia.Views.Pages.Main.Overlays;
public partial class Menu : UserControl
{
    public Menu()
    {
        InitializeComponent();
        DataContext = new MenuViewModel();
    }
}