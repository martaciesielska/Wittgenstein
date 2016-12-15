namespace Wittgenstein.Naming
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ExceptionShouldHaveExceptionInTheirNameAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIdProvider.ExceptionShouldHaveExceptionInTheirName;

        private static readonly string Title = "Exception name should contain the word exception."; 
        private static readonly string MessageFormat = "Exception name '{0}' should contain the word \"Exception\".";
        private static readonly string Description = "Exception name should contain the word exception."; 
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(CheckIfExceptionIsNamedCorrectly, SymbolKind.NamedType);
        }

        private void CheckIfExceptionIsNamedCorrectly(SymbolAnalysisContext context)
        {
            var typeSymbol = context.Symbol as INamedTypeSymbol;
            var exceptionSymbol = context.Compilation.GetTypeByMetadataName("System.Exception");

            if (InheritsFrom(typeSymbol, exceptionSymbol) && !typeSymbol.Name.Contains("Exception"))
            {
                var diagnostic = Diagnostic.Create(Rule, typeSymbol.Locations.First(), typeSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool InheritsFrom(INamedTypeSymbol child, INamedTypeSymbol ancestor)
        {
            if (child.BaseType.SpecialType == SpecialType.System_Object)
            {
                return false;
            }

            if (child.BaseType.Equals(ancestor))
            {
                return true;
            }

            return InheritsFrom(child.BaseType, ancestor);
        }
    }
}