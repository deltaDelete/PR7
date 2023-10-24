using App.ViewModels;
using Avalonia.Controls;
using Avalonia.ReactiveUI;

namespace App.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();
    }
}