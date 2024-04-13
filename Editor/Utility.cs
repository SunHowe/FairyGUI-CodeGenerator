using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FairyGUI.CodeGenerator
{
    internal static class Utility
    {
        /// <summary>
        /// 获取UIPackage文件名列表
        /// </summary>
        public static List<string> GetUIPackageFileNames(string uiAssetsRoot, string uiByteSuffix)
        {
            var packageFileNames = new List<string>();
            var files = Directory.GetFiles(uiAssetsRoot);
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var file in files)
            {
                if (!file.EndsWith(uiByteSuffix))
                    continue;

                packageFileNames.Add(Path.GetFileName(file));
            }
            
            return packageFileNames;
        }
    }
}