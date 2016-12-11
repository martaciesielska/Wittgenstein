namespace Wittgenstein.Test.Maintainability
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Wittgenstein.Test.Helpers;
    using Wittgenstein.Test.Verifiers;

    [TestClass]
    public class RethrowAndKeepStackTraceTests : CodeFixVerifier
    {

        [TestMethod]
        public void NoCodeNoSquiggle()
        {
            var before = string.Empty;
            this.VerifyDiagnostic(before);
        }

        [TestMethod]
        public void NoExceptionIdDoesntTriggerAnalyzer()
        {
            var before = CodeSampleProvider.GetBeforeCode();
            this.VerifyDiagnostic(before);
        }

        [TestMethod]
        public void ExceptionRethrownRetainingTheStackTrace()
        {
            var before = CodeSampleProvider.GetBeforeCode();
            this.VerifyDiagnostic(before);
        }

        [TestMethod]
        public void NewExceptionOfDifferentType()
        {
            var before = CodeSampleProvider.GetBeforeCode();
            this.VerifyDiagnostic(before);
        }

        [TestMethod]
        public void DiagnosticTriggered()
        {
            var test = CodeSampleProvider.GetBeforeCode();

            var expected = new DiagnosticResult
            {
                Id = RethrowAndKeepStackTraceAnalyzer.DiagnosticId,
                Message = string.Format("Exception '{0}' is rethrown with an explicit reference to the exception object.", "ex"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 15, 23)
                        }
            };

            this.VerifyDiagnostic(test, expected);

            var fixtest = CodeSampleProvider.GetAfterCode();
            this.VerifyFix(test, fixtest);
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new RethrowAndKeepStackTraceCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new RethrowAndKeepStackTraceAnalyzer();
        }
    }
}