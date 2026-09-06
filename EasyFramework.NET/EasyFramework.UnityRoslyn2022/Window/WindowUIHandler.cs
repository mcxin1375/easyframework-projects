using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

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

                var className = classSymbol.Name;
                var typeName = args[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var source = $@"
    public partial class {className}
    {{
        public {typeName} UI {{ get; private set; }} = new();

        public void InitializeUI(UnityEngine.GameObject uiObject)
        {{
            global::EasyFramework.UnityHelper.AutoSetComponents(UI, uiObject);
        }}
    }}
";


                var content = classSymbol.ContainingNamespace.IsGlobalNamespace ? source : Helper.GeneratorNamespaceBodyString(source, classSymbol.ContainingNamespace.ToDisplayString());

                context.AddSource(className + "_WindowUI.g.cs", SourceText.From(content, Encoding.UTF8));
            }
        }
    }
}
