using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EasyFramework.UnityRoslyn
{
    public class WindowAwaiterHandler : IInterfaceHandler
    {
        public void Execute(
            GeneratorExecutionContext context,
            INamedTypeSymbol classSymbol)
        {
            foreach (var interfaceSymbol in classSymbol.Interfaces)
            {
                if (interfaceSymbol.Name != "IWindowAwaiter")
                    continue;

                if (interfaceSymbol.TypeArguments.Length != 1)
                    continue;

                var resultType = interfaceSymbol.TypeArguments[0];

                var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

                var className = classSymbol.Name;

                var resultTypeName = resultType.ToDisplayString();

                var source =
$@"
    using System.Threading.Tasks;
 
    public partial class {className}
    {{
        public bool TrySetResult(in {resultTypeName} result)
            => EasyFramework.WindowAwaiter<{resultTypeName}>.TrySetResult(result);

        public Task<{resultTypeName}> WaitResultAsync()
            => EasyFramework.WindowAwaiter<{resultTypeName}>.GetAwaiter();
    }}
";
                var conent = classSymbol.ContainingNamespace.IsGlobalNamespace ? source : Helper.GeneratorNamespaceBodyString(source, classSymbol.ContainingNamespace.ToDisplayString());

                context.AddSource(className + "_WindowAwaiter.g.cs", SourceText.From(conent, Encoding.UTF8));
            }
        }
    }
}