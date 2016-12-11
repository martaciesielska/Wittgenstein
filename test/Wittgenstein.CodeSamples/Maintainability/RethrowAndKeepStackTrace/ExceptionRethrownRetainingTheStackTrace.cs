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
            catch (Exception ex)
            {
                Console.WriteLine("Some logging logic... {0}", ex.Message);
                throw;
            }
        }
    }
}
