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
                if (ex is ArgumentException)
                {
                    Console.WriteLine("It's an argument exception.", ex.Message);
                    throw;
                }

                if (ex is ArgumentNullException)
                {
                    Console.WriteLine("It's an argument null exception.", ex.Message);
                    throw ex;
                }

                if (ex is FormatException)
                {
                    Console.WriteLine("It's an argument null exception.", ex.Message);
                    throw new WrapperException(ex);
                }

                Console.WriteLine("Some logging logic... {0}", ex.Message);
                throw ex;
            }
        }
    }
}
