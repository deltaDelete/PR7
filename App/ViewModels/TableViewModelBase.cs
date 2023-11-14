using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using DynamicData;
using ReactiveUI;

namespace App.ViewModels;

public class TableViewModelBase<T> : TableViewModelBase {
    protected List<T> _itemsFull = null!;
    private readonly ObservableAsPropertyHelper<int> _totalPages;

    private readonly Func<List<T>> _databaseGetter;

    /// <summary>
    /// Словарь, где ключ это индекс колонки, а значение селектор этой колонки
    /// </summary>
    private readonly Dictionary<int, Func<T, object>> _orderSelectors;

    /// <summary>
    /// Дефорлтный селектор на случай если не будет найдено другого 
    /// </summary>
    private readonly Func<T, object> _defaultOrderSelector;

    /// <summary>
    /// Словарь, где ключ это индекс колонки, а значение функция возвращающая булево при нужном условии
    /// </summary>
    private readonly Dictionary<int, Func<string, Func<T, bool>>> _filterSelectors;

    /// <summary>
    /// Дефорлтный селектор на случай если не будет найдено другого 
    /// </summary>
    private readonly Func<string, Func<T, bool>> _defaultFilterSelector;

    protected readonly Action<T?> _editItem;
    protected readonly Func<Task> _newItem;
    protected readonly Action<T?> _removeItem;
    protected AvaloniaList<T> _items = new();
    private List<T> _filtered = new List<T>();
    private T? _selectedRow = default;

    #region Notifying Properties

    public new AvaloniaList<T> Items {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    public new List<T> Filtered {
        get => _filtered;
        set => this.RaiseAndSetIfChanged(ref _filtered, value);
    }

    public new T? SelectedRow {
        get => _selectedRow;
        set => this.RaiseAndSetIfChanged(ref _selectedRow, value);
    }
    
    public new int TotalPages {
        get => _totalPages.Value;
    }

    #endregion

    public TableViewModelBase(
        Func<List<T>> databaseGetter,
        Dictionary<int, Func<T, object>> orderSelectors,
        Func<T, object> defaultOrderSelector,
        Dictionary<int, Func<string, Func<T, bool>>> filterSelectors,
        Func<string, Func<T, bool>> defaultFilterSelector, Action<T?> editItem, Func<Task> newItem,
        Action<T?> removeItem) {
        _databaseGetter = databaseGetter;
        _orderSelectors = orderSelectors;
        _defaultOrderSelector = defaultOrderSelector;
        _filterSelectors = filterSelectors;
        _defaultFilterSelector = defaultFilterSelector;
        _editItem = editItem;
        _newItem = newItem;
        _removeItem = removeItem;

        // TODO: решить траблы
        _totalPages = this.WhenAnyValue(x => Filtered.Count)
            .Select(x => {
                var val = (int)Math.Ceiling(x / (double)Take);
                if (val <= 0) val = 1;
                return val;
            }).ToProperty(this, x => x.TotalPages);
        
        var canTakeNext = this.WhenAnyValue(
            x => x.CurrentPage,
            selector: it => it < TotalPages);
        var canTakeBack = this.WhenAnyValue(
            x => x.CurrentPage,
            selector: it => it > 1);
        var canTakeLast = this.WhenAnyValue(
            x => x.CurrentPage,
            x => x.TotalPages,
            selector: (i1, i2) => i1 < i2);

        // var canEdit = this.WhenAnyValue(
        //     x => x.SelectedRow,
        //     selector: client => client is not null
        //                         && MainWindow.CurrentUserGroups
        //                                      .Any(it => it.Permissions.HasFlag(Permissions.Write)));
        //
        // var canInsert = MainWindow.WhenAnyValue(
        //     it => it.CurrentUserGroups,
        //     selector: it => it.Any(group => group.Permissions.HasFlag(Permissions.Insert))
        // );

        TakeNextCommand = ReactiveCommand.Create(TakeNext, canTakeNext);
        TakePrevCommand = ReactiveCommand.Create(TakePrev, canTakeBack);
        TakeFirstCommand = ReactiveCommand.Create(TakeFirst, canTakeBack);
        TakeLastCommand = ReactiveCommand.Create(TakeLast, canTakeLast);
        EditItemCommand = ReactiveCommand.Create(() => _editItem(SelectedRow)); //, canEdit);
        RemoveItemCommand = ReactiveCommand.Create(() => _removeItem(SelectedRow)); // , canEdit);
        NewItemCommand = ReactiveCommand.CreateFromTask(_newItem); //, canInsert);
        ReloadCommand = ReactiveCommand.Create(GetDataFromDb);

        GetDataFromDb();

        this.WhenAnyValue(
                x => x.SearchQuery,
                x => x.SelectedSearchColumn,
                x => x.IsSortByDescending
            )
            .DistinctUntilChanged()
            .Subscribe(OnSearchChanged);
        this.WhenAnyValue(
            x => x.Filtered
        ).Subscribe(_ => TakeFirst());
    }

    private void OnSearchChanged((string query, int column, bool isDescending) tuple) {
        if (_itemsFull is null) {
            return;
        }

        var filtered = string.IsNullOrWhiteSpace(tuple.query)
            ? _itemsFull
            : _itemsFull.Where(
                _filterSelectors.GetValueOrDefault(tuple.column, _defaultFilterSelector)(tuple.query.ToLower()));

        Filtered = tuple.isDescending switch {
            true => filtered.OrderByDescending(_orderSelectors.GetValueOrDefault(tuple.column, _defaultOrderSelector))
                .ToList(),
            false => filtered.OrderBy(_orderSelectors.GetValueOrDefault(tuple.column, _defaultOrderSelector))
                .ToList(),
        };
    }

    private async void GetDataFromDb() {
        await Task.Run(async () => {
            IsLoading = true;
            if (_databaseGetter is null) {
                throw new NullReferenceException();
            }

            var list = _databaseGetter.Invoke();
            _itemsFull = list ?? new List<T>();
            Filtered = _itemsFull;
            IsLoading = false;
            SearchQuery = string.Empty;
            return Task.CompletedTask;
        });
    }

    public void RemoveLocal(T arg) {
        Items.Remove(arg);
        _itemsFull.Remove(arg);
        Filtered.Remove(arg);
    }

    public void AddLocal(T arg) {
        _itemsFull.Add(arg);
        if (Items.Count < 10) {
            Items.Add(arg);
        }
    }

    public void ReplaceItem(T prevItem, T newItem) {
        if (Filtered.Contains(prevItem)) {
            Filtered.Replace(prevItem, newItem);
            // var index = Filtered.IndexOf(prevItem);
        }

        if (_itemsFull.Contains(prevItem)) {
            var index = _itemsFull.IndexOf(prevItem);
            _itemsFull[index] = newItem;
        }

        if (Items.Contains(prevItem)) {
            Items.Replace(prevItem, newItem);
        }
    }

    protected void TakeNext() {
        Skip += Take;
        Items = new(
            Filtered.Skip(Skip).Take(Take).ToList()
        );
    }

    protected void TakePrev() {
        Skip -= Take;
        Items = new(
            Filtered.Skip(Skip).Take(Take).ToList()
        );
    }

    protected void TakeFirst() {
        Skip = 0;
        Items = new(
            Filtered.Take(Take).ToList()
        );
    }

    protected void TakeLast() {
        Skip = Filtered.Count - Take;
        Items = new(
            Filtered.TakeLast(Take).ToList()
        );
    }
}

public abstract class TableViewModelBase : ViewModelBase {
    private int _selectedSearchColumn;
    private bool _isSortByDescending = false;
    private string _searchQuery = string.Empty;
    private int _take = 10;
    private int _skip = 0;
    private int _currentPage = 1;
    private bool _isLoading = true;

