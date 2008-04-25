using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupPropertiesSearchServiceTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _parentGroupProperty;
    private IBusinessObjectReferenceProperty _tenantProperty;
    private Group _group;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      _dbFixtures = new DatabaseFixtures();
      _dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new GroupPropertiesSearchService();
      IBusinessObjectClass groupClass = BindableObjectProvider.Current.GetBindableObjectClass (typeof (Group));
      _parentGroupProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Parent");
      _tenantProperty = (IBusinessObjectReferenceProperty) groupClass.GetPropertyDefinition ("Tenant");
      Assert.That (_parentGroupProperty, Is.Not.Null);

      _group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (_group, Is.Not.Null);
    }

    [Test]
    public void SupportsIdentity ()
    {
      Assert.That (_searchService.SupportsIdentity (_parentGroupProperty), Is.True);
    }

    [Test]
    public void SupportsIdentity_WithInvalidProperty ()
    {
      Assert.That (_searchService.SupportsIdentity (_tenantProperty), Is.False);
    }

    [Test]
    public void Search ()
    {
      List<Group> expectedParentGroups = _group.GetPossibleParentGroups (_group.Tenant.ID);
      Assert.That (expectedParentGroups, Is.Not.Empty);

      IBusinessObject[] actualParentGroups = _searchService.Search (_group, _parentGroupProperty, null);

      Assert.That (actualParentGroups, Is.EquivalentTo (expectedParentGroups));
    }

    [Test]
    public void Search_WithGroupHasNoTenant ()
    {
      _group.Tenant = null;

      IBusinessObject[] actualParentGroups = _searchService.Search (_group, _parentGroupProperty, null);

      Assert.That (actualParentGroups, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The property 'Tenant' is not supported by the 'Remotion.SecurityManager.Domain.OrganizationalStructure.GroupPropertiesSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {

      _searchService.Search (_group, _tenantProperty, null);
    }
  }
}