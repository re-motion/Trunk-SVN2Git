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
  public class GetRequiredStaticMethodPermissionsTest
  {
    private IPermissionProvider _permissionReflector;

    [SetUp]
    public void SetUp ()
    {
      _permissionReflector = new PermissionReflector ();
    }

    [Test]
    public void Test_MethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_MethodWithoutAttributes_MethodInformation ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CheckPermissions"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation, MemberAffiliation.Static);

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions");

      Assert.AreEqual (requiredAccessTypes, _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions"));
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes_MethodInformation ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CheckPermissions"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation, MemberAffiliation.Static);

      Assert.AreEqual (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation, MemberAffiliation.Static));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Create, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_MethodWithOneAttribute_MethodInformation ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CreateForSpecialCase"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation, MemberAffiliation.Static);

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Create, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribut ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase"));
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribut_MethodInformation ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("CreateForSpecialCase"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation, MemberAffiliation.Static);

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation, MemberAffiliation.Static));
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "IsValid");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Read, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttributes_MethodInformation ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (SecurableObject).GetMethod ("IsValid", new[]{typeof(SecurableObject)}));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodInformation, MemberAffiliation.Static);

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Read, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_MethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (DerivedSecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Create, requiredAccessTypes[0]);
    }

    [Test]
    [Ignore ("Check method")]
    public void Test_MethodOfDerivedClass_MethodInformation ()
    {
      IMethodInformation methodInformation = new MethodInfoAdapter (typeof (DerivedSecurableObject).GetMethod ("CreateForSpecialCase"));
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), methodInformation, MemberAffiliation.Static );

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Create, requiredAccessTypes[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Sve' could not be found.\r\nParameter name: memberName")]
    public void Test_NotExistingMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "Sve");
    }
  }
}
