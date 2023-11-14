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
using Microsoft.EntityFrameworkCore;

namespace App.Views; 

public partial class DoctorView : ReactiveUserControl<TableViewModelBase<Doctor>> {
    public DoctorView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().Doctors
                .Include(x => x.Speciality)
                .ToList(),
            OrderSelectors,
            DefaultOrderSelector,
            FilterSelectors,
            DefaultFilterSelector,
            EditItem,
            NewItem,
            RemoveItem
            
        );
    }
    
     private async void RemoveItem(Doctor? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
            "Подтверждение",
            $"Вы действительно хотите удалить клиента {i.Name}"
        );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.Doctors.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        await using var db = new ApplicationDbContext();
        var dialog = new ContentDialog() {
            Title = "Добавление доктора",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = new Doctor(),
            Content = new StackPanel {
                Spacing = 15,
                Children = {
                    new TextBox() {
                        Watermark = "Имя",
                        [!TextBox.TextProperty] = new Binding("Name")
                    },
                    new TextBox() {
                        Watermark = "Телефон",
                        [!TextBox.TextProperty] = new Binding("Phone")
                    },
                    new TextBox() {
                        Watermark = "Почта",
                        [!TextBox.TextProperty] = new Binding("Email")
                    },
                    new ComboBox() {
                        PlaceholderText = "Специальность",
                        ItemsSource = db.Specialities.ToList(),
                        [!ComboBox.SelectedItemProperty] = new Binding("Speciality"),
                        DisplayMemberBinding = new Binding("Name"),
                        SelectedValueBinding = new Binding("Id")
                    }
                }
            },
            DefaultButton = ContentDialogButton.Primary
        };
        var result = await dialog.ShowAsync();
        if (result is not ContentDialogResult.Primary) {
            return;
        }

        var item = dialog.DataContext as Doctor;
        if (item is null) return;
        db.Doctors.Add(item);
        await db.SaveChangesAsync();
        ViewModel!.AddLocal(item);
    }

    private async void EditItem(Doctor? i) {
        if (i is null) return;
        await using var db = new ApplicationDbContext();
        var specialities = db.Specialities.ToList();
        var dialog = new ContentDialog() {
            Title = "Изменение доктора",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = new StackPanel {
                Spacing = 15,
                Children = {
                    new TextBox() {
                        Watermark = "Имя",
                        [!TextBox.TextProperty] = new Binding("Name")
                    },
                    new TextBox() {
                        Watermark = "Телефон",
                        [!TextBox.TextProperty] = new Binding("Phone")
                    },
                    new TextBox() {
                        Watermark = "Почта",
                        [!TextBox.TextProperty] = new Binding("Email")
                    },
                    new ComboBox() {
                        PlaceholderText = "Специальность",
                        ItemsSource = specialities,
                        [!ComboBox.SelectedItemProperty] = new Binding("Speciality"),
                        [!ComboBox.SelectedValueProperty] = new Binding("Speciality.Id"),
                        DisplayMemberBinding = new Binding("Name"),
                        SelectedValueBinding = new Binding("Id")
                    }
                }
            },
            DefaultButton = ContentDialogButton.Primary
        };
        var result = await dialog.ShowAsync();
        if (result is not ContentDialogResult.Primary) {
            return;
        }

        var item = dialog.DataContext as Doctor;
        if (item is null) return;
        db.Doctors.Update(item);
        await db.SaveChangesAsync();
        ViewModel!.ReplaceItem(i, item);
    }
    
    private static readonly Dictionary<int, Func<Doctor, object>> OrderSelectors = new() {
        { 1, it => it.DoctorId },
        { 2, it => it.Name },
        { 3, it => it.Speciality.Name },
        { 4, it => it.Email },
        { 5, it => it.Phone },
    };

    private static readonly Dictionary<int, Func<string, Func<Doctor, bool>>> FilterSelectors = new() {
        { 1, query => it => it.DoctorId.ToString().Contains(query) },
        { 2, query => it => it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 3, query => it => it.Speciality.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 4, query => it => it.Email.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 5, query => it => it.Phone.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Doctor it) => it.DoctorId;

    private static Func<Doctor, bool> DefaultFilterSelector(string query)
        => it => it.DoctorId.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Speciality.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Email.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Phone.Contains(query, StringComparison.InvariantCultureIgnoreCase);

}