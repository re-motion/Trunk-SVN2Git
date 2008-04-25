using System;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class GenericClassWithNested<T>
  {
    public class Nested
    {
      public T T;
    }
  }
}