using System;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class ClassWithProtectedMethod
  {
    protected string GetSecret ()
    {
      return "The secret is to be more provocative and interesting than anything else in [the] environment.";
    }
  }
}