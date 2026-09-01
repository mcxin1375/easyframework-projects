using Microsoft.CodeAnalysis;

namespace EasyFramework.UnityRoslyn
{
    public interface IInterfaceHandler
    {
        void Execute(GeneratorExecutionContext context, INamedTypeSymbol classSymbol);
    }
}