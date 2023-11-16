using System;
using System.Runtime.InteropServices;
using App.ViewModels;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using FluentAvalonia.Interop;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Media.Animation;
using Brushes = Avalonia.Media.Brushes;
using Color = System.Drawing.Color;

namespace App.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainWindowViewModel();
    }

    private void NavView_OnSelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e) {
        if (e.SelectedItem is NavigationViewItem { Tag: Type type }) {
            NavFrame.Navigate(type, null, new SlideNavigationTransitionInfo());
        }
    }
}