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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class RelationChangingTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;
    private IDisposable _transactionScope;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
      _transactionScope = _testHelper.Transaction.EnterDiscardingScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
      _transactionScope.Dispose ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (ClientTransaction.CreateRootTransaction (), securableObject, "Parent", null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (ClientTransaction.CreateRootTransaction (), securableObject, "Parent", null, null);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.RelationChanging (ClientTransaction.CreateRootTransaction (), securableObject, "Parent", null, null);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (ClientTransaction.CreateRootTransaction (), nonSecurableObject, "Parent", null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSide_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      securableObject.OtherParent = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate
      {
        securableObject.OtherParent = newObject;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.RelationChanging (ClientTransaction.CreateRootTransaction (), securableObject, "Parent", null, null);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSideSetNull_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (newObject, TestAccessTypes.Second, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Parent = newObject;

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSideSetNewValue_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject oldObject = _testHelper.CreateSecurableObject ();
      securableObject.Parent = oldObject;
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (oldObject, TestAccessTypes.Second, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Parent = null;

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySideAdd_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject newObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (newObject, TestAccessTypes.Second, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Children.Add (newObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySideRemove_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject oldObject = _testHelper.CreateSecurableObject ();
      securableObject.Children.Add (oldObject);
      _testHelper.AddExtension (_extension);
      using (_testHelper.Ordered ())
      {
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Parent", TestAccessTypes.Second);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (oldObject, TestAccessTypes.Second, true);
        _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("Children", TestAccessTypes.First);
        _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      }
      _testHelper.ReplayAll ();

      securableObject.Children.Remove (oldObject);

      _testHelper.VerifyAll ();
    }
  }
}
