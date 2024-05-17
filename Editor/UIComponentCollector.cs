using System;
using System.Collections.Generic;
using FairyGUI.CodeGenerator.Core;
using UnityEditor;
using UnityEngine;

namespace FairyGUI.CodeGenerator
{
    /// <summary>
    /// FairyGUI组件收集器
    /// </summary>
    public static class UIComponentCollector
    {
        public static List<UIComponent> Collect(string uiAssetsRoot, string uiByteSuffix)
        {
            AddPackages(uiAssetsRoot, uiByteSuffix);

            var components = new List<UIComponent>();
            foreach (var package in UIPackage.GetPackages())
                components.AddRange(LoadFromPackage(package));

            UIPackage.RemoveAllPackages();
            return components;
        }

        private static List<UIComponent> LoadFromPackage(UIPackage package)
        {
            var components = new List<UIComponent>();

            foreach (var packageItem in package.GetItems())
            {
                if (packageItem.type != PackageItemType.Component)
                    continue;

                components.Add(LoadFromPackageItem(packageItem));
            }

            return components;
        }

        private static UIComponent LoadFromPackageItem(PackageItem item)
        {
            return new UIComponent
            {
                URL = FormatURL(item.owner.id, item.id),
                PackageName = item.owner.name,
                Name = item.name,
                IsExported = item.exported,
                ObjectType = item.objectType,
                Nodes = LoadComponentNodes(item),
                Controllers = LoadControllers(item),
                Transitions = LoadTransitions(item),
                ExtensionType = item.objectType switch
                {
                    ObjectType.Label => typeof(GLabel),
                    ObjectType.Button => typeof(GButton),
                    ObjectType.ComboBox => typeof(GComboBox),
                    ObjectType.ProgressBar => typeof(GProgressBar),
                    ObjectType.Slider => typeof(GSlider),
                    ObjectType.ScrollBar => typeof(GScrollBar),

                    ObjectType.Component => typeof(GComponent),

                    _ => throw new ArgumentOutOfRangeException()
                }
            };
        }

        private static List<UIComponentNode> LoadComponentNodes(PackageItem item)
        {
            var nodes = new List<UIComponentNode>();

            var buffer = item.rawData;
            buffer.Seek(0, 2);

            int childCount = buffer.ReadShort();
            for (var i = 0; i < childCount; i++)
            {
                int dataLen = buffer.ReadShort();
                var curPos = buffer.position;

                buffer.Seek(curPos, 0);

                var objectType = (ObjectType)buffer.ReadByte();
                var src = buffer.ReadS();
                var pkgId = buffer.ReadS() ?? item.owner.id;

                buffer.Seek(curPos, 0);
                buffer.Skip(5);

                buffer.ReadS();
                var name = buffer.ReadS();
                buffer.position = curPos + dataLen;

                nodes.Add(new UIComponentNode
                {
                    Index = i,
                    Name = name,
                    ObjectType = objectType,
                    Ref = !string.IsNullOrEmpty(src) ? FormatURL(pkgId, src) : string.Empty,
                });
            }

            return nodes;
        }

        private static List<UIController> LoadControllers(PackageItem item)
        {
            var controllers = new List<UIController>();
            var buffer = item.rawData;

            buffer.Seek(0, 1);

            int controllerCount = buffer.ReadShort();
            for (var i = 0; i < controllerCount; i++)
            {
                int nextPos = buffer.ReadShort();
                nextPos += buffer.position;

                var beginPos = buffer.position;
                buffer.Seek(beginPos, 0);

                var name = buffer.ReadS();
                var pages = new List<string>();

                buffer.Seek(beginPos, 1);

                var pageCount = buffer.ReadShort();
                for (var j = 0; j < pageCount; j++)
                {
                    var pageId = buffer.ReadS(); // pageId
                    var packageName = buffer.ReadS();

                    if (string.IsNullOrEmpty(packageName))
                        packageName = pageId;

                    pages.Add(packageName);
                }

                buffer.position = nextPos;

                controllers.Add(new UIController
                {
                    Index = i,
                    Name = name,
                    Pages = pages,
                });
            }

            return controllers;
        }

        private static List<UITransition> LoadTransitions(PackageItem item)
        {
            var transitions = new List<UITransition>();

            var buffer = item.rawData;
            buffer.Seek(0, 5);

            int transitionCount = buffer.ReadShort();
            for (var i = 0; i < transitionCount; i++)
            {
                int nextPos = buffer.ReadShort();
                nextPos += buffer.position;

                var name = buffer.ReadS();

                buffer.position = nextPos;

                transitions.Add(new UITransition
                {
                    Index = i,
                    Name = name
                });
            }

            return transitions;
        }

        private static void AddPackages(string uiAssetsRoot, string uiByteSuffix)
        {
            UIPackage.RemoveAllPackages();

            if (string.IsNullOrEmpty(uiAssetsRoot))
                return;

            var fileNames = Utility.GetUIPackageFileNames(uiAssetsRoot, uiByteSuffix);
            foreach (var name in fileNames)
            {
                var assetPath = uiAssetsRoot + "/" + name;
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (asset == null)
                    continue;

                UIPackage.AddPackage(asset.bytes, string.Empty, null);
            }
        }

        private static string FormatURL(string packageId, string objectId)
        {
            return $"{UIPackage.URL_PREFIX}{packageId}{objectId}";
        }
    }
}