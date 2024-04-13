using System;
using System.Collections.Generic;

namespace FairyGUI.CodeGenerator.Core
{
    /// <summary>
    /// UI组件
    /// </summary>
    public class UIComponent
    {
        public string URL { get; set; }
        public string PackageName { get; set; }
        public string Name { get; set; }
        public bool IsExported { get; set; }
        public ObjectType ObjectType { get; set; }
        public Type ExtensionType { get; set; }
        public List<UIComponentNode> Nodes { get; set; }
        public List<UIController> Controllers { get; set; }
        public List<UITransition> Transitions { get; set; }
        
        /// <summary>
        /// 目标类型名 若该值与ExtendType的FullName相同 则不生成代码
        /// </summary>
        public string DistTypeFullName { get; set; }
    }
}