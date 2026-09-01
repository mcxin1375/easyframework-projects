using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Linq;

namespace EasyFramework.UnityRoslyn
{
    public class WindowUIHandler : IInterfaceHandler
    {
        public void Execute(GeneratorExecutionContext context, INamedTypeSymbol classSymbol)
        {
            foreach (var interfaceSymbol in classSymbol.Interfaces)
            {
                if (interfaceSymbol.Name != "IWindowUI")
                    continue;

                var args = interfaceSymbol.TypeArguments;
                if (args.Length == 0) continue;

                var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

                var className = classSymbol.Name;

                var paramFields = new StringBuilder();

                for (int i = 0; i < args.Length; i++)
                {
                    var typeName = args[i].ToDisplayString();
                    var index = i + 1;

                    paramFields.AppendLine($"        public {typeName} UI {{ get; private set; }}");
                }

                string source = string.Empty;
                //                var source =
                //$@"
                //    public partial class {className}
                //    {{
                //{paramFields}

                //        public void InitializeUI(UnityEngine.GameObject uiObject)
                //        {{
                //            UI ??= new();
                //            EasyFramework.UnityHelper.AutoSetComponents(UI, uiObject);
                //        }}
                //    }}
                //";
                var interfaceNamespace = interfaceSymbol.ContainingNamespace.ToDisplayString();
                if (interfaceNamespace == "EasyFramework")
                {
                    source =
$@"
    public partial class {className}
    {{
{paramFields}

        public void InitializeUI(UnityEngine.GameObject uiObject)
        {{
            UI ??= new();
            EasyFramework.UnityHelper.AutoSetComponents(UI, uiObject);
        }}
    }}
";
                }
                else
                {
                    source =
$@"
    public partial class {className}
    {{
{paramFields}

        public void InitializeUI(UnityEngine.GameObject uiObject)
        {{
            UI ??= new();
            EasyFramework.AOT.UnityHelper.AutoSetComponents(UI, uiObject);
        }}
    }}
";
                }


                var conent = classSymbol.ContainingNamespace.IsGlobalNamespace ? source : Helper.GeneratorNamespaceBodyString(source, classSymbol.ContainingNamespace.ToDisplayString());

                context.AddSource(className + "_WindowUI.g.cs", SourceText.From(conent, Encoding.UTF8));
            }
        }
    }
}