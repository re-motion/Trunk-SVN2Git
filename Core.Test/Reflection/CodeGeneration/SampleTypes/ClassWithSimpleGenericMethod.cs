namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class ClassWithSimpleGenericMethod
  {
    public virtual string GenericMethod<T1, T2, T3> (T1 t1, T2 t2, T3 t3)
    {
      return string.Format ("{0}, {1}, {2}", t1, t2, t3);
    }
  }
}