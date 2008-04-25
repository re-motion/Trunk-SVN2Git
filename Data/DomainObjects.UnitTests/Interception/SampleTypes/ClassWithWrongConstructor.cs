using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public class ClassWithWrongConstructor : DomainObject
  {
    public static ClassWithWrongConstructor NewObject ()
    {
      return NewObject<ClassWithWrongConstructor> ().With();
    }

    public static ClassWithWrongConstructor NewObject (double d)
    {
      return NewObject<ClassWithWrongConstructor> ().With (d);
    }

    public ClassWithWrongConstructor (string s)
    {
      Assert.Fail ("Shouldn't be executed.");
    }
  }
}