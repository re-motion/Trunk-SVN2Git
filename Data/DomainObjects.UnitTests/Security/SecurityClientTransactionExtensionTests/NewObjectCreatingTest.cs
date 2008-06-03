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
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.DomainObjects.UnitTests.Security.TestDomain;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.UnitTests.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class NewObjectCreatingTest
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
      HasStatelessAccessDelegate hasAccess = delegate (Type type, ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
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
  }
}
