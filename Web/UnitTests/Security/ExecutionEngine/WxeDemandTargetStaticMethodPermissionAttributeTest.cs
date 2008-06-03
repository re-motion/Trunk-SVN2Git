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

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandTargetStaticMethodPermissionAttributeTest
  {
    [Test]
    public void Initialize_WithMethodNameAndSecurableClass ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute ("Search", typeof (SecurableObject));

      Assert.AreEqual (MethodType.Static, attribute.MethodType);
      Assert.AreEqual ("Search", attribute.MethodName);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnum ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search);

      Assert.AreEqual (MethodType.Static, attribute.MethodType);
      Assert.AreEqual ("Search", attribute.MethodName);
      Assert.AreSame (typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnumAndSecurableClass ()
    {
      WxeDemandTargetStaticMethodPermissionAttribute attribute = 
          new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Search, typeof (DerivedSecurableObject));

      Assert.AreEqual (MethodType.Static, attribute.MethodType);
      Assert.AreEqual ("Search", attribute.MethodName);
      Assert.AreSame (typeof (DerivedSecurableObject), attribute.SecurableClass);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated type 'Remotion.Web.UnitTests.Security.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedType ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The declaring type of enumerated type 'Remotion.Web.UnitTests.Security.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Remotion.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringType ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (SimpleType.MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated type 'Remotion.Web.UnitTests.Security.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedTypeAndSecurableClass ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (MethodNameEnum.Show, typeof (DerivedSecurableObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The declaring type of enumerated type 'Remotion.Web.UnitTests.Security.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Remotion.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringTypeAndSecurableClass ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (SimpleType.MethodNameEnum.Show, typeof (DerivedSecurableObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Type 'Remotion.Web.UnitTests.Security.Domain.OtherSecurableObject' cannot be assigned to the declaring type of enumerated type"
        + " 'Remotion.Web.UnitTests.Security.Domain.SecurableObject+Method'.\r\nParameter name: securableClass")]
    public void TestWithParameterNotOfNotMatchingType ()
    {
      new WxeDemandTargetStaticMethodPermissionAttribute (SecurableObject.Method.Show, typeof (OtherSecurableObject));
    }
  }
}
