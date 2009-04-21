// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Configuration;
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

    [TearDown]
    public void TearDown ()
    {
      TestPermissionReflector.Cache.Clear ();
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
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save"));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Delete");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Delete, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_MethodWithTwoPermissions ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Create, requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record"));
    }

    [Test]
    public void Test_MethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Create, requiredAccessTypes);
    }

    [Test]
    public void Test_OverriddenMethodWithPermissionFromBaseMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Record");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains(GeneralAccessTypes.Edit, requiredAccessTypes);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Sve' could not be found.\r\nParameter name: memberName")]
    public void Test_NotExistingMethod ()
    {
      _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Sve");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
      ExpectedMessage = "The DemandMethodPermissionAttribute must not be defined on members overriden or redefined in derived classes. "
        + "A member 'Send' exists in class 'Remotion.Security.UnitTests.Core.SampleDomain.DerivedSecurableObject' and its base class."
        + "\r\nParameter name: memberName")]
    public void Test_PermissionsDeclaredOnBaseAndDerivedClass ()
    {
      _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Send");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Load' has multiple DemandMethodPermissionAttribute defined.\r\nParameter name: memberName")]
    public void Test_PermissionsDeclaredOnOverloads ()
    {
      _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Load");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
      ExpectedMessage = "The DemandMethodPermissionAttribute must not be defined on members overriden or redefined in derived classes. "
        + "A member 'Print' exists in class 'Remotion.Security.UnitTests.Core.SampleDomain.DerivedSecurableObject' and its base class."
        + "\r\nParameter name: memberName")]
    public void Test_OverriddenMethodDefinesPermission ()
    {
      _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Print");
    }

    [Test]
    public void FilterMultipleAccessTypes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Close");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Find, requiredAccessTypes);
    }
  }
}
