// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Core.Security.Domain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Core.Security.ExecutionEngine
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
       + " WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is of type 'Remotion.Web.UnitTests.Core.Security.Domain.SecurableObject',"
        + " which is not a base type of type 'Remotion.Web.UnitTests.Core.Security.Domain.OtherSecurableObject'.")]
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
        + " WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' does not implement interface"
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
    [ExpectedException (typeof (WxeException), ExpectedMessage = "WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithoutParameters' has"
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
        + " WxeFunction 'Remotion.Web.UnitTests.Core.Security.ExecutionEngine.TestFunctionWithThisObjectAsSecondParameter' is not a valid parameter"
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
