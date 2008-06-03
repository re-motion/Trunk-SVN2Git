/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandMethodPermissionAttributeHelperTestForGetSecurableObject
  {
    // types

    // static members

    // member fields
    private WxeDemandTargetMethodPermissionAttribute _attribute;
    private TestFunctionWithThisObjectAsSecondParameter _functionWithThisObjectAsSecondParamter;

    // construction and disposing

    public WxeDemandMethodPermissionAttributeHelperTestForGetSecurableObject ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _attribute = new WxeDemandTargetMethodPermissionAttribute ("Read");

      object someObject = new object ();
      SecurableObject thisObject = new SecurableObject (null);
      _functionWithThisObjectAsSecondParamter = new TestFunctionWithThisObjectAsSecondParameter (someObject, thisObject);
      _functionWithThisObjectAsSecondParamter.SomeObject = someObject; // Required because in this test the WxeFunction has not started executing.
      _functionWithThisObjectAsSecondParamter.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.
    }

    [Test]
    public void TestWithValidParameterName ()
    {
      _attribute.ParameterName = "ThisObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      Assert.AreSame (_functionWithThisObjectAsSecondParamter.ThisObject, helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter));
    }

    [Test]
    public void TestWithDefaultParameter ()
    {
      SecurableObject thisObject = new SecurableObject (null);
      TestFunctionWithThisObject function = new TestFunctionWithThisObject (thisObject, null);
      function.ThisObject = thisObject; // Required because in this test the WxeFunction has not started executing.

      _attribute.ParameterName = null;

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          function.GetType (),
          _attribute);

      Assert.AreSame (function.ThisObject, helper.GetSecurableObject (function));
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'ThisObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithThisObject' is null.")]
    public void TestWithParameterNull ()
    {
      _attribute.ParameterName = "ThisObject";
      TestFunctionWithThisObject function = new TestFunctionWithThisObject (null, null);

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          function.GetType (),
          _attribute);

      helper.GetSecurableObject (function);
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'SomeObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' does not implement"
        + " interface 'Remotion.Security.ISecurableObject'.")]
    public void TestWithParameterNotImplementingISecurableObject ()
    {
      _attribute.ParameterName = "SomeObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'ThisObject' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is not derived from type"
        + " 'Remotion.Web.UnitTests.Security.Domain.OtherSecurableObject'.")]
    public void TestWithParameterNotOfMatchingType ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show", typeof (OtherSecurableObject));
      attribute.ParameterName = "ThisObject";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void TestWithInvalidFunctionType ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithThisObject),
          _attribute);

      helper.GetSecurableObject (new TestFunctionWithoutPermissions ());
    }

    [Test]
    [ExpectedException (typeof (WxeException),
       ExpectedMessage = "WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithoutParameters' has a WxeDemandTargetMethodPermissionAttribute"
       + " applied, but does not define any parameters to supply the 'this-object'.")]
    public void TestWithFunctionWithoutParameters ()
    {
      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          typeof (TestFunctionWithoutParameters),
          _attribute);

      helper.GetSecurableObject (new TestFunctionWithoutParameters ());
    }

    [Test]
    [ExpectedException (typeof (WxeException), ExpectedMessage = "The parameter 'Invalid' specified by the WxeDemandTargetMethodPermissionAttribute applied to"
        + " WxeFunction 'Remotion.Web.UnitTests.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is not a valid parameter"
        + " of this function.")]
    public void TestWithInvalidParameterName ()
    {
      _attribute.ParameterName = "Invalid";

      WxeDemandMethodPermissionAttributeHelper helper = new WxeDemandMethodPermissionAttributeHelper (
          _functionWithThisObjectAsSecondParamter.GetType (),
          _attribute);

      helper.GetSecurableObject (_functionWithThisObjectAsSecondParamter);
    }
  }
}
