namespace Wittgenstein
{
    public static class DiagnosticIdProvider
    {
        public const string RethrowAndKeepStackTrace = "WITTG0001";
        public const string ExceptionShouldHaveExceptionInTheirName = "WITTG0002";
        public const string ExceptionShouldImplementRecommendedConstructors = "WITTG0003";
        public const string MethodShouldHaveLessThanEightParameters = "WITTG0004";
    }
}
