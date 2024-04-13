using System;
using System.Linq;

namespace FairyGUI.CodeGenerator
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    public static class UICodeGenerator
    {
        /// <summary>
        /// 生成代码
        /// </summary>
        public static void Generate(string uiAssetsRoot, string uiByteSuffix, IUICodeGenerator generator, IUIComponentFilter filter)
        {
            var components = UIComponentCollector.Collect(uiAssetsRoot, uiByteSuffix);

            foreach (var component in components)
                component.DistTypeFullName = filter.Filter(component);
            
            // 更新RefTypeFullName
            var dict = components.ToDictionary(component => component.URL);
            
            foreach (var node in components.SelectMany(component => component.Nodes))
            {
                if (node.ObjectType == ObjectType.Component)
                {
                    // 修正Component类型
                    if (dict.TryGetValue(node.Ref, out var refComponent))
                        node.ObjectType = refComponent.ObjectType;
                }
                
                switch (node.ObjectType)
                {
                    case ObjectType.Image:
                        node.RefTypeFullName = typeof(GImage).FullName;
                        break;
                    case ObjectType.MovieClip:
                        node.RefTypeFullName = typeof(GMovieClip).FullName;
                        break;
                    case ObjectType.Graph:
                        node.RefTypeFullName = typeof(GGraph).FullName;
                        break;
                    case ObjectType.Loader:
                        node.RefTypeFullName = typeof(GLoader).FullName;
                        break;
                    case ObjectType.Group:
                        node.RefTypeFullName = typeof(GGroup).FullName;
                        break;
                    case ObjectType.Text:
                        node.RefTypeFullName = typeof(GTextField).FullName;
                        break;
                    case ObjectType.RichText:
                        node.RefTypeFullName = typeof(GRichTextField).FullName;
                        break;
                    case ObjectType.InputText:
                        node.RefTypeFullName = typeof(GTextInput).FullName;
                        break;
                    case ObjectType.List:
                        node.RefTypeFullName = typeof(GList).FullName;
                        break;
                    case ObjectType.Component:
                    {
                        node.RefTypeFullName = dict.TryGetValue(node.Ref, out var refComponent) ? refComponent.DistTypeFullName : typeof(GComponent).FullName;
                        break;
                    }
                    case ObjectType.Label:
                    {
                        node.RefTypeFullName = dict.TryGetValue(node.Ref, out var refComponent) ? refComponent.DistTypeFullName : typeof(GLabel).FullName;
                        break;
                    }
                    case ObjectType.Button:
                    {
                        node.RefTypeFullName = dict.TryGetValue(node.Ref, out var refComponent) ? refComponent.DistTypeFullName : typeof(GButton).FullName;
                        break;
                    }
                    case ObjectType.ComboBox:
                    {
                        node.RefTypeFullName = dict.TryGetValue(node.Ref, out var refComponent) ? refComponent.DistTypeFullName : typeof(GComboBox).FullName;
                        break;
                    }
                    case ObjectType.ProgressBar:
                    {
                        node.RefTypeFullName = dict.TryGetValue(node.Ref, out var refComponent) ? refComponent.DistTypeFullName : typeof(GProgressBar).FullName;
                        break;
                    }
                    case ObjectType.Slider:
                    {
                        node.RefTypeFullName = dict.TryGetValue(node.Ref, out var refComponent) ? refComponent.DistTypeFullName : typeof(GSlider).FullName;
                        break;
                    }
                    case ObjectType.ScrollBar:
                    {
                        node.RefTypeFullName = dict.TryGetValue(node.Ref, out var refComponent) ? refComponent.DistTypeFullName : typeof(GScrollBar).FullName;
                        break;
                    }
                    case ObjectType.Tree:
                        node.RefTypeFullName = typeof(GTree).FullName;
                        break;
                    case ObjectType.Loader3D:
                        node.RefTypeFullName = typeof(GLoader3D).FullName;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var component in components)
            {
                if (component.DistTypeFullName == component.ExtensionType.FullName)
                    continue;

                generator.Generate(component);
            }
        }
    }
}