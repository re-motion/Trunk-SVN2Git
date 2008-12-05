// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Security.UnitTests.Core.SampleDomain;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class DemandMethodPermissionAttributeTest
  {
    [Test]
    public void AcceptValidAccessType ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = new DemandMethodPermissionAttribute (TestAccessTypes.Second);
      Assert.AreEqual (TestAccessTypes.Second, methodPermissionAttribute.GetAccessTypes()[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Enumerated Type 'Remotion.Security.UnitTests.Core.SampleDomain.TestAccessTypesWithoutAccessTypeAttribute' cannot be used as an access type. "
        + "Valid access types must have the Remotion.Security.AccessTypeAttribute applied.\r\nParameter name: accessType")]
    public void RejectAccessTypeWithoutAccessTypeAttribute ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = 
          new DemandMethodPermissionAttribute (TestAccessTypesWithoutAccessTypeAttribute.First);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemTypeException))]
    public void RejectOtherObjectTypes ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute = new DemandMethodPermissionAttribute (new SimpleType());
    }

    [Test]
    public void AcceptMultipleAccessTypes ()
    {
      DemandMethodPermissionAttribute methodPermissionAttribute =
          new DemandMethodPermissionAttribute (TestAccessTypes.Second, TestAccessTypes.Fourth);

      Assert.AreEqual (2, methodPermissionAttribute.GetAccessTypes().Length);
      Assert.Contains (TestAccessTypes.Second, methodPermissionAttribute.GetAccessTypes());
      Assert.Contains (TestAccessTypes.Fourth, methodPermissionAttribute.GetAccessTypes());
    }
  }
}
