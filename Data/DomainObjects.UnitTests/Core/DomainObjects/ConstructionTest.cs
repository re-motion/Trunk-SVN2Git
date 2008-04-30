using System;
using NUnit.Framework;


namespace Remotion.Data.DomainObjects.UnitTests.Core.DomainObjects
{
  [TestFixture]
  public class ConstructionTest : ClientTransactionBaseTest
  {
    [DBTable]
    public class DomainObjectWithSpecialConstructor : DomainObject
    {
      public string S;
      public object O;

      protected DomainObjectWithSpecialConstructor (string s)
      {
        S = s;
      }

      protected DomainObjectWithSpecialConstructor (object o)
      {
        O = o;
      }

      public static DomainObjectWithSpecialConstructor NewObject (string s)
      {
        return NewObject<DomainObjectWithSpecialConstructor>().With(s);
      }

      public static DomainObjectWithSpecialConstructor NewObject (object o)
      {
        return NewObject<DomainObjectWithSpecialConstructor> ().With (o);
      }
    }

    [Test]
    public void ConstructorSelection ()
    {
      DomainObjectWithSpecialConstructor d1 = DomainObjectWithSpecialConstructor.NewObject ("string");
      Assert.AreEqual ("string", d1.S);
      Assert.IsNull (d1.O);

      object obj = new object ();
      DomainObjectWithSpecialConstructor d2 = DomainObjectWithSpecialConstructor.NewObject (obj);
      Assert.IsNull (d2.S);
      Assert.AreSame (obj, d2.O);
    }
  }
}
