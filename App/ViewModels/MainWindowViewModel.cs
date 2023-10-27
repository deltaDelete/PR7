using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ReactiveUI;

namespace App.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public ApplicationDbContext Database { get; private set; }

    public MainWindowViewModel() {
        // TODO: Адаптировать код из прошлых работ
    }
}