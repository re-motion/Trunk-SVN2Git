using System;
using NUnit.Framework;
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
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficTenant (tenant);

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateSpecificTenant_IsNull ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficTenant (tenant);
      ace.SpecificTenant = null;

      AccessControlEntryValidationResult result = ace.Validate();

      Assert.IsFalse (result.IsValid);
      Assert.IsTrue (result.IsSpecificTenantMissing);
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException),
        ExpectedMessage = "The access control entry has the Tenant property set to SpecificTenant, but no Tenant is assigned.")]
    public void Commit_SpecificTenantIsNull ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficTenant (tenant);
      ace.SpecificTenant = null;

      ClientTransactionScope.CurrentTransaction.Commit();
    }

    [Test]
    public void ValidateSpecificTenant_IsNullAndObjectIsDeleted ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficTenant (tenant);
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