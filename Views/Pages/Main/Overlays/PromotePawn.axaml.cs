using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;

namespace ChessAvalonia.Views.Pages.Main.Overlays;
public partial class PromotePawn : UserControl
{
    public PromotePawn()
    {
        InitializeComponent();
        DataContext = new PromotePawnViewModel();
    }
}