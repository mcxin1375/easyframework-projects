using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyFramework.UnityRoslyn
{
    internal sealed class ReflectionAttributeHandler
    {
        private const string ReflectionAttributeMetadataName = "EasyFramework.ReflectionAttribute";
        private const string AttributeMetadataName = "System.Attribute";

        public void Execute(GeneratorExecutionContext context, CodeSyntaxReceiver receiver)
        {
            var reflectionAttribute = context.Compilation.GetTypeByMetadataName(ReflectionAttributeMetadataName);
            var attributeBaseType = context.Compilation.GetTypeByMetadataName(AttributeMetadataName);
            if (reflectionAttribute == null || attributeBaseType == null)
                return;

            var sourceTypes = GetSourceTypes(context, receiver);
            foreach (var reflectionType in sourceTypes)
            {
                if (!IsAttributeMode(reflectionType, reflectionAttribute) ||
                    reflectionType.TypeKind != TypeKind.Class ||
                    !InheritsFrom(reflectionType, attributeBaseType))
                    continue;

                Generate(context, reflectionType, sourceTypes);
            }
        }

        private static List<INamedTypeSymbol> GetSourceTypes(
            GeneratorExecutionContext context,
            CodeSyntaxReceiver receiver)
        {
            var result = new List<INamedTypeSymbol>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (var syntax in receiver.CandidateTypes)
            {
                var semanticModel = context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = semanticModel.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                if (symbol == null)
                    continue;

                var name = GetTypeName(symbol);
                if (seen.Add(name))
                    result.Add(symbol);
            }

            return result;
        }

        private static bool IsAttributeMode(INamedTypeSymbol typeSymbol, INamedTypeSymbol reflectionAttribute)
        {
            foreach (var attribute in typeSymbol.GetAttributes())
            {
                if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, reflectionAttribute) ||
                    attribute.ConstructorArguments.Length != 1)
                    continue;

                return attribute.ConstructorArguments[0].Value is int mode && mode == 1;
            }

            return false;
        }

        private static void Generate(
            GeneratorExecutionContext context,
            INamedTypeSymbol reflectionType,
            IReadOnlyList<INamedTypeSymbol> sourceTypes)
        {
            var targets = sourceTypes
                .Where(type => IsAccessible(type) && HasDirectAttribute(type, reflectionType))
                .OrderBy(GetTypeName, StringComparer.Ordinal)
                .ToList();

            var instanceTargets = targets
                .Where(CanCreateWithActivator)
                .ToList();

            var source = new StringBuilder();
            source.AppendLine("using System;");
            source.AppendLine();
            var namespaceName = reflectionType.ContainingNamespace.ToDisplayString();
            if (!string.IsNullOrEmpty(namespaceName))
            {
                source.AppendLine("namespace " + namespaceName);
                source.AppendLine("{");
            }
            source.Append("    public partial class ");
            source.Append(reflectionType.Name);
            source.AppendLine(" : global::EasyFramework.IReflection");
            source.AppendLine("    {");
            AppendProperty(source, "Types", targets);
            AppendProperty(source, "InstanceTypes", instanceTargets);
            source.AppendLine("    }");
            if (!string.IsNullOrEmpty(namespaceName))
                source.AppendLine("}");

            context.AddSource(
                GetHintName(reflectionType),
                SourceText.From(source.ToString(), Encoding.UTF8));
        }

        private static void AppendProperty(
            StringBuilder source,
            string propertyName,
            IEnumerable<INamedTypeSymbol> types)
        {
            source.Append("        public Type[] ");
            source.Append(propertyName);
            source.AppendLine(" { get; } = new Type[]");
            source.AppendLine("        {");
            foreach (var type in types)
            {
                source.Append("            typeof(");
                source.Append(GetTypeName(type));
                source.AppendLine("),");
            }
            source.AppendLine("        };");
            source.AppendLine();
        }

        private static bool HasDirectAttribute(INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeType)
        {
            foreach (var attribute in typeSymbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType))
                    return true;
            }

            return false;
        }

        private static bool CanCreateWithActivator(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind != TypeKind.Class || typeSymbol.IsAbstract || typeSymbol.TypeParameters.Length != 0)
                return false;

            foreach (var constructor in typeSymbol.InstanceConstructors)
            {
                if (constructor.Parameters.Length == 0 &&
                    constructor.DeclaredAccessibility == Accessibility.Public)
                    return true;
            }

            return false;
        }

        private static bool IsAccessible(INamedTypeSymbol typeSymbol)
        {
            for (var current = typeSymbol; current != null; current = current.ContainingType)
            {
                if (current.DeclaredAccessibility != Accessibility.Public &&
                    current.DeclaredAccessibility != Accessibility.Internal)
                    return false;
            }

            return true;
        }

        private static bool InheritsFrom(INamedTypeSymbol typeSymbol, INamedTypeSymbol baseType)
        {
            for (var current = typeSymbol.BaseType; current != null; current = current.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(current, baseType))
                    return true;
            }

            return false;
        }

        private static string GetTypeName(INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        private static string GetHintName(INamedTypeSymbol typeSymbol)
        {
            var namespaceName = typeSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : typeSymbol.ContainingNamespace.ToDisplayString() + ".";
            return namespaceName + typeSymbol.Name + ".Reflection.g.cs";
        }
    }
}
