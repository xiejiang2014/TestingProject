using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

//https://github.com/xiejiang2014/WpfFrame/blob/main/WpfFrame/NotificationList.cs

namespace WpfMenus;

public class ItemPropertyChangedEventArgs<T> : PropertyChangedEventArgs
{
    public T Item { get; set; }

    public ItemPropertyChangedEventArgs(string propertyName) : base(propertyName)
    {
    }

    public ItemPropertyChangedEventArgs(T item, string propertyName) : base(propertyName)
    {
        Item = item;
    }
}

/// <summary>
/// ObservableCollection 的增强版,可提供子对象的属性变化事件.其中只能包含实现了 INotifyPropertyChanged 接口的对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class NotificationList<T> : ObservableCollectionEx<T> where T : class, INotifyPropertyChanged
{
    public NotificationList(IEnumerable<T> collection)
    {
        foreach (var item in collection)
        {
            Add(item);
        }
    }

    public NotificationList()
    {
    }


    /// <summary>
    /// 列表中对象属性改变时激活.
    /// </summary>
    public event EventHandler<ItemPropertyChangedEventArgs<T>>? ItemPropertyChanged;

    public new void Add(T item)
    {
        item.PropertyChanged += Item_PropertyChanged;
        base.Add(item);

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AnyItem)));
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            item.PropertyChanged += Item_PropertyChanged;
            base.Add(item);
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AnyItem)));
    }

    public new void Insert(int index, T item)
    {
        item.PropertyChanged += Item_PropertyChanged;
        base.Insert(index, item);
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AnyItem)));
    }

    public new bool Remove(T item)
    {
        item.PropertyChanged -= Item_PropertyChanged;
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AnyItem)));
        return base.Remove(item);
    }

    public new void RemoveAt(int index)
    {
        this[index].PropertyChanged -= Item_PropertyChanged;
        base.RemoveAt(index);
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AnyItem)));
    }


    public bool AnyItem => this.Any();

    public new void Clear()
    {
        foreach (var v in this)
        {
            v.PropertyChanged -= Item_PropertyChanged;
        }

        base.Clear();
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AnyItem)));
    }

    private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var args = new ItemPropertyChangedEventArgs<T>((T) sender, e.PropertyName);
        OnItemPropertyChanged(args);
        ItemPropertyChanged?.Invoke(this, args);
    }


    protected virtual void OnItemPropertyChanged(ItemPropertyChangedEventArgs<T> e)
    {
    }
}

public interface ISelectableNotificationList
{
    /// <summary>
    /// 是否允许多选,默认是
    /// </summary>
    bool CanMultipleSelect { get; set; }

    /// <summary>
    /// 当前选中项数量
    /// </summary>
    int CountOfSelected { get; }

    /// <summary>
    /// 当前是否多选
    /// </summary>
    bool AreMultipleSelected { get; }

    /// <summary>
    /// 当前是否单选
    /// </summary>
    bool IsSingleSelected { get; }

    /// <summary>
    /// 当前是否选中了至少1项
    /// </summary>
    bool IsAnySelected { get; }

    bool IsAllSelected { get; set; }


    bool AnyItem { get; }
    int  Count   { get; }
}

public interface ISelectableNotificationList<T> : ISelectableNotificationList
{
    /// <summary>
    /// 列表中对象属性改变时激活.
    /// </summary>
    public event EventHandler<ItemPropertyChangedEventArgs<T>> ItemPropertyChanged;

    IEnumerable<T> SelectedItems { get; }

    T? FirstSelectedItem { get; }
}

public class SelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// 在本次选中变化事件中,新的被选中项
    /// </summary>
    public List<ISelectableItem>? Selected { get; set; }

    /// <summary>
    /// 在本次选中变化事件中,新的未被选中项
    /// </summary>
    public List<ISelectableItem>? Unselected { get; set; }
}

