using System;
using NUnit.Framework;
using Remotion.Web.Security.UI;
using Remotion.Web.UnitTests.Security.Domain;

namespace Remotion.Web.UnitTests.Security.UI
{
  [TestFixture]
  public class DemandTargetMethodPermissionAttributeTest
  {
    [Test]
    public void Initialize_WithMethodName ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute ("Show");

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.IsNull (attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameAndSecurableClass ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute ("Show", typeof (SecurableObject));

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnum ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute (SecurableObject.Method.Show);

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame (typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnumAndSecurableClass ()
    {
      DemandTargetMethodPermissionAttribute attribute = new DemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (DerivedSecurableObject));

      Assert.AreEqual (PermissionSource.SecurableObject, attribute.PermissionSource);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame (typeof (DerivedSecurableObject), attribute.SecurableClass);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Enumerated type 'Remotion.Web.UnitTests.Security.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedType ()
    {
      new DemandTargetMethodPermissionAttribute (MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The declaring type of enumerated type 'Remotion.Web.UnitTests.Security.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Remotion.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringType ()
    {
      new DemandTargetMethodPermissionAttribute (SimpleType.MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Type 'Remotion.Web.UnitTests.Security.Domain.OtherSecurableObject' cannot be assigned to the declaring type of enumerated type"
        + " 'Remotion.Web.UnitTests.Security.Domain.SecurableObject+Method'.\r\nParameter name: securableClass")]
    public void TestWithParameterNotOfNotMatchingType ()
    {
      new DemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (OtherSecurableObject));
    }
  }
}