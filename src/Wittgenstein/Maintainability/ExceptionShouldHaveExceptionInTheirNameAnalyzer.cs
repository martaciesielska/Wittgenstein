namespace Wittgenstein
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionShouldHaveExceptionInTheirNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIdProvider.ExceptionShouldHaveExceptionInTheirName;

        private static readonly string Title = "Exception name should contain the word exception."; 
        private static readonly string MessageFormat = "Exception name '{0}' should contain the word \"Exception\".";
        private static readonly string Description = "Exception name should contain the word exception."; 
        private const string Category = "Maintainability";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(CheckIfExceptionIsNamedCorrectly, SymbolKind.NamedType);
        }

        private void CheckIfExceptionIsNamedCorrectly(SymbolAnalysisContext context)
        {
            var typeSymbol = context.Symbol as INamedTypeSymbol;
            if (typeSymbol == null || typeSymbol.TypeKind != TypeKind.Class) return;

            if (InheritsFrom(typeSymbol, typeof(Exception)) && !typeSymbol.Name.EndsWith("Exception"))
            {
                var location = typeSymbol.Locations.First();
                var diagnostic = Diagnostic.Create(Rule, location, typeSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool InheritsFrom(INamedTypeSymbol typeSymbol, Type type)
        {
            if (typeSymbol.BaseType is IErrorTypeSymbol) return false;
            if (type == typeof(object)) return true;

            if (typeSymbol.BaseType.SpecialType == SpecialType.System_Object) return false;

            if (typeSymbol.BaseType.ToDisplayString() == type.FullName) return true;
            return InheritsFrom(typeSymbol.BaseType, type);
        }
    }
}
