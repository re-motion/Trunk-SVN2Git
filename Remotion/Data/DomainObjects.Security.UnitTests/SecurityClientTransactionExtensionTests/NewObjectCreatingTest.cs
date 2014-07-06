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
  public class NewObjectCreatingTest
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
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (_testHelper.Transaction, typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, false);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (_testHelper.Transaction, typeof (SecurableObject));
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.NewObjectCreating (_testHelper.Transaction, typeof (SecurableObject));
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (_testHelper.Transaction, typeof (NonSecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      IObjectSecurityStrategy objectSecurityStrategy = _testHelper.CreateObjectSecurityStrategy ();
      _testHelper.AddExtension (_extension);
      HasStatelessAccessDelegate hasAccess = delegate
      {
        SecurableObject.NewObject (_testHelper.Transaction, objectSecurityStrategy);
        return true;
      };
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, hasAccess);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (_testHelper.Transaction, typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      IObjectSecurityStrategy objectSecurityStrategy = _testHelper.CreateObjectSecurityStrategy ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll ();

      SecurableObject.NewObject (_testHelper.Transaction, objectSecurityStrategy);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithActiveTransactionMatchingTransactionPassedAsArgument_DoesNotCreateScope ()
    {
      using (var scope = _testHelper.Transaction.EnterNonDiscardingScope())
      {
        _testHelper.AddExtension (_extension);
        _testHelper.ExpectFunctionalSecurityStrategyHasAccessWithMatchingScope (scope);
        _testHelper.ReplayAll();

        _extension.NewObjectCreating (_testHelper.Transaction, typeof (SecurableObject));
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithActiveTransactionNotMatchingTransactionPassedAsArgument_CreatesScope ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        _extension.NewObjectCreating (_testHelper.Transaction, typeof (SecurableObject));
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithInactiveTransaction_CreatesScope ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll ();

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive (_testHelper.Transaction))
        {
        _extension.NewObjectCreating (_testHelper.Transaction, typeof (SecurableObject));
        }
      }

      _testHelper.VerifyAll ();
    }
  }
}
