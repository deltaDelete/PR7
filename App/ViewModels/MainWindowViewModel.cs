using System;
using System.Linq;
using System.Reactive.Linq;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ReactiveUI;

namespace App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ApplicationDbContext Database { get; private set; }

    public MainWindowViewModel() {
        this.WhenAnyValue(x => x.Database)
            .Subscribe(EnsureDatabase);
    }

    private async void EnsureDatabase(ApplicationDbContext? db) {
        if (db is null || db.Patients.Any()) {
            Database = new ApplicationDbContext();
            await Database.Database.OpenConnectionAsync();
            return;
        }

        await db.Database.EnsureCreatedAsync();
        await db.Patients.AddAsync(new Patient() {
            Address = "Аскабан",
            Email = "user@example.com",
            FullName = "Сигмов Сигма Сигмович",
            Phone = "+7 912 345 67 89"
        });
    }
}