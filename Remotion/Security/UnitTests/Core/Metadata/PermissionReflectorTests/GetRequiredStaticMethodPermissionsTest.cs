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
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  [TestFixture]
  public class GetRequiredStaticMethodPermissionsTest
  {
    private IPermissionProvider _permissionReflector;

    [SetUp]
    public void SetUp ()
    {
      _permissionReflector = new PermissionReflector();
    }

    [Test]
    public void Test_MethodWithoutAttributes ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CheckPermissions"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CheckPermissions"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodInformation));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CreateForSpecialCase"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Create, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribut ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CreateForSpecialCase"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodInformation));
    }
  }
}