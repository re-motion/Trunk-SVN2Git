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
  public class ObjectDeletingTest
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
      HasAccessDelegate hasAccess = delegate (ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
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

      securableObject.Delete ();

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
  }
}
