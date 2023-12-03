using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace WpfMenus
{
    public class MenusViewModel : BindableBase
    {
        public MenusViewModel()
        {
            GroupName = Guid.NewGuid().ToString("N");
        }

        #region 菜单项

        /// <summary>
        /// 菜单分组名,用来保证在当前组内同时只有一个菜单项被选择
        /// </summary>
        public string GroupName { get; private set; }

        public ObservableCollection<MenuItemViewModel> MenuList { get; } = new();

        /// <summary>
        /// 所有菜单项
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuItemViewModel> AllMenuItemsList()
        {
            foreach (var menuItemViewModel in MenuList)
            {
                yield return menuItemViewModel;

                foreach (var itemViewModel in menuItemViewModel.Descendants())
                {
                    yield return itemViewModel;
                }
            }
        }

        #endregion

        #region 菜单项被选择

        public event EventHandler<MenuItemViewModel>? MenuItemClicked;

        /// <summary>
        /// 当前被选中的菜单项
        /// </summary>
        public MenuItemViewModel? CurrentSelectedMenuItem => AllMenuItemsList().FirstOrDefault(v => v.IsSelected);

        private DelegateCommand<object[]>? _selectedItemChangedCommand;

        public DelegateCommand<object[]> SelectedItemChangedCommand => _selectedItemChangedCommand ??=
            new DelegateCommand<object[]>(SelectedItemChangedExecute);

        private void SelectedItemChangedExecute(object[] objects)
        {
            if (objects[0] is MenuItemViewModel menuItemViewModel)
            {
                SelectMenuItem(menuItemViewModel, objects[1]);
            }
        }

        public void SelectMenuItem(MenuItemViewModel menuItemViewModel, object? parameter = null)
        {
            //除了被选项之外其它的项都应该是不选中的
            foreach (var other in AllMenuItemsList())
            {
                if (!ReferenceEquals(other, menuItemViewModel))
                {
                    other.IsSelected = false;
                }
            }

            menuItemViewModel.IsSelected = true;


            OnSelectedItemChanged(menuItemViewModel, parameter);
            MenuItemClicked?.Invoke(this, menuItemViewModel);
        }

        protected virtual void OnSelectedItemChanged(MenuItemViewModel menuItemViewModel, object? parameter = null)
        {
        }

        #endregion

        #region 菜单项左图标按钮被点击

        private DelegateCommand<MenuItemViewModel>? _leftIconClickedCommand;

        public DelegateCommand<MenuItemViewModel> LeftIconClickedCommand => _leftIconClickedCommand ??=
            new DelegateCommand<MenuItemViewModel>(LeftIconClicked);

        protected virtual void LeftIconClicked(MenuItemViewModel menuItemViewModel)
        {
        }

        #endregion

        #region 菜单项右图标按钮被点击

        private DelegateCommand<MenuItemViewModel>? _rightIconClickedCommand;

        public DelegateCommand<MenuItemViewModel> RightIconClickedCommand => _rightIconClickedCommand ??=
            new DelegateCommand<MenuItemViewModel>(RightIconClickedExecute);

        private void RightIconClickedExecute(MenuItemViewModel menuItemViewModel)
        {
        }

        #endregion

        #region 菜单项关闭按钮被点击

        private DelegateCommand<MenuItemViewModel>? _closeMenuItemCommand;

        public DelegateCommand<MenuItemViewModel> CloseMenuItemCommand => _closeMenuItemCommand ??=
            new DelegateCommand<MenuItemViewModel>(CloseMenuItemExecute);

        protected virtual void CloseMenuItemExecute(MenuItemViewModel menuItemViewModel)
        {
        }

        #endregion

        #region 菜单被双击

        private DelegateCommand<MenuItemViewModel>? _menuItemLeftDoubleClickCommand;

        public DelegateCommand<MenuItemViewModel> MenuItemLeftDoubleClickCommand => _menuItemLeftDoubleClickCommand ??=
            new DelegateCommand<MenuItemViewModel>(MenuItemLeftDoubleClick);

        protected virtual void MenuItemLeftDoubleClick(MenuItemViewModel menuItemViewModel)
        {
        }

        #endregion

        #region 菜单项被重命名

        private DelegateCommand<object[]>? _keyUpCommand;

        public DelegateCommand<object[]> KeyUpCommand =>
            _keyUpCommand ??= new DelegateCommand<object[]>(KeyUpExecuteAsync);

        private void KeyUpExecuteAsync(object[] values)
        {
            if (values[0] is not MenuItemViewModel menuItemViewModel ||
                values[1] is not KeyEventArgs e) return;


            switch (e.Key)
            {
                case Key.Escape:
                    menuItemViewModel.NewName    = menuItemViewModel.Name;
                    menuItemViewModel.IsRenaming = false;
                    break;

                case Key.Enter when menuItemViewModel.Name == menuItemViewModel.NewName:

                    menuItemViewModel.IsRenaming = false;
                    break;

                case Key.Enter:
                    MenuItemRenamed(menuItemViewModel);
                    break;
            }
        }


        private DelegateCommand<MenuItemViewModel>? _textBoxRenameLostFocusCommand;

        public DelegateCommand<MenuItemViewModel> TextBoxRenameLostFocusCommand => _textBoxRenameLostFocusCommand ??=
            new DelegateCommand<MenuItemViewModel>(TextBoxRenameLostFocus);


        private void TextBoxRenameLostFocus(MenuItemViewModel menuItemViewModel)
        {
            if (menuItemViewModel.NewName != menuItemViewModel.Name)
            {
                MenuItemRenamed(menuItemViewModel);
            }
            else
            {
                menuItemViewModel.IsRenaming = false;
            }
        }

        protected virtual void MenuItemRenamed(MenuItemViewModel menuItemViewModel)
        {
        }

        #endregion

        #region 菜单有内容被拖入

        private DelegateCommand<object[]>? _menuItemDropCommand;

        public DelegateCommand<object[]> MenuItemDropCommand => _menuItemDropCommand ??=
            new DelegateCommand<object[]>(MenuItemDrop);

        private void MenuItemDrop(object[] args)
        {
            if (args[0] is MenuItemViewModel menuItemViewModel &&
                args[1] is DragEventArgs e)
            {
                MenuItemDrop(menuItemViewModel, e);
            }
        }

        protected virtual void MenuItemDrop(MenuItemViewModel menuItemViewModel, DragEventArgs e)
        {
        }

        #endregion
    }
}