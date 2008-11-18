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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class ValidationTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Validate_IsValid ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateSpecificTenant_IsValid ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant (tenant);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateSpecificTenant_IsNull ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      ace.SpecificTenant = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.IsFalse (result.IsValid);
      Assert.That (result.GetErrors(), List.Contains (AccessControlEntryValidationError.IsSpecificTenantMissing));
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
        "The access control entry is in an invalid state:\r\n"
        + "  The TenantCondition property is set to SpecificTenant, but no SpecificTenant is assigned.")]
    public void Commit_OneError ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      ace.SpecificTenant = null;

      ClientTransactionScope.CurrentTransaction.Commit();
    }

    [Test]
    public void ValidateSpecificTenant_IsNullAndObjectIsDeleted ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecificTenant (tenant);
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        ace.SpecificTenant = null;
        ace.Delete();

        AccessControlEntryValidationResult result = ace.Validate();

        Assert.IsTrue (result.IsValid);
        Assert.AreEqual (StateType.Deleted, ace.State);
      }
    }
  }
}