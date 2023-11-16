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

public partial class MedicationView : ReactiveUserControl<TableViewModelBase<Medication>> {
    public MedicationView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().Medications
                                            .Include(x => x.Patient)
                                            .Include(x => x.Drug)
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

    private async void RemoveItem(Medication? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
                         "Подтверждение",
                         $"Вы действительно хотите удалить прием лекарств"
                     );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.Medications.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        await using var db = new ApplicationDbContext();
        var itemToEdit = new Medication();
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new ComboBox() {
                    PlaceholderText = "Пациент",
                    ItemsSource = db.Patients.ToList(),
                    [!ComboBox.SelectedItemProperty] = new Binding("Patient"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Patient.Id"),
                    DisplayMemberBinding = new Binding("FullName"),
                    SelectedValueBinding = new Binding("Id"),
                },
                new ComboBox() {
                    PlaceholderText = "Препарат",
                    ItemsSource = db.Drugs.ToList(),
                    [!ComboBox.SelectedItemProperty] = new Binding("Drug"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Drug.Id"),
                    DisplayMemberBinding = new Binding("Name"),
                    SelectedValueBinding = new Binding("Id"),
                },
                new NumericUpDown() {
                    Watermark = "Дозировка",
                    ShowButtonSpinner = false,
                    FormatString = "0.000 мг",
                    [!NumericUpDown.ValueProperty] = new Binding("Dosage")
                },
                new TextBox() {
                    Watermark = "Частота приема",
                    [!TextBox.TextProperty] = new Binding("Frequency")
                }
            }
        };

        var dialog = new ContentDialog() {
            Title = "Добавление приема лекарств",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = itemToEdit,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Medication>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Medications.Add(item);
            await db.SaveChangesAsync();
            ViewModel!.AddLocal(item);
        });
        await dialog.ShowAsync();
    }

    private async void EditItem(Medication? i) {
        if (i is null) return;
        await using var db = new ApplicationDbContext();
        var stack = new StackPanel {
            Spacing = 15,
            Children = {
                new ComboBox() {
                    PlaceholderText = "Пациент",
                    ItemsSource = db.Patients.ToList(),
                    [!ComboBox.SelectedItemProperty] = new Binding("Patient"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Patient.Id"),
                    DisplayMemberBinding = new Binding("FullName"),
                    SelectedValueBinding = new Binding("Id"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                },
                new ComboBox() {
                    PlaceholderText = "Препарат",
                    ItemsSource = db.Drugs.ToList(),
                    [!ComboBox.SelectedItemProperty] = new Binding("Drug"),
                    [!ComboBox.SelectedValueProperty] = new Binding("Drug.Id"),
                    DisplayMemberBinding = new Binding("Name"),
                    SelectedValueBinding = new Binding("Id"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                },
                new NumericUpDown() {
                    Watermark = "Дозировка",
                    ShowButtonSpinner = false,
                    FormatString = "0.000 мг",
                    [!NumericUpDown.ValueProperty] = new Binding("Dosage")
                },
                new TextBox() {
                    Watermark = "Частота приема",
                    [!TextBox.TextProperty] = new Binding("Frequency")
                }
            }
        };
        var dialog = new ContentDialog() {
            Title = "Изменение приема лекарств",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Medication>(stack.Children, async item => {
            if (item is null) return;
            db.Attach(item);
            db.Medications.Update(item);
            await db.SaveChangesAsync();
            ViewModel!.ReplaceItem(i, item);
        });

        await dialog.ShowAsync();
    }

    private static readonly Dictionary<int, Func<Medication, object>> OrderSelectors = new() {
        { 1, it => it.Id },
        { 2, it => it.Patient.FullName },
        { 3, it => it.Drug.Name },
        { 4, it => it.Dosage },
        { 5, it => it.Frequency },
    };

    private static readonly Dictionary<int, Func<string, Func<Medication, bool>>> FilterSelectors = new() {
        { 1, query => it => it.Id.ToString().Contains(query) },
        { 2, query => it => it.Patient.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 3, query => it => it.Drug.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 4, query => it => it.Dosage.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 5, query => it => it.Frequency.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Medication it) => it.Id;

    private static Func<Medication, bool> DefaultFilterSelector(string query)
        => it => it.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Patient.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Drug.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Dosage.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Frequency.Contains(query, StringComparison.InvariantCultureIgnoreCase);
}