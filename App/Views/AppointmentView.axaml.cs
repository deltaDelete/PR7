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

public partial class AppointmentView : ReactiveUserControl<TableViewModelBase<Appointment>> {
    public AppointmentView() {
        InitializeComponent();
        ViewModel = new(
            () => new ApplicationDbContext().Appointments
                                            .Include(x => x.Patient)
                                            .Include(x => x.Doctor)
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

    private async void RemoveItem(Appointment? i) {
        if (i is null) {
            return;
        }

        var result = await MessageBoxUtils.ShowYesNoDialog(
                         "Подтверждение",
                         $"Вы действительно хотите удалить запись"
                     );
        if (result is not ContentDialogResult.Primary) return;
        await using var db = new ApplicationDbContext();
        db.Appointments.Remove(i);
        await db.SaveChangesAsync();
        ViewModel?.RemoveLocal(i);
    }

    private async Task NewItem() {
        var itemToEdit = new Appointment();
        var stack = GenerateDialogPanel();

        var dialog = new ContentDialog() {
            Title = "Добавление записи",
            PrimaryButtonText = "Создать",
            CloseButtonText = "Закрыть",
            DataContext = itemToEdit,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Appointment>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Appointments.Add(item);
            await db.SaveChangesAsync();
            ViewModel!.AddLocal(item);
        });
        await dialog.ShowAsync();
    }

    private async void EditItem(Appointment? i) {
        if (i is null) return;
        var stack = GenerateDialogPanel();
        // TODO Где то здесь NullReferenceException
        var dialog = new ContentDialog() {
            Title = "Изменение записи",
            PrimaryButtonText = "Изменить",
            CloseButtonText = "Закрыть",
            DataContext = i,
            Content = stack,
            DefaultButton = ContentDialogButton.Primary,
            [!ContentDialog.PrimaryButtonCommandParameterProperty] = new Binding(".")
        };

        dialog.AddControlValidation<Appointment>(stack.Children, async item => {
            if (item is null) return;
            await using var db = new ApplicationDbContext();
            db.Attach(item);
            db.Appointments.Update(item);
            await db.SaveChangesAsync();
            ViewModel!.ReplaceItem(i, item);
        });

        await dialog.ShowAsync();
    }

    private static readonly Dictionary<int, Func<Appointment, object>> OrderSelectors = new() {
        { 1, it => it.AppointmentId },
        { 2, it => it.Patient.FullName },
        { 3, it => it.Doctor.Name },
        { 4, it => it.Date },
        { 5, it => it.Type },
        { 6, it => it.Comment },
    };

    private static readonly Dictionary<int, Func<string, Func<Appointment, bool>>> FilterSelectors = new() {
        { 1, query => it => it.AppointmentId.ToString().Contains(query) },
        { 2, query => it => it.Patient.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 3, query => it => it.Doctor.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 4, query => it => it.Date.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 5, query => it => it.Type.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
        { 6, query => it => it.Comment.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Appointment it) => it.AppointmentId;

    private static Func<Appointment, bool> DefaultFilterSelector(string query)
        => it => it.AppointmentId.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Patient.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Doctor.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Date.ToString(CultureInfo.InvariantCulture).Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Type.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Comment.Contains(query, StringComparison.InvariantCultureIgnoreCase);
    
    private StackPanel GenerateDialogPanel() {
        using var db = new ApplicationDbContext();
        var doctors = db.Doctors.ToList();
        var patients = db.Patients.ToList();
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
                    SelectedValueBinding = new Binding("Id"),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                },
                new DatePicker() {
                    [!DatePicker.SelectedDateProperty] = new Binding("Date"),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                },
                new TimePicker() {
                    [!TimePicker.SelectedTimeProperty] = new Binding("Time"),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                },
                new TextBox() {
                    Watermark = "Тип",
                    [!TextBox.TextProperty] = new Binding("Type")
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