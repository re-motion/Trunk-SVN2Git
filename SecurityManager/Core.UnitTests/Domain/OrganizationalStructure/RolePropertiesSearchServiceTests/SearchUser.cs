using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using System.Collections.Generic;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchUser : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _userProperty;
    private User _user;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new RolePropertiesSearchService();
      IBusinessObjectClass roleClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (Role));
      _userProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("User");
      Assert.That (_userProperty, Is.Not.Null);

      _user = User.FindByUserName ("group0/user1");
      Assert.That (_user, Is.Not.Null);
    }

    [Test]
    public void SupportsIdentity ()
    {
      Assert.That (_searchService.SupportsIdentity (_userProperty), Is.True);
    }

    [Test]
    public void Search ()
    {
      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      Role role = _testHelper.CreateRole (_user, group, null);
      DomainObjectCollection expectedGroups = User.FindByTenantID (group.Tenant.ID);
      Assert.That (expectedGroups, Is.Not.Empty);

      IBusinessObject[] actualUsers = _searchService.Search (role, _userProperty, null);

      Assert.That (actualUsers, Is.EquivalentTo (expectedGroups));
    }

    [Test]
    public void Search_WithRoleHasNoGroup ()
    {
      Role role = _testHelper.CreateRole (_user, null, null);

      IBusinessObject[] actualUsers = _searchService.Search (role, _userProperty, null);

      Assert.That (actualUsers, Is.Empty);
    }

    [Test]
    public void Search_WithGroupHasNoTenant ()
    {
      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      group.Tenant = null;
      Role role = _testHelper.CreateRole (_user, group, null);

      IBusinessObject[] actualUsers = _searchService.Search (role, _userProperty, null);

      Assert.That (actualUsers, Is.Empty);
    }
  }
}