namespace Wittgenstein.Maintainability
{
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RethrowAndKeepStackTraceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIdProvider.RethrowAndKeepStackTrace;

        private static readonly string Title = "Stack is reset when exception is thrown."; 
        private static readonly string MessageFormat = "Exception '{0}' is rethrown with an explicit reference to the exception object.";
        private static readonly string Description = "Exception should be rethrown referencing the exception implictly."; 
        private const string Category = "Maintainability";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckIfRethrowPreservesTheStack, SyntaxKind.CatchClause);
        }

        private static void CheckIfRethrowPreservesTheStack(SyntaxNodeAnalysisContext context)
        {
            var catchStatement = context.Node as CatchClauseSyntax;
            if (catchStatement == null || catchStatement.Declaration == null || catchStatement.Block == null) { return; }

            var identifier = catchStatement.Declaration.Identifier.Text;
            if (string.IsNullOrEmpty(identifier)) { return; }

            var throwStatements = catchStatement.Block.DescendantNodes()
                .OfType<ThrowStatementSyntax>();

            foreach (var throwStatement in throwStatements)
            {
                if (throwStatement == null) continue;

                var thrownIdentifierNode = throwStatement.ChildNodes()
                    .OfType<IdentifierNameSyntax>()
                    .SingleOrDefault();

                if (thrownIdentifierNode == null) continue;

                var thrownIdentifier = thrownIdentifierNode.GetFirstToken().Text;

                if (identifier == thrownIdentifier)
                {
                    var diagnostic = Diagnostic.Create(Rule, thrownIdentifierNode.GetLocation(), identifier);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
