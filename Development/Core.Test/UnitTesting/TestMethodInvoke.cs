using System;
using System.Text;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.UnitTesting
{
  public class TypeWithMethods
  {
    public string f ()
    {
      return "f";
    }

    public string f (int i)
    {
      return "f int";
    }

    public string f (int i, string s)
    {
      return "f int string";
    }

    public string f (string s)
    {
      return "f string";
    }

    public string f (StringBuilder sb)
    {
      return "f StringBuilder";
    }

    private string f (int i, StringBuilder s)
    {
      return "f int StringBuilder";
    }

    public static string s (int i)
    {
      return "s int";
    }

    private static string s (string s)
    {
      return "s string";
    }
  }

  public class DerivedType : TypeWithMethods
  {
  }

  [TestFixture]
	public class TestMethodInvoke
	{
    TypeWithMethods _twm = new TypeWithMethods();
    DerivedType _dt = new DerivedType();

    [Test]
    public void TestInvoke()
    {
      Assert.AreEqual ("f",                    PrivateInvoke.InvokePublicMethod (_twm, "f"));
      Assert.AreEqual ("f int",                PrivateInvoke.InvokePublicMethod (_twm, "f", 1));
      Assert.AreEqual ("f int string",         PrivateInvoke.InvokePublicMethod (_twm, "f", 1, null));
      Assert.AreEqual ("f string",             PrivateInvoke.InvokePublicMethod (_twm, "f", "test"));
      Assert.AreEqual ("f StringBuilder",      PrivateInvoke.InvokePublicMethod (_twm, "f", new StringBuilder()));
      Assert.AreEqual ("f int StringBuilder",  PrivateInvoke.InvokeNonPublicMethod (_twm, "f", 1, new StringBuilder()));

      Assert.AreEqual ("f",                    PrivateInvoke.InvokePublicMethod (_dt, "f"));
      Assert.AreEqual ("f int",                PrivateInvoke.InvokePublicMethod (_dt, "f", 1));
      Assert.AreEqual ("f int string",         PrivateInvoke.InvokePublicMethod (_dt, "f", 1, null));
      Assert.AreEqual ("f string",             PrivateInvoke.InvokePublicMethod (_dt, "f", "test"));
      Assert.AreEqual ("f StringBuilder",      PrivateInvoke.InvokePublicMethod (_dt, "f", new StringBuilder()));
      Assert.AreEqual ("f int StringBuilder",  PrivateInvoke.InvokeNonPublicMethod (_dt, typeof (TypeWithMethods), "f", 1, new StringBuilder()));
    }

    [Test]
    public void TestStaticInvoke()
    {
      Assert.AreEqual ("s int",                PrivateInvoke.InvokePublicStaticMethod (typeof(TypeWithMethods), "s", 1));
      Assert.AreEqual ("s string",             PrivateInvoke.InvokeNonPublicStaticMethod (typeof(TypeWithMethods), "s", "test"));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMethodNameException))]
    public void TestPublicInvokeAmbiguous()
    {
      PrivateInvoke.InvokePublicMethod (_twm, "f", null);
    }

    [Test]
    [ExpectedException (typeof (MethodNotFoundException))]
    public void TestPublicInvokeMethodNotFound()
    {
      PrivateInvoke.InvokePublicMethod (_twm, "f", 1.0);
    }
	}
}
