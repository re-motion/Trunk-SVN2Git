using System;

namespace Remotion.UnitTests.Configuration
{
  public class ConstructorException : Exception
  {
    public ConstructorException (string message)
        : base (message)
    {
    }
  }
}