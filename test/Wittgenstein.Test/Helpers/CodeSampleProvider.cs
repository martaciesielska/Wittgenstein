namespace Wittgenstein.Test.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public static class CodeSampleProvider
    {
        [MethodImpl(MethodImplOptions.NoInlining)]

        internal static string GetBeforeCode()
        {
            var testInfo = GetTestInfo();
            var relativePath = $"{testInfo.Category}\\{testInfo.AnalyzerName}\\{testInfo.MethodName}.cs";
            return GetFile(relativePath);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]

        internal static string GetAfterCode()
        {
            var testInfo = GetTestInfo();
            var relativePath = $"{testInfo.Category}\\{testInfo.AnalyzerName}\\{testInfo.MethodName}_After.cs";
            return GetFile(relativePath);
        }

        internal static string GetRootPath()
        {
            var solutionPath = Directory.CreateDirectory(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            return Path.Combine(solutionPath, "Wittgenstein.CodeSamples");
        }

        private static string GetFile(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);

            return File.ReadAllText(fullPath);
        }

        private static string GetFullPath(string relativePath)
        {
            string testRootPath = GetRootPath();

            var fullPath = Path.Combine(testRootPath, relativePath);
            return fullPath;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static TestInfo GetTestInfo()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(2);

            var method = stackFrame.GetMethod();
            var type = method.DeclaringType;
            var analyzerName = type.Name.Replace("Tests", string.Empty);
            var category = type.Namespace.Split('.').Last();

            return new TestInfo(method.Name, category, analyzerName);
        }

        private sealed class TestInfo
        {
            private readonly string methodName;
            private readonly string category;
            private readonly string analyzerName;

            public TestInfo(string methodName, string category, string analyzerName)
            {
                this.methodName = methodName;
                this.category = category;
                this.analyzerName = analyzerName;
            }

            public string MethodName { get { return this.methodName; } }

            public string Category { get { return this.category; } }

            public string AnalyzerName { get { return this.analyzerName; } }
        }
    }
}