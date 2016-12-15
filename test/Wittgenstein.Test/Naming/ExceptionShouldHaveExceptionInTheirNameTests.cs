namespace Wittgenstein.Test.Naming
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using Wittgenstein.Naming;
    using Wittgenstein.Test.Helpers;
    using Wittgenstein.Test.Verifiers;

    [TestClass]
    public class ExceptionShouldHaveExceptionInTheirNameTests : CodeFixVerifier
    {

        [TestMethod]
        public void NoCodeNoSquiggle()
        {
            var before = string.Empty;
            this.VerifyDiagnostic(before);
        }

        [TestMethod]
        public void ExceptionNameIsCorrect()
        {
            var before = CodeSampleProvider.GetBeforeCode();
            this.VerifyDiagnostic(before);
        }

        [TestMethod]
        public void NotAnException()
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
                Id = ExceptionShouldHaveExceptionInTheirNameAnalyzer.DiagnosticId,
                Message = string.Format("Exception name 'HoustonWeHaveAProblem' should contain the word \"Exception\".", "ex"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 18)
                        }
            };

            this.VerifyDiagnostic(actual, expected);

            var afterTheFix = CodeSampleProvider.GetAfterCode();
            this.VerifyFix(actual, afterTheFix);
        }

        [TestMethod]
        public void DiagnosticTriggered2()
        {
            var actual = CodeSampleProvider.GetBeforeCode();

            var expected = new DiagnosticResult
            {
                Id = ExceptionShouldHaveExceptionInTheirNameAnalyzer.DiagnosticId,
                Message = string.Format("Exception name 'HoustonWeHaveAProblem' should contain the word \"Exception\".", "ex"),
                Severity = DiagnosticSeverity.Warning,
                Locations = new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 5, 18)
                        }
            };

            this.VerifyDiagnostic(actual, expected);

            var afterTheFix = CodeSampleProvider.GetAfterCode();
            this.VerifyFix(actual, afterTheFix, 1);
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new ExceptionShouldHaveExceptionInTheirNameCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ExceptionShouldHaveExceptionInTheirNameAnalyzer();
        }
    }
}