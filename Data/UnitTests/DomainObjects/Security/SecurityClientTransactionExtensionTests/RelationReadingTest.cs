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
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class RelationReadingTest
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
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Parent", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.RelationReading (new RootClientTransaction (), securableObject, "Parent", ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Parent", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.RelationReading (new RootClientTransaction(), securableObject, "Parent", ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.RelationReading (new RootClientTransaction(), securableObject, "Parent", ValueAccess.Current);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.RelationReading (new RootClientTransaction(), nonSecurableObject, "Parent", ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSide_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      securableObject.OtherParent = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Parent", TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate (ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        Dev.Null = securableObject.OtherParent;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.RelationReading (new RootClientTransaction(), securableObject, "Parent", ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySide_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      securableObject.OtherChildren.Add (_testHelper.CreateSecurableObject ());
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Children", TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate (ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        Dev.Null = securableObject.OtherChildren[0];
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.RelationReading (new RootClientTransaction(), securableObject, "Children", ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_OneSide_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      securableObject.Parent = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Parent", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      Dev.Null = securableObject.Parent;

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ManySide_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      securableObject.Children.Add (_testHelper.CreateSecurableObject ());
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("Children", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      Dev.Null = securableObject.Children[0];

      _testHelper.VerifyAll ();
    }
  }
}
