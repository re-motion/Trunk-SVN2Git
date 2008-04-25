using System;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using System.Reflection;

namespace Remotion.Mixins.Samples.UnitTests
{
  [TestFixture]
  public class EquatableMixinTest
  {
    [Uses(typeof (EquatableMixin<C>))]
    public class C
    {
      public int I;
      public string S;
      public bool B;
    }

    [Test]
    public void ImplementsEquatable()
    {
      C c = new C();
      Assert.IsFalse (c is IEquatable<C>);

      C c2 = ObjectFactory.Create<C>().With();
      Assert.IsTrue (c2 is IEquatable<C>);
    }

    [Test]
    public void EqualsRespectsMembers ()
    {
      C c = ObjectFactory.Create<C> ().With ();
      C c2 = ObjectFactory.Create<C> ().With ();
      Assert.AreEqual (c, c2);

      c2.S = "foo";
      Assert.AreNotEqual (c, c2);
      c2.I = 5;
      c2.B = true;
      Assert.AreNotEqual (c, c2);
      c.S = "foo";
      Assert.AreNotEqual (c, c2);
      c.I = 5;
      Assert.AreNotEqual (c, c2);
      c.B = true;
      Assert.AreEqual (c, c2);
    }

    [Test]
    public void GetHashCodeRespectsMembers ()
    {
      C c = ObjectFactory.Create<C> ().With ();
      C c2 = ObjectFactory.Create<C> ().With ();
      Assert.AreEqual (c.GetHashCode(), c2.GetHashCode());

      c2.S = "foo";
      Assert.AreNotEqual (c.GetHashCode(), c2.GetHashCode());
      c2.I = 5;
      c2.B = true;
      Assert.AreNotEqual (c.GetHashCode (), c2.GetHashCode ());
      c.S = "foo";
      Assert.AreNotEqual (c.GetHashCode (), c2.GetHashCode ());
      c.I = 5;
      Assert.AreNotEqual (c.GetHashCode (), c2.GetHashCode ());
      c.B = true;
      Assert.AreEqual (c.GetHashCode (), c2.GetHashCode ());
    }
  }
}
