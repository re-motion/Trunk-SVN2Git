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
