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

    private async void EditItem(Drug? i) {
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