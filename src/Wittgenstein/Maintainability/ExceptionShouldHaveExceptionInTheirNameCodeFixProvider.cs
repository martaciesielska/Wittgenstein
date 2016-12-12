namespace Wittgenstein
{
    using System;
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
    using System.Text.RegularExpressions;
    using System.Text;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExceptionShouldHaveExceptionInTheirNameCodeFixProvider)), Shared]
    public class ExceptionShouldHaveExceptionInTheirNameCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Add the word 'Exception' to the class name.";
        private const string Title2 = "Replace last word with the word 'Exception' in the class name.";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(ExceptionShouldHaveExceptionInTheirNameAnalyzer.DiagnosticId); }
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
            var classDeclaration = token.Parent as ClassDeclarationSyntax;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => AddExceptionWordToTheName(context.Document, classDeclaration, c),
                    equivalenceKey: Title),
                diagnostic);

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title2,
                    createChangedDocument: c => ReplaceLastWordWithException(context.Document, classDeclaration, c),
                    equivalenceKey: Title2),
                diagnostic);
        }

        private async Task<Document> ReplaceLastWordWithException(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync(c);

            var idToken = classDeclaration.ChildTokens().Single(x => x.Kind() == SyntaxKind.IdentifierToken);
            var oldName = idToken.Text;
            Regex regex = new Regex("([A-Z][a-z]*)");
            var matches = regex.Matches(oldName);

            var endingToRemove = matches.Cast<Match>().Last().Value;
            var newName = idToken.Text.Replace(endingToRemove, string.Empty) + "Exception";

            return GetDocumentWithNewName(document, classDeclaration, root, idToken, newName);
        }

        private async Task<Document> AddExceptionWordToTheName(Document document, ClassDeclarationSyntax classDeclaration, CancellationToken c)
        {
            var root = await document.GetSyntaxRootAsync(c);

            var idToken = classDeclaration.ChildTokens().Single(x => x.Kind() == SyntaxKind.IdentifierToken);
            var newName = idToken.Text + "Exception";

            return GetDocumentWithNewName(document, classDeclaration, root, idToken, newName);
        }

        private static Document GetDocumentWithNewName(Document document, ClassDeclarationSyntax classDeclaration, SyntaxNode root, SyntaxToken idToken, string newName)
        {
            var newClassDeclaration = classDeclaration.ReplaceToken(
                idToken,
                SyntaxFactory.Identifier(idToken.LeadingTrivia, newName, idToken.TrailingTrivia));

            var newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);
            var newDocument = document.WithSyntaxRoot(newRoot);

            return newDocument;
        }
    }
}