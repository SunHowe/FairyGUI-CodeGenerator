using FairyGUI.CodeGenerator.Core;

namespace FairyGUI.CodeGenerator
{
    /// <summary>
    /// UI组件过滤器
    /// </summary>
    public interface IUIComponentFilter
    {
        /// <summary>
        /// 过滤 返回组件的类型全名 若返回的类型名与component的ExtendType相同 则不生成代码
        /// </summary>
        string Filter(UIComponent component);
    }
}