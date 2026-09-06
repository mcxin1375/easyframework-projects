using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace EasyFramework.UnityRoslyn
{
    public class CodeSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses = new List<ClassDeclarationSyntax>();
        public List<TypeDeclarationSyntax> CandidateTypes = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            var typeDecl = syntaxNode as TypeDeclarationSyntax;
            if (typeDecl != null)
                CandidateTypes.Add(typeDecl);

            var classDecl = syntaxNode as ClassDeclarationSyntax;

            if (classDecl == null)
                return;

            if (classDecl.BaseList == null)
                return;

            CandidateClasses.Add(classDecl);
        }
    }
}
