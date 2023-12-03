using Prism.Mvvm;
using System.Collections.Generic;

namespace WpfMenus;

public class MenuItemViewModel : BindableBase, ISelectableItem
{
    public MenuItemViewModel()
    {
        Children = new SelectableNotificationList<MenuItemViewModel>();
    }

    /// <summary>
    /// 菜单类型,一用于识别菜单项
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 工作负载,可存放一个数据
    /// </summary>
    public object Payload { get; set; }

    /// <summary>
    /// 参数,一个临时放数据的位置.
    /// </summary>
    public object Parameter { get; set; }

    /// <summary>
    /// 是否显示收缩/展开按钮
    /// </summary>
    public bool IsExpandButtonVisible => CanUserExpand && Children.AnyItem;

    /// <summary>
    /// 用户是否有权控制收缩/展开
    /// </summary>
    public bool CanUserExpand { get; set; } = true;

    /// <summary>
    /// 当前是否是展开的
    /// </summary>
    public bool IsExpanded { get; set; }

    /// <summary>
    /// 菜单名称(标题)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 是否可选中
    /// </summary>
    public bool IsSelectable { get; set; } = true;

    /// <summary>
    /// 当前是否选中
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 是否加重(粗体)
    /// </summary>
    public bool IsAccent { get; set; }

    /// <summary>
    /// 是否允许拖入内容
    /// </summary>
    public bool IsAllowDrop { get; set; }

    /// <summary>
    /// 是否可用
    /// </summary>
    public bool IsEnabled { get; set; } = true;


    /// <summary>
    /// 鼠标提示
    /// </summary>
    public object ToolTip { get; set; }

    /// <summary>
    /// 是否可关闭
    /// </summary>
    public bool CanClose { get; set; }


    public string TemplateKey { get; set; }


    #region 左图标

    /// <summary>
    /// 做图标的 Path 数据
    /// </summary>
    public string LeftIcon { get; set; }


    /// <summary>
    /// 左图标是否是一个按钮
    /// </summary>
    public bool IsLeftIconAButton { get; set; } = false;

    #endregion

    #region 右图标

    public string RightIcon { get; set; }

    /// <summary>
    /// 右图标是否是一个按钮
    /// </summary>
    public bool IsRightIconAButton { get; set; } = false;

    public bool IsVisible { get; set; } = true;

    #endregion

    #region 重命名

    public string NewName { get; set; }

    public bool IsRenaming { get; set; }

    #endregion

    #region 子菜单

    public SelectableNotificationList<MenuItemViewModel> Children { get; init; }

    /// <summary>
    /// 缩进
    /// </summary>
    public int ChildrenIndentation { get; set; }

    public string ItemTemplateKey { get; set; } = "DefaultChildrenMenuItemsTemplate";


    public IEnumerable<MenuItemViewModel> Descendants()
    {
        foreach (var menuItemViewModel in Children)
        {
            yield return menuItemViewModel;

            foreach (var child in menuItemViewModel.Descendants())
            {
                yield return child;
            }
        }
    }

    #endregion
}

public enum MenuIconTypes
{
    IconFont,
    ImageSource,
}