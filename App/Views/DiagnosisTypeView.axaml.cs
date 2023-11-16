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

public partial class DiagnosisTypeView : ReactiveUserControl<TableViewModelBase<DiagnosisType>> {
    public DiagnosisTypeView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().DiagnosisTypes.ToList(),
            OrderSelectors,
            DefaultOrderSelector,
            FilterSelectors,
            DefaultFilterSelector,
            EditItem,
            NewItem,
            RemoveItem
        );
    }

    private async void RemoveItem(DiagnosisType? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
            "Подтверждение",
            $"Вы действительно хотите удалить тип диагноза {i.Name}"
        );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.DiagnosisTypes.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new TextBox() {
                    Watermark = "Название типа диагноза",
                    [!TextBox.TextProperty] = new Binding("Name")
                }
            }
        };
        var dialog = new ContentDialog() {
            Title = "Добавление типа дизноза",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = new DiagnosisType(),
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<DiagnosisType>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.DiagnosisTypes.Add(item);
            await db.SaveChangesAsync();
            ViewModel!.AddLocal(item);
        });
        await dialog.ShowAsync();
    }

    private async void EditItem(DiagnosisType? i) {
        if (i is null) return;
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new TextBox() {
                    Watermark = "Название препарата",
                    [!TextBox.TextProperty] = new Binding("Name"),
                }
            }
        };
        var dialog = new ContentDialog() {
            Title = "Изменение типа диагноза",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<DiagnosisType>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.DiagnosisTypes.Update(item);
            await db.SaveChangesAsync();
            ViewModel!.ReplaceItem(i, item);
        });
        await dialog.ShowAsync();
    }

    private static readonly Dictionary<int, Func<DiagnosisType, object>> OrderSelectors = new() {
        { 1, it => it.Id },
        { 2, it => it.Name },
    };

    private static readonly Dictionary<int, Func<string, Func<DiagnosisType, bool>>> FilterSelectors = new() {
        { 1, query => it => it.Id.ToString().Contains(query) },
        { 2, query => it => it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(DiagnosisType it) => it.Id;

    private static Func<DiagnosisType, bool> DefaultFilterSelector(string query)
        => it => it.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase);
}