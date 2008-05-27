using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Security.Principal;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Rhino.Mocks;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Configuration;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class RoleTest : DomainTest
  {
    private ObjectID _expectedTenantID;
    private ObjectID _expectedRootGroupID;
    private ObjectID _expectedParentGroup0ID;
    private ObjectID _expectedGroup0ID;
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private IUserProvider _mockUserProvider;
    private IFunctionalSecurityStrategy _stubFunctionalSecurityStrategy;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransactionScope.CurrentTransaction);
        _expectedTenantID = tenant.ID;

        DomainObjectCollection groups = Group.FindByTenantID (_expectedTenantID);
        foreach (Group group in groups)
        {
          if (group.UniqueIdentifier == "UID: rootGroup")
            _expectedRootGroupID = group.ID;
          else if (group.UniqueIdentifier == "UID: parentGroup0")
            _expectedParentGroup0ID = group.ID;
          else if (group.UniqueIdentifier == "UID: group0")
            _expectedGroup0ID = group.ID;
        }
      }
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _mockSecurityProvider = (ISecurityProvider) _mocks.CreateMultiMock (typeof (ProviderBase), typeof (ISecurityProvider));
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockUserProvider = (IUserProvider) _mocks.CreateMultiMock (typeof (ProviderBase), typeof (IUserProvider));
      _stubFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _stubFunctionalSecurityStrategy;

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    public override void TearDown ()
    {
      base.TearDown();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void GetPossibleGroups ()
    {
      IPrincipal principal = new GenericPrincipal (new GenericIdentity ("group0/user1"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser()).Return (principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: rootGroup", "UID: testTenant", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: parentGroup0", "UID: testTenant", principal, SecurityManagerAccessTypes.AssignRole);
      ExpectSecurityProviderGetAccessForGroup ("UID: group0", "UID: testTenant", principal, SecurityManagerAccessTypes.AssignRole);
      ExpectSecurityProviderGetAccessForGroup ("UID: parentGroup1", "UID: testTenant", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: group1", "UID: testTenant", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testRootGroup", "UID: testTenant", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testParentOfOwningGroup", "UID: testTenant", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testOwningGroup", "UID: testTenant", principal);
      ExpectSecurityProviderGetAccessForGroup ("UID: testGroup", "UID: testTenant", principal);
      Role role = Role.NewObject();
      _mocks.ReplayAll ();

      List<Group> groups = role.GetPossibleGroups (_expectedTenantID);

      _mocks.VerifyAll ();
      Assert.AreEqual (2, groups.Count);
      foreach (string groupUnqiueIdentifier in new string[] { "UID: parentGroup0", "UID: group0" })
      {
        Assert.IsTrue (
            groups.Exists (delegate (Group current) { return groupUnqiueIdentifier == current.UniqueIdentifier; }),
            "Group '{0}' was not found.",
            groupUnqiueIdentifier);
      }
    }

    [Test]
    public void GetPossibleGroups_WithoutSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      Role role = Role.NewObject();

      List<Group> groups = role.GetPossibleGroups (_expectedTenantID);

      Assert.AreEqual (9, groups.Count);
    }

    [Test]
    public void GetPossiblePositions ()
    {
      IPrincipal principal = new GenericPrincipal (new GenericIdentity ("group0/user1"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser ()).Return (principal);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principal, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principal);
      Role role = Role.NewObject();
      Group parentGroup = Group.GetObject (_expectedParentGroup0ID);
      _mocks.ReplayAll ();

      List<Position> positions = role.GetPossiblePositions (parentGroup);

      _mocks.VerifyAll ();
      Assert.AreEqual (1, positions.Count);
      Assert.AreEqual ("Official", positions[0].Name);
    }

    [Test]
    public void GetPossiblePositions_WithoutGroupType ()
    {
      IPrincipal principal = new GenericPrincipal (new GenericIdentity ("group0/user1"), new string[0]);
      SetupResult.For (_mockUserProvider.GetUser ()).Return (principal);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principal, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principal);
      Role role = Role.NewObject();
      Group rootGroup = Group.GetObject (_expectedRootGroupID);
      _mocks.ReplayAll ();

      List<Position> positions = role.GetPossiblePositions (rootGroup);

      _mocks.VerifyAll ();
      Assert.AreEqual (2, positions.Count);
      foreach (string positionName in new string[] { "Official", "Global" })
      {
        Assert.IsTrue (
            positions.Exists (delegate (Position current) { return positionName == current.Name; }),
            "Position '{0}' was not found.",
            positionName);
      }
    }

    [Test]
    public void GetPossiblePositions_WithoutSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      Role role = Role.NewObject();
      Group parentGroup = Group.GetObject (_expectedParentGroup0ID);

      List<Position> positions = role.GetPossiblePositions (parentGroup);

      Assert.AreEqual (2, positions.Count);
      foreach (string positionName in new string[] { "Official", "Manager" })
      {
        Assert.IsTrue (
            positions.Exists (delegate (Position current) { return positionName == current.Name; }),
            "Position '{0}' was not found.",
            positionName);
      }
    }

    [Test]
    public void GetPossiblePositions_WithoutGroupTypeAndWithoutSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      Role role = Role.NewObject();
      Group rootGroup = Group.GetObject (_expectedRootGroupID);

      List<Position> positions = role.GetPossiblePositions (rootGroup);

      Assert.AreEqual (3, positions.Count);
    }

    [Test]
    public void SearchGroups ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (RolePropertiesSearchService), new RolePropertiesSearchService ());
      IBusinessObjectClass roleClass = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (Role));
      IBusinessObjectReferenceProperty groupProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Group");
      Assert.That (groupProperty, Is.Not.Null);

      User user = User.FindByUserName ("group0/user1");
      Assert.That (user, Is.Not.Null);
      Role role = Role.NewObject();
      role.User = user;
      List<Group> expectedGroups = role.GetPossibleGroups (user.Tenant.ID);
      Assert.That (expectedGroups, Is.Not.Empty);

      Assert.That (groupProperty.SupportsSearchAvailableObjects (true), Is.True);

      IBusinessObject[] actualGroups = groupProperty.SearchAvailableObjects (role, true, null);
      Assert.That (actualGroups, Is.EquivalentTo (expectedGroups));
    }

    [Test]
    public void SearchUsers ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (typeof (RolePropertiesSearchService), new RolePropertiesSearchService ());
      IBusinessObjectClass roleClass = BindableObjectProvider.GetBindableObjectClassFromProvider (typeof (Role));
      IBusinessObjectReferenceProperty userProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("User");
      Assert.That (userProperty, Is.Not.Null);

      Group group = Group.FindByUnqiueIdentifier ("UID: group0");
      Assert.That (group, Is.Not.Null);
      Role role = Role.NewObject ();
      role.Group = group;
      DomainObjectCollection expectedGroups = User.FindByTenantID (group.Tenant.ID);
      Assert.That (expectedGroups, Is.Not.Empty);

      Assert.That (userProperty.SupportsSearchAvailableObjects (true), Is.True);

      IBusinessObject[] actualUsers = userProperty.SearchAvailableObjects (role, true, null);
      Assert.That (actualUsers, Is.EquivalentTo (expectedGroups));
    }

    private void ExpectSecurityProviderGetAccessForGroup (string owningGroup, string owningTenant, IPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Group);
      string owner = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      List<Enum> abstractRoles = new List<Enum> ();
      SecurityContext securityContext = new SecurityContext (classType, owner, owningGroup, owningTenant, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll<Enum, AccessType> (returnedAccessTypeEnums, AccessType.Get);

      Expect.Call (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }

    private void SetupResultSecurityProviderGetAccessForPosition (Delegation delegation, IPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Position);
      string owner = string.Empty;
      string owningGroup = string.Empty;
      string owningTenant = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Delegation", delegation);
      List<Enum> abstractRoles = new List<Enum> ();
      SecurityContext securityContext = new SecurityContext (classType, owner, owningGroup, owningTenant, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll<Enum, AccessType> (returnedAccessTypeEnums, AccessType.Get);

      SetupResult.For (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }
  }
}
