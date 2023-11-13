using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using App.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
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
        // if (i is null) {
        //     return;
        // }
        //
        // var mbox = MessageBoxUtils.CreateConfirmMessageBox(
        //     "Подтверждение",
        //     $"Вы действительно хотите удалить клиента {i.LastName} {i.FirstName}"
        // );
        // var result = await mbox.ShowAsPopupAsync(this);
        // if (result is not "Да") return;
        //
        // DatabaseContext.InstanceFor(this).Clients.Remove(i);
        // await DatabaseContext.InstanceFor(this).SaveChangesAsync();
        // ViewModel?.RemoveLocal(i);
        throw new NotImplementedException();
    }

    private async Task NewItem() {
        // var window = new EditClientView(group => {
        //     if (group is null) return;
        //     using var db = DatabaseContext.NewInstance();
        //     db.Clients.Add(group);
        //     db.SaveChanges();
        //     ViewModel!.AddLocal(group);
        // });
        // await window.ShowDialog(Application.Current!.MainWindow());
        throw new NotImplementedException();
    }

    private async void EditItem(Patient? i) {
        // if (i is null) return;
        // var window = new EditClientView(group => {
        //     if (group is null) return;
        //     using var db = DatabaseContext.NewInstance();
        //     db.Clients.Update(i);
        //     db.SaveChanges();
        //     ViewModel!.ReplaceItem(i, group);
        // }, i);
        // await window.ShowDialog(Application.Current!.MainWindow());
        throw new NotImplementedException();
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
        { 5, query => it => it.Phone.Contains(query, StringComparison.InvariantCultureIgnoreCase) },
    };

    private static object DefaultOrderSelector(Patient it) => it.Id;

    private static Func<Patient, bool> DefaultFilterSelector(string query)
        => it => it.Id.ToString().Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.FullName.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Address.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Email.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                 || it.Phone.Contains(query, StringComparison.InvariantCultureIgnoreCase);

}