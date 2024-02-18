using System.Collections.ObjectModel;

namespace QuickCAD1.VModels
{
    /// <summary>
    /// 菜单项模型
    /// </summary>
    public class MenuItemVMod
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// 图标地址
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 子菜单项
        /// </summary>
        public ObservableCollection<MenuItemVMod> Children { get; set; }
        public MenuItemVMod()
        {
            Children = new ObservableCollection<MenuItemVMod>();
        }
    }
}
