//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Text;
//using System.Text;
//using System.Linq;

//namespace EasyFramework.UnityRoslyn
//{
//    public class WindowOpenHandler : IInterfaceHandler
//    {
//        public void Execute(GeneratorExecutionContext context, INamedTypeSymbol classSymbol)
//        {
//            foreach (var interfaceSymbol in classSymbol.Interfaces)
//            {
//                if (interfaceSymbol.Name != "IWindowOpen")
//                    continue;

//                var args = interfaceSymbol.TypeArguments;
//                if (args.Length == 0) continue;

//                var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

//                var className = classSymbol.Name;

//                var paramFields = new StringBuilder();
//                var paramList = new StringBuilder();
//                var assignList = new StringBuilder();
//                var objectAssignList = new StringBuilder();


//                for (int i = 0; i < args.Length; i++)
//                {
//                    var typeName = args[i].ToDisplayString();
//                    var index = i + 1;
//                    var arrIndex = i;

//                    paramFields.AppendLine($"        public ref readonly {typeName} TOpen{index} => ref TOpen{index}RW;");
//                    paramFields.AppendLine($"        protected {typeName} TOpen{index}RW;");
//                    paramList.Append($"in {typeName} t{index}");
//                    assignList.AppendLine($"            TOpen{index}RW = t{index};");
//                    objectAssignList.AppendLine($"            TOpen{index}RW = tObjects?.Length > {arrIndex} ? ({typeName})tObjects[{arrIndex}] : default;");


//                    if (i < args.Length - 1)
//                        paramList.Append(", ");
//                }

//                var source =
//$@"
//    public partial class {className}
//    {{
//{paramFields}

//        public void SetOpenParams({paramList})
//        {{
//{assignList}
//        }}

//        public void SetOpenParams(object[] tObjects)
//        {{
//{objectAssignList}
//        }}
//    }}
//";

//                var conent = classSymbol.ContainingNamespace.IsGlobalNamespace ? source : Helper.GeneratorNamespaceBodyString(source, classSymbol.ContainingNamespace.ToDisplayString());

//                context.AddSource(className + "_WindowOpen.g.cs", SourceText.From(conent, Encoding.UTF8));
//            }
//        }
//    }
//}