// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Web.UnitTests.Security.Domain;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandTargetMethodPermissionAttributeTest
  {
    [Test]
    public void Initialize_WithMethodName ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show");

      Assert.AreEqual (MethodType.Instance, attribute.MethodType);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.IsNull (attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameAndSecurableClass ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute ("Show", typeof (SecurableObject));

      Assert.AreEqual (MethodType.Instance, attribute.MethodType);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnum ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show);

      Assert.AreEqual (MethodType.Instance, attribute.MethodType);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame (typeof (SecurableObject), attribute.SecurableClass);
    }

    [Test]
    public void Initialize_WithMethodNameEnumAndSecurableClass ()
    {
      WxeDemandTargetMethodPermissionAttribute attribute = new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (DerivedSecurableObject));

      Assert.AreEqual (MethodType.Instance, attribute.MethodType);
      Assert.AreEqual ("Show", attribute.MethodName);
      Assert.AreSame (typeof (DerivedSecurableObject), attribute.SecurableClass);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Enumerated type 'Remotion.Web.UnitTests.Security.Domain.MethodNameEnum' is not declared as a nested type.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotNestedType ()
    {
      new WxeDemandTargetMethodPermissionAttribute (MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The declaring type of enumerated type 'Remotion.Web.UnitTests.Security.Domain.SimpleType+MethodNameEnum' does not implement interface"
        + " 'Remotion.Security.ISecurableObject'.\r\nParameter name: methodNameEnum")]
    public void Initialize_WithMethodNameEnumNotHavingValidDeclaringType ()
    {
      new WxeDemandTargetMethodPermissionAttribute (SimpleType.MethodNameEnum.Show);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Type 'Remotion.Web.UnitTests.Security.Domain.OtherSecurableObject' cannot be assigned to the declaring type of enumerated type"
        + " 'Remotion.Web.UnitTests.Security.Domain.SecurableObject+Method'.\r\nParameter name: securableClass")]
    public void TestWithParameterNotOfNotMatchingType ()
    {
      new WxeDemandTargetMethodPermissionAttribute (SecurableObject.Method.Show, typeof (OtherSecurableObject));
    }
  }
}
