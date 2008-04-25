using System;
using NUnit.Framework;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandMethodPermissionAttributeHelperTestForGetTypeOfSecurableObject
  {
    // types

    // static members

    // member fields
    private WxeDemandTargetMethodPermissionAttribute _attribute;

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelperTestForGetTypeOfSecurableObject ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _attribute = new WxeDemandTargetMethodPermissionAttribute ("Show");
    }

    [Test]
    public void TestWithValidParameterName ()
    {
      _attribute.ParameterName = "ThisObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObjectAsSecondParameter),
          _attribute);

      Assert.AreSame (typeof (SecurableObject), helper.GetTypeOfSecurableObject ());
    }

    [Test]
    public void TestWithDefaultParameter ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          _attribute);

      Assert.AreSame (typeof (SecurableObject), helper.GetTypeOfSecurableObject ());
    }

    [Test]
    public void TestWithParameterTypeIsBaseType ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("ShowSpecial", typeof (DerivedSecurableObject));
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          attribute);

      Assert.AreSame (typeof (DerivedSecurableObject), helper.GetTypeOfSecurableObject ());
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'ThisObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
       + " WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is of type 'Remotion.Web.UnitTests.Security.Domain.SecurableObject',"
        + " which is not a base type of type 'Remotion.Web.UnitTests.Security.Domain.OtherSecurableObject'.")]
    public void TestWithParameterNotOfNotMatchingType ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show", typeof (OtherSecurableObject));
      attribute.ParameterName = "ThisObject";
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObjectAsSecondParameter),
          attribute);
    
      helper.GetTypeOfSecurableObject ();
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'SomeObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' does not implement interface"
        + " 'Remotion.Security.ISecurableObject'.")]
    public void TestWithParameterNotImplementingISecurableObject ()
    {
      _attribute.ParameterName = "SomeObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObjectAsSecondParameter),
          _attribute);

      helper.GetTypeOfSecurableObject ();
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithoutParameters' has"
       + " a WxeDemandTargetMethodPermissionAttribute applied, but does not define any parameters to supply the 'this-object'.")]
    public void TestFromFunctionWithoutParameters ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithoutParameters),
          _attribute);

      helper.GetTypeOfSecurableObject ();
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'Invalid' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is not a valid parameter"
        + " of this function.")]
    public void TestWithInvalidParameterName ()
    {
      _attribute.ParameterName = "Invalid";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObjectAsSecondParameter),
          _attribute);

      helper.GetTypeOfSecurableObject ();
    }
  }
}