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
