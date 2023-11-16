using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using App.Models;
using App.Utils;
using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using DynamicData;
using DynamicData.Binding;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Controls;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace App.Views;

public partial class DiagnosisView : ReactiveUserControl<TableViewModelBase<Diagnosis>> {
    public DiagnosisView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().Diagnoses
                                            .Include(x => x.Patient)
                                            .Include(x => x.Doctor)
                                            .Include(x => x.Type)
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

    private async void RemoveItem(Diagnosis? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
                         "Подтверждение",
                         $"Вы действительно хотите удалить диагноз"
                     );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.Diagnoses.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        var itemToEdit = new Diagnosis();
        var stack = GenerateDialogPanel();

        var dialog = new ContentDialog() {
            Title = "Добавление диагноза",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = itemToEdit,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Diagnosis>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Diagnoses.Add(item);
            await db.SaveChangesAsync();
            ViewModel!.AddLocal(item);
        });
        await dialog.ShowAsync();
    }

    private async void EditItem(Diagnosis? i) {
        if (i is null) return;
        var stack = GenerateDialogPanel();
        var dialog = new ContentDialog() {
            Title = "Изменение диагноза",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Diagnosis>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Diagnoses.Update(item);
            await db.SaveChangesAsync();
            ViewModel!.ReplaceItem(i, item);
        });

        await dialog.ShowAsync();
    }

    private static readonly Dictionary<int, Func<Diagnosis, object>> OrderSelectors = new() {
        { 1, it => it.Id },
        { 2, it => it.Patient.FullName },
        { 3, it => it.Doctor.Name },
        { 4, it => it.Date },
        { 5, it => it.Type },
        { 6, it => it.Comment },
    };

    private static readonly Dictionary<int, Func<string, Func<Diagnosis, bool>>> FilterSelectors = new() {
        { 1, query => it => it.Id.ToString().Contains(query) },
        { 2, query => it => it.Patient.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 3, query => it => it.Doctor.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 4, query => it => it.Date.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 5, query => it => it.Type.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 6, query => it => it.Comment.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Diagnosis it) => it.Id;

    private static Func<Diagnosis, bool> DefaultFilterSelector(string query)
        => it => it.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Patient.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Doctor.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Date.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Type.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Comment.Contains(query, StringComparison.InvariantCultureIgnoreCase);
    
    private StackPanel GenerateDialogPanel() {
        using var db = new ApplicationDbContext();
        var doctors = db.Doctors.ToList();
        var patients = db.Patients.ToList();
        var types = db.DiagnosisTypes.ToList();
        db.Dispose();
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new ComboBox() {
                    PlaceholderText = "Пациент",
                    ItemsSource = patients,
                    [!ComboBox.SelectedItemProperty] = new Binding("Patient"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Patient.Id"),
                    DisplayMemberBinding = new Binding("FullName"),
                    SelectedValueBinding = new Binding("Id"),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                },
                new ComboBox() {
                    PlaceholderText = "Врач",
                    ItemsSource = doctors,
                    [!ComboBox.SelectedItemProperty] = new Binding("Doctor"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Doctor.DoctorId"),
                    DisplayMemberBinding = new Binding("Name"),
                    SelectedValueBinding = new Binding("DoctorId"),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                },
                new ComboBox() {
                    PlaceholderText = "Тип диагноза",
                    ItemsSource = types,
                    [!ComboBox.SelectedItemProperty] = new Binding("Type"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Type.Id"),
                    DisplayMemberBinding = new Binding("Name"),
                    SelectedValueBinding = new Binding("Id"),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                },
                new DatePicker() {
                    [!DatePicker.SelectedDateProperty] = new Binding("Date"),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                },
                new TextBox() {
                    Watermark = "Примечание",
                    [!TextBox.TextProperty] = new Binding("Comment")
                }
            }
        };
        return stack;
    }
}