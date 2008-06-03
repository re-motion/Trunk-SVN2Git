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
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  [TestFixture]
  public class GetRequiredPropertyWritePermissionsTest
  {
    private IPermissionProvider _permissionReflector;

    [SetUp]
    public void SetUp ()
    {
      _permissionReflector = new PermissionReflector ();
    }

    [TearDown]
    public void TearDown ()
    {
      TestPermissionReflector.Cache.Clear ();
    }

    [Test]
    public void Test_PropertyWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsEnabled");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForPropertyWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsEnabled");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsEnabled"));
    }

    [Test]
    public void Test_PropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsVisible");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (TestAccessTypes.Fourth, requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForPropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsVisible");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsVisible"));
    }

    [Test]
    public void Test_NonPublicPropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "NonPublicProperty");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (TestAccessTypes.Second, requiredAccessTypes);
    }

    [Test]
    public void Test_ExplicitInterfacePropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), 
          typeof (IInterfaceWithProperty).FullName + ".InterfaceProperty");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (TestAccessTypes.Second, requiredAccessTypes);
    }
  }
}
