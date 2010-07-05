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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  [TestFixture]
  public class GetRequiredPropertyReadPermissionsTest
  {
    private IPermissionProvider _permissionReflector;

    [SetUp]
    public void SetUp ()
    {
      _permissionReflector = new PermissionReflector ();
    }

    [Test]
    public void Test_PropertyWithoutAttributes ()
    {
      IPropertyInformation propertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsEnabled"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation);

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForPropertyWithoutAttributes ()
    {
      IPropertyInformation propertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsEnabled"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation);

      Assert.AreEqual (requiredAccessTypes, _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation));
    }

    [Test]
    public void Test_PropertyWithOneAttribute ()
    {
      IPropertyInformation propertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsVisible"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation);

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (TestAccessTypes.Third, requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForPropertyWithOneAttribute ()
    {
      IPropertyInformation propertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsVisible"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation);

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyInformation));
    }
  }
}
