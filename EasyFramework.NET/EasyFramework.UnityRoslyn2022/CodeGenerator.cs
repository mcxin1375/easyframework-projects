using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Collections.Generic;

namespace EasyFramework.UnityRoslyn
{
    [Generator]
    public class CodeGenerator : ISourceGenerator
    {
        private static readonly List<IInterfaceHandler> Handlers = new List<IInterfaceHandler>
                                                                   {
                                                                       //new WindowOpenHandler(),
                                                                       new WindowAwaiterHandler(),
                                                                       new WindowUIHandler(),
                                                                       new TParamsHandler(),
                                                                   };

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new CodeSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = context.SyntaxReceiver as CodeSyntaxReceiver;
            if (receiver == null)
                return;

            foreach (var classDecl in receiver.CandidateClasses)
            {
                var semanticModel = context.Compilation.GetSemanticModel(classDecl.SyntaxTree);

                var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;

                if (classSymbol == null)
                    continue;

                foreach (var handler in Handlers)
                {
                    handler.Execute(context, classSymbol);
                }
            }
        }
    }
}