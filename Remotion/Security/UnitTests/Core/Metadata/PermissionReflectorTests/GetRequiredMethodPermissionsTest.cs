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
using System.Collections.Specialized;
using System.Reflection;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  [TestFixture]
  public class GetRequiredMethodPermissionsTest
  {
    private IPermissionProvider _permissionReflector;

    [SetUp]
    public void SetUp ()
    {
      _permissionReflector = new PermissionReflector ();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new PermissionReflector ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void Test_MethodWithoutAttributes ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Save"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_PropertyWithoutAttributes ()
    {
      IPropertyInformation propertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsEnabled"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), propertyInformation.GetGetMethod());

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Save"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Record"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForPropertyWithoutAttributes ()
    {
      IPropertyInformation propertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsEnabled"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), propertyInformation.GetGetMethod());

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), propertyInformation.GetGetMethod()));
    }

    [Test]
    public void Test_MethodWithTwoPermissions ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Show"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Create, requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribute ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Record"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation));
    }

    [Test]
    public void Test_CacheForPropertyWithOneAttribute ()
    {
      IPropertyInformation propertyInformation = new PropertyInfoAdapter (typeof (SecurableObject).GetProperty ("IsVisible"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), propertyInformation.GetGetMethod());

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), propertyInformation.GetGetMethod()));
    }

    [Test]
    public void Test_OverriddenMethodWithPermissionFromBaseMethod ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (DerivedSecurableObject).GetMethod ("Record"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), methodInformation);

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
    }

    [Test]
    public void FilterMultipleAccessTypes ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("Close"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation);

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Find, requiredAccessTypes);
    }
  }
}
