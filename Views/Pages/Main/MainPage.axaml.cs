using Avalonia.Controls;
using ChessAvalonia.ViewModels.Pages.Main;

namespace ChessAvalonia.Views.Pages.Main;

public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();
        DataContext = new MainPageViewModel();
    }
}