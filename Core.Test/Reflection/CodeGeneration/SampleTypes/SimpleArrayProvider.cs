namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class SimpleArrayProvider : IArrayProvider
  {
    public object[] GetArray ()
    {
      return new object[] { 1, 2, 3, 4, 5 };
    }
  }
}