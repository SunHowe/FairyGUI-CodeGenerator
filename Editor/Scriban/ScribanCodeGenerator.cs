using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FairyGUI.CodeGenerator.Core;
using Scriban;
using Scriban.Runtime;

namespace FairyGUI.CodeGenerator.Scriban
{
    public delegate bool GetExportSettings(UIComponent component, out string templatePath, out string outputPath);

    public sealed class ScribanCodeGenerator : IUICodeGenerator
    {
        public GetExportSettings GetExportSettings { get; }
        public Func<string, bool> AcceptNameFilter { get; set; } = IgnoreDefaultName;

        public ScribanCodeGenerator(GetExportSettings getExportSettings)
        {
            GetExportSettings = getExportSettings;
        }

        public void Generate(UIComponent component)
        {
            if (!GetExportSettings(component, out var templatePath, out var outputPath))
                return;

            var template = Template.Parse(File.ReadAllText(templatePath));
            
            var scriptObject = new ScriptObject();
            scriptObject.Import(component);
            scriptObject.Import("upper_first", new Func<string, string>(str =>
            {
                // 将字符串中所有_后的字符转换为大写 然后确保首字母是大写
                var stringBuilder = new StringBuilder();
                var upper = true;
                
                foreach (var c in str)
                {
                    if (c == '_')
                    {
                        upper = true;
                    }
                    else
                    {
                        stringBuilder.Append(upper ? char.ToUpper(c) : c);
                        upper = false;
                    }
                }
                
                return stringBuilder.ToString();
            }));

            scriptObject.Import("is_accept_name", AcceptNameFilter);
            
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);
            
            var output = template.Render(context);
            
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(outputPath, output, Encoding.UTF8);
        }

        private static bool IgnoreDefaultName(string str)
        {
            return !Regex.IsMatch(str, "^[ntc]\\d+$");
        }
    }
}