using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using App.Utils;
using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FluentAvalonia.UI.Controls;

namespace App.Views;

public partial class DrugView : ReactiveUserControl<TableViewModelBase<Drug>> {
    public DrugView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().Drugs.ToList(),
            OrderSelectors,
            DefaultOrderSelector,
            FilterSelectors,
            DefaultFilterSelector,
            EditItem,
            NewItem,
            RemoveItem
        );
    }

    private async void RemoveItem(Drug? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
            "Подтверждение",
            $"Вы действительно хотите удалить клиента {i.Name}"
        );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.Drugs.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        var dialog = new ContentDialog() {
            Title = "Добавление препарата",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = new Drug(),
            Content = new StackPanel {
                Spacing = 15,
                Children = {
                    new TextBox() {
                        Watermark = "Название препарата",
                        [!TextBox.TextProperty] = new Binding("Name")
                    }
                }
            },
            DefaultButton = ContentDialogButton.Primary
        };
        var result = await dialog.ShowAsync();
        if (result is not ContentDialogResult.Primary) {
            return;
        }

        var drug = dialog.DataContext as Drug;
        if (drug is null) return;
        await using var db = new ApplicationDbContext();
        db.Drugs.Add(drug);
        await db.SaveChangesAsync();
        ViewModel!.AddLocal(drug);
    }

    private async void EditItem(Drug? i) {
        if (i is null) return;
        var dialog = new ContentDialog() {
            Title = "Изменение препарата",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = new StackPanel {
                Spacing = 15,
                Children = {
                    new TextBox() {
                        Watermark = "Название препарата",
                        [!TextBox.TextProperty] = new Binding("Name"),
                    }
                }
            },
            DefaultButton = ContentDialogButton.Primary
        };
        var result = await dialog.ShowAsync();
        if (result is not ContentDialogResult.Primary) {
            return;
        }

        var drug = dialog.DataContext as Drug;
        if (drug is null) return;
        await using var db = new ApplicationDbContext();
        db.Drugs.Update(drug);
        await db.SaveChangesAsync();
        ViewModel!.ReplaceItem(i, drug);
    }

    private static readonly Dictionary<int, Func<Drug, object>> OrderSelectors = new() {
        { 1, it => it.Id },
        { 2, it => it.Name },
    };

    private static readonly Dictionary<int, Func<string, Func<Drug, bool>>> FilterSelectors = new() {
        { 1, query => it => it.Id.ToString().Contains(query) },
        { 2, query => it => it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Drug it) => it.Id;

    private static Func<Drug, bool> DefaultFilterSelector(string query)
        => it => it.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase);
}