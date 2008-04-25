using System;
using System.Text;
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.UnitTesting
{

public class PublicClass
{
  private class InternalClass: PublicClass
  {
    private string _s;

    public InternalClass (string s)
    {
      _s = s;
    }

    private InternalClass ()
    {
      _s = "private ctor";
    }

    protected override string f()
    {
      return _s;
    }

    public InternalClass (StringBuilder s)
    {
      _s = s.ToString();
    }

  }

  protected virtual string f ()
  {
    return "PublicClass";
  }
}

[TestFixture]
public class TestCreateInstance
{
  const string c_assemblyName = "Remotion.Development.UnitTests";
  const string c_publicClassName = "Remotion.Development.UnitTests.UnitTesting.PublicClass";
  const string c_internalClassName = "Remotion.Development.UnitTests.UnitTesting.PublicClass+InternalClass";

  [Test]
  public void TestCreateInstances()
  {
    PublicClass internalInstance;

    internalInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_internalClassName, 
        "test 1");
    Assert.AreEqual ("test 1", PrivateInvoke.InvokeNonPublicMethod (internalInstance, "f"));

    internalInstance = (PublicClass) PrivateInvoke.CreateInstanceNonPublicCtor (
        c_assemblyName, c_internalClassName);
    Assert.AreEqual ("private ctor", PrivateInvoke.InvokeNonPublicMethod (internalInstance, "f"));

    PublicClass publicInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_publicClassName);
    Assert.AreEqual ("PublicClass", PrivateInvoke.InvokeNonPublicMethod (publicInstance, "f"));
  }

  [Test]
  [ExpectedException (typeof (AmbiguousMethodNameException))]
  public void TestCreateInstanceAmbiguous()
  {
    PublicClass internalInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_internalClassName, 
        null);
  }
}

}
