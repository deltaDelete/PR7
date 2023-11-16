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

public partial class SpecialityView : ReactiveUserControl<TableViewModelBase<Speciality>> {
    public SpecialityView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().Specialities.ToList(),
            OrderSelectors,
            DefaultOrderSelector,
            FilterSelectors,
            DefaultFilterSelector,
            EditItem,
            NewItem,
            RemoveItem
        );
    }

    private async void RemoveItem(Speciality? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
            "Подтверждение",
            $"Вы действительно хотите удалить препарат {i.Name}"
        );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.Specialities.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new TextBox() {
                    Watermark = "Название препарата",
                    [!TextBox.TextProperty] = new Binding("Name")
                }
            }
        };
        var dialog = new ContentDialog() {
            Title = "Добавление препарата",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = new Speciality(),
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Speciality>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Specialities.Add(item);
            await db.SaveChangesAsync();
            ViewModel!.AddLocal(item);
        });

        await dialog.ShowAsync();
    }

    private async void EditItem(Speciality? i) {
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
            Title = "Изменение типа препарата",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Speciality>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Specialities.Update(item);
            await db.SaveChangesAsync();
            ViewModel!.ReplaceItem(i, item);
        });
        
        await dialog.ShowAsync();
    }

    private static readonly Dictionary<int, Func<Speciality, object>> OrderSelectors = new() {
        { 1, it => it.Id },
        { 2, it => it.Name },
    };

    private static readonly Dictionary<int, Func<string, Func<Speciality, bool>>> FilterSelectors = new() {
        { 1, query => it => it.Id.ToString().Contains(query) },
        { 2, query => it => it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Speciality it) => it.Id;

    private static Func<Speciality, bool> DefaultFilterSelector(string query)
        => it => it.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase);
}