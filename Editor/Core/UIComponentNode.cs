namespace FairyGUI.CodeGenerator.Core
{
    /// <summary>
    /// UI组件的子节点
    /// </summary>
    public class UIComponentNode
    {
        public string Name { get; set; }
        public ObjectType ObjectType { get; set; }
        public string Ref { get; set; }
        
        public string RefTypeFullName { get; set; }
    }
}