namespace Wittgenstein.CodeSamples.Maintainability
{
    using System;

    public class RethrowTheSameExceptionObject
    {
        public void DoSomething()
        {
            try
            {
            }
            catch (Exception)
            {
                Console.WriteLine("Some logging logic...");
                throw;
            }
        }
    }
}