    public ReactiveCommand<Unit, Unit> EditItemCommand { get; protected set; }
    public ReactiveCommand<Unit, Unit> RemoveItemCommand { get; protected set; }
    public ReactiveCommand<Unit, Unit> NewItemCommand { get; protected set; }
    public ReactiveCommand<Unit, Unit> TakeNextCommand { get; protected set; }
    public ReactiveCommand<Unit, Unit> TakePrevCommand { get; protected set; }
    public ReactiveCommand<Unit, Unit> TakeFirstCommand { get; protected set; }
    public ReactiveCommand<Unit, Unit> TakeLastCommand { get; protected set; }
    public ReactiveCommand<Unit, Unit> ReloadCommand { get; protected set; }


    public int SelectedSearchColumn {
        get => _selectedSearchColumn;
        set => this.RaiseAndSetIfChanged(ref _selectedSearchColumn, value);
    }

    public bool IsSortByDescending {
        get => _isSortByDescending;
        set => this.RaiseAndSetIfChanged(ref _isSortByDescending, value);
    }

    public string SearchQuery {
        get => _searchQuery;
        set => this.RaiseAndSetIfChanged(ref _searchQuery, value);
    }


    public int Take {
        get => _take;
        set => this.RaiseAndSetIfChanged(ref _take, value);
    }

    public int Skip {
        get => _skip;
        set => this.RaiseAndSetIfChanged(ref _skip, value);
    }

    public int CurrentPage {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public bool IsLoading {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public int TotalPages {
        get {
            var val = (int)Math.Ceiling(Filtered.Count / (double)Take);
            if (val <= 0) val = 1;
            return val;
        }
    }

    public List<object> Filtered { get; set; } = new();
    public AvaloniaList<object> Items { get; set; } = new();
    public object? SelectedRow { get; set; } = default;
}