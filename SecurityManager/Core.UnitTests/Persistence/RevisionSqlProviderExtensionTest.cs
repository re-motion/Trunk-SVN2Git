using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain;

namespace Remotion.SecurityManager.UnitTests.Persistence
{
  [TestFixture]
  public class RevisionSqlProviderExtensionTest : DomainTest
  {
    private OrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      
      _factory = new OrganizationalStructureFactory ();
    }

    [Test]
    public void Saving_OneSecurityManagerDomainObject ()
    {
      Tenant tenant = _factory.CreateTenant ();

      ClientTransactionScope.CurrentTransaction.Commit ();

      Assert.AreEqual (1, Revision.GetRevision ());
    }

    [Test]
    public void Saving_DisacardedDomainObject ()
    {
      Tenant tenant = _factory.CreateTenant ();
      tenant.Delete ();

      ClientTransactionScope.CurrentTransaction.Commit ();

      Assert.AreEqual (0, Revision.GetRevision ());
    }
  }
}