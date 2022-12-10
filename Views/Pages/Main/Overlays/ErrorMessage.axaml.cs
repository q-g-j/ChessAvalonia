using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ChessAvalonia.ViewModels.Pages.Main.Overlays;

namespace ChessAvalonia.Views.Pages.Main.Overlays;

public partial class ErrorMessage : UserControl
{
    public ErrorMessage()
    {
        InitializeComponent();
        DataContext = new MainPageErrorMessageViewModel();
    }
}