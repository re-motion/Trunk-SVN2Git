// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Security.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.Security.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class ObjectDeletingTest
  {
    private TestHelper _testHelper;
    private IClientTransactionExtension _extension;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new TestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityIoCConfiguration ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityIoCConfiguration ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, true);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, false);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.ObjectDeleting (_testHelper.Transaction, securableObject);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, nonSecurableObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject otherObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      HasAccessDelegate hasAccess = delegate
      {
        otherObject.Delete ();
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, hasAccess);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, true);
      _testHelper.ReplayAll ();

      _testHelper.Transaction.ExecuteInScope (securableObject.Delete);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNewObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithActiveTransactionMatchingTransactionPassedAsArgument_DoesNotCreateScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.Transaction.Commit ();
      using (var scope = _testHelper.Transaction.EnterNonDiscardingScope())
      {
        _testHelper.AddExtension (_extension);
        _testHelper.ExpectObjectSecurityStrategyHasAccessWithMatchingScope (securableObject, scope);
        _testHelper.ReplayAll();

        _extension.ObjectDeleting (_testHelper.Transaction, securableObject);
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithActiveTransactionNotMatchingTransactionPassedAsArgument_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, true);
      _testHelper.ReplayAll();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        _extension.ObjectDeleting (_testHelper.Transaction, securableObject);
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithInactiveTransaction_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, true);
      _testHelper.ReplayAll ();

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive (_testHelper.Transaction))
        {
          _extension.ObjectDeleting (_testHelper.Transaction, securableObject);
        }
      }

      _testHelper.VerifyAll ();
    }
  }
}