public class SelectableNotificationList<T> : NotificationList<T>, ISelectableNotificationList<T>
    where T : class, ISelectableItem
{
    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public SelectableNotificationList(IEnumerable<T> collection) : base(collection)
    {
    }

    public SelectableNotificationList()
    {
    }

    protected override void OnItemPropertyChanged(ItemPropertyChangedEventArgs<T> e)
    {
        if (e.Item is { } selectableItem)
        {
            if (e.PropertyName == nameof(selectableItem.IsSelected))
            {
                if (selectableItem.IsSelected == false)
                {
                    //有任意一项未选,那么肯定不是全选了
                    _isAllSelectedEnable = false;
                    IsAllSelected        = false;
                    _isAllSelectedEnable = true;
                }
                else
                {
                    //检查是否全选了
                    if (this.All(v => v.IsSelected))
                    {
                        _isAllSelectedEnable = false;
                        IsAllSelected        = true;
                        _isAllSelectedEnable = true;
                    }


                    //如果是单选状态,那么要将其它所有项都设为未选状态
                    if (!CanMultipleSelect)
                    {
                        foreach (var item in this)
                        {
                            if (!ReferenceEquals(item, selectableItem))
                            {
                                if (item is not null)
                                {
                                    item.IsSelected = false;
                                }
                            }
                        }
                    }
                }

                OnPropertyChanged(new PropertyChangedEventArgs(nameof(CountOfSelected)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreMultipleSelected)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsSingleSelected)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsAnySelected)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(FirstSelectedItem)));

                if (SelectionChanged is not null)
                {
                    var selectionChangedEventArgs = new SelectionChangedEventArgs();
                    if (selectableItem.IsSelected)
                    {
                        selectionChangedEventArgs.Selected = new List<ISelectableItem>() {selectableItem};
                    }
                    else
                    {
                        selectionChangedEventArgs.Unselected = new List<ISelectableItem>() {selectableItem};
                    }


                    SelectionChanged?.Invoke(this, selectionChangedEventArgs);
                }
            }
        }
    }


    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        //在单选模式下,如果添加了新项,并且原本已存在选中项,那么新增的项不能是选中的
        if (!CanMultipleSelect     &&
            e.NewItems is not null &&
            e.NewItems.Count != 0  &&
            AreMultipleSelected)
        {
            var isFirstSelectedSkipped = false;

            for (var i = 0; i < this.Count; i++)
            {
                if (this[i].IsSelected)
                {
                    if (isFirstSelectedSkipped)
                    {
                        isFirstSelectedSkipped = true;
                        continue;
                    }

                    this[i].IsSelected = false;
                }
            }
        }

        base.OnCollectionChanged(e);
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(CountOfSelected)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AreMultipleSelected)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsSingleSelected)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsAnySelected)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(AnyItem)));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsAllSelected)));
    }

    /// <summary>
    /// 是否允许多选,默认是
    /// </summary>
    public bool CanMultipleSelect { get; set; } = true;


    /// <summary>
    /// 当前选中项数量
    /// </summary>
    public int CountOfSelected => this.Count(v => v.IsSelected);

    /// <summary>
    /// 当前是否多选
    /// </summary>
    public bool AreMultipleSelected => CountOfSelected > 1;

    /// <summary>
    /// 当前是否单选
    /// </summary>
    public bool IsSingleSelected => CountOfSelected == 1;

    /// <summary>
    /// 当前是否选中了至少1项
    /// </summary>
    public bool IsAnySelected => CountOfSelected >= 1;


    private bool _isAllSelectedEnable = true;

    private bool _isAllSelected;

    public bool IsAllSelected
    {
        get => _isAllSelected;
        set
        {
            _isAllSelected = value;

            if (_isAllSelectedEnable)
            {
                foreach (var item in this)
                {
                    item.IsSelected = value;
                }
            }

            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsAllSelected)));
        }
    }

    public IEnumerable<T> SelectedItems => this.Where(v => v.IsSelected);

    public T? FirstSelectedItem => this.FirstOrDefault(v => v.IsSelected);
}

public interface ISelectableItem : INotifyPropertyChanged
{
    bool IsSelected { get; set; }
}

public class SelectableItemBase<T> : ISelectableItem
{
    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    private T    _payload;
    public T Payload
    {
        get => _payload;
        set => SetField(ref _payload, value);
    }


    #region PropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion
}