using System.Threading.Tasks;
using App.Models;
using Avalonia.Controls;
using Avalonia.Data;
using FluentAvalonia.UI.Controls;

namespace App.Utils; 

public class MessageBoxUtils {
    public static Task<ContentDialogResult> ShowSimpleMessageBox(string title, string message) {
        var dialog = new ContentDialog() {
            Title = title,
            PrimaryButtonText = "Ок",
            Content = new StackPanel {
                Spacing = 15,
                Children = {
                    new TextBlock() {
                        Text = message
                    }
                }
            },
        };
        return dialog.ShowAsync();
    }
    
    public static Task<ContentDialogResult> ShowYesNoDialog(string title, string message) {
        var dialog = new ContentDialog() {
            Title = title,
            PrimaryButtonText = "Да",
            SecondaryButtonText = "Нет",
            Content = new StackPanel {
                Spacing = 15,
                Children = {
                    new TextBlock() {
                        Text = message
                    }
                }
            },
        };
        return dialog.ShowAsync();
    }
}