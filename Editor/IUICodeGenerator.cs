using FairyGUI.CodeGenerator.Core;

namespace FairyGUI.CodeGenerator
{
    public interface IUICodeGenerator
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        void Generate(UIComponent component);
    }
}