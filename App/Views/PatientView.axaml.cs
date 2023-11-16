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
using Microsoft.EntityFrameworkCore.Storage;

namespace App.Views;

public partial class PatientView : ReactiveUserControl<TableViewModelBase<Patient>> {
    public PatientView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().Patients.ToList(),
            OrderSelectors,
            DefaultOrderSelector,
            FilterSelectors,
            DefaultFilterSelector,
            EditItem,
            NewItem,
            RemoveItem
            
        );
    }
    
    private async void RemoveItem(Patient? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
                         "Подтверждение",
                         $"Вы действительно хотите удалить пациента {i.FullName}"
                     );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.Patients.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        await using var db = new ApplicationDbContext();
        var itemToEdit = new Patient();
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new TextBox() {
                    Watermark = "Имя",
                    [!TextBox.TextProperty] = new Binding("FullName")
                },
                new NumericUpDown() {
                    Watermark = "Телефон",
                    ShowButtonSpinner = false,
                    Minimum = 0,
                    FormatString = "+0 (###) ###-####",
                    [!NumericUpDown.ValueProperty] = new Binding("Phone"),
                },
                new TextBox() {
                    Watermark = "Почта",
                    [!TextBox.TextProperty] = new Binding("Email")
                },
                new TextBox() {
                    Watermark = "Адрес",
                    [!TextBox.TextProperty] = new Binding("Address")
                },
            }
        };

        var dialog = new ContentDialog() {
            Title = "Добавление пациента",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = itemToEdit,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Patient>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Patients.Add(item);
            await db.SaveChangesAsync();
            ViewModel!.AddLocal(item);
        });
        await dialog.ShowAsync();
    }

    private async void EditItem(Patient? i) {
        if (i is null) return;
        await using var db = new ApplicationDbContext();
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new TextBox() {
                    Watermark = "Имя",
                    [!TextBox.TextProperty] = new Binding("FullName")
                },
                new NumericUpDown() {
                    Watermark = "Телефон",
                    ShowButtonSpinner = false,
                    Minimum = 0,
                    FormatString = "+0 (###) ###-####",
                    [!NumericUpDown.ValueProperty] = new Binding("Phone"),
                },
                new TextBox() {
                    Watermark = "Почта",
                    [!TextBox.TextProperty] = new Binding("Email")
                },
                new TextBox() {
                    Watermark = "Адрес",
                    [!TextBox.TextProperty] = new Binding("Address")
                },
            }
        };

        var dialog = new ContentDialog() {
            Title = "Изменение пациента",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Patient>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Patients.Update(item);
            await db.SaveChangesAsync();
            ViewModel!.AddLocal(item);
        });
        await dialog.ShowAsync();
    }
    
    private static readonly Dictionary<int, Func<Patient, object>> OrderSelectors = new() {
        { 1, it => it.Id },
        { 2, it => it.FullName },
        { 3, it => it.Address },
        { 4, it => it.Email },
        { 5, it => it.Phone },
    };

    private static readonly Dictionary<int, Func<string, Func<Patient, bool>>> FilterSelectors = new() {
        { 1, query => it => it.Id.ToString().Contains(query) },
        { 2, query => it => it.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 3, query => it => it.Address.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 4, query => it => it.Email.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 5, query => it => it.Phone.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Patient it) => it.Id;

    private static Func<Patient, bool> DefaultFilterSelector(string query)
        => it => it.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Address.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Email.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Phone.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase);

}