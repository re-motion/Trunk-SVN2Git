using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class OrganizationalStructureFactoryTest : DomainTest
  {
    private IOrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();

      _factory = new OrganizationalStructureFactory();
      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void CreateTenant ()
    {
      Assert.That (_factory.CreateTenant (), Is.InstanceOfType (typeof (Tenant)));
    }

    [Test]
    public void CreateGroup ()
    {
      Assert.That (_factory.CreateGroup (), Is.InstanceOfType (typeof (Group)));
    }

    [Test]
    public void CreateUser ()
    {
      Assert.That (_factory.CreateUser (), Is.InstanceOfType (typeof (User)));
    }

    [Test]
    public void CreatePosition ()
    {
      Assert.That (_factory.CreatePosition (), Is.InstanceOfType (typeof (Position)));
    }

    [Test]
    public void GetTenantType ()
    {
      Assert.That (_factory.GetTenantType(), Is.SameAs (typeof (Tenant)));
    }

    [Test]
    public void GetGroupType ()
    {
      Assert.That (_factory.GetGroupType (), Is.SameAs (typeof (Group)));
    }

    [Test]
    public void GetUserType ()
    {
      Assert.That (_factory.GetUserType (), Is.SameAs (typeof (User)));
    }

    [Test]
    public void GetPositionType ()
    {
      Assert.That (_factory.GetPositionType (), Is.SameAs (typeof (Position)));
    }
  }
}