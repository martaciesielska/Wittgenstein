namespace Wittgenstein.Test.Maintainability
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Wittgenstein.Maintainability;
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
            var actual = CodeSampleProvider.GetBeforeCode();

            var expected = new DiagnosticResult
            {
                Id = RethrowAndKeepStackTraceAnalyzer.DiagnosticId,
                Message = "Exception 'ex' is rethrown with an explicit reference to the exception object.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 15, 23)
                        }
            };

            this.VerifyDiagnostic(actual, expected);

            var afterTheFix = CodeSampleProvider.GetAfterCode();
            this.VerifyFix(actual, afterTheFix);
        }

        [TestMethod]
        public void DiagnosticTriggeredMultipleRethrows()
        {
            var actual = CodeSampleProvider.GetBeforeCode();

            var expected = new[] 
            {
                new DiagnosticResult
                {
                    Id = RethrowAndKeepStackTraceAnalyzer.DiagnosticId,
                    Message = "Exception 'ex' is rethrown with an explicit reference to the exception object.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 23, 27) }
                },
                new DiagnosticResult
                {
                    Id = RethrowAndKeepStackTraceAnalyzer.DiagnosticId,
                    Message = "Exception 'ex' is rethrown with an explicit reference to the exception object.",
                    Severity = DiagnosticSeverity.Warning,
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 33, 23) }
                }
            };

            this.VerifyDiagnostic(actual, expected);

            var afterTheFix = CodeSampleProvider.GetAfterCode();
            this.VerifyFix(actual, afterTheFix);
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