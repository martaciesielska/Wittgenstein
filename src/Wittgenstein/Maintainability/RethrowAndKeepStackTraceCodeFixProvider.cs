namespace Wittgenstein
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RethrowAndKeepStackTraceCodeFixProvider)), Shared]
    public class RethrowAndKeepStackTraceCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Rethrow retaining the original stack trace";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(RethrowAndKeepStackTraceAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var token = root.FindToken(diagnosticSpan.Start);
            var throwStatement = token.Parent.AncestorsAndSelf().OfType<ThrowStatementSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => FixRethrow(context.Document, throwStatement, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private async Task<Document> FixRethrow(Document document, ThrowStatementSyntax throwStatement, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync(c);

            var newThrowStatement = RemoveIdentifier(throwStatement);

            var newRoot = root.ReplaceNode(throwStatement, newThrowStatement);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }

        private ThrowStatementSyntax RemoveIdentifier(ThrowStatementSyntax throwStatement)
        {
            var identifierNameNode = throwStatement.ChildNodes()
                .OfType<IdentifierNameSyntax>()
                .SingleOrDefault();

            var newThrowStatement = throwStatement
                .RemoveNode(identifierNameNode, SyntaxRemoveOptions.KeepNoTrivia);

            var throwToken = newThrowStatement.ChildTokens().FirstOrDefault();

            return newThrowStatement
                .ReplaceToken(throwToken, SyntaxFactory.Token(SyntaxKind.ThrowKeyword).WithLeadingTrivia(throwToken.LeadingTrivia));
        }
    }
}