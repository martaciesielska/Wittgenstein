namespace Wittgenstein.Test.Helpers
{
    using System.Linq;
    using Microsoft.CodeAnalysis;

    internal static class DocumentFactory
    {
        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string</param>
        /// <returns>A Document created from the source string</returns>
        public static Document CreateDocument(string source)
        {
            return ProjectFactory.CreateProject(new[] { source }).Documents.First();
        }
    }
}
