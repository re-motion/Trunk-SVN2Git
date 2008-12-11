// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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
    private OrganizationalStructureTestHelper _testHelper;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
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
      _mockSecurityProvider = (ISecurityProvider) _mocks.StrictMultiMock (typeof (ProviderBase), typeof (ISecurityProvider));
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockUserProvider = (IUserProvider) _mocks.StrictMultiMock (typeof (ProviderBase), typeof (IUserProvider));
      _stubFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy> ();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _stubFunctionalSecurityStrategy;

      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();

      _testHelper = new OrganizationalStructureTestHelper ();
      _testHelper.Transaction.EnterNonDiscardingScope ();
    }

    public override void TearDown ()
    {
      base.TearDown();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
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
      ISearchAvailableObjectsService searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService> ();
      ISearchAvailableObjectsArguments args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments> ();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute> ().AddService (typeof (RolePropertiesSearchService), searchServiceStub);
      IBusinessObjectClass roleClass = BindableObjectProvider.GetBindableObjectClass (typeof (Role));
      IBusinessObjectReferenceProperty groupProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Group");
      Assert.That (groupProperty, Is.Not.Null);

      Role role = Role.NewObject ();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (groupProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (role, groupProperty, args)).Return (expected);

      Assert.That (groupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = groupProperty.SearchAvailableObjects (role, args);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void SearchUsers ()
    {
      ISearchAvailableObjectsService searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService> ();
      ISearchAvailableObjectsArguments args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments> ();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute> ().AddService (typeof (RolePropertiesSearchService), searchServiceStub);
      IBusinessObjectClass roleClass = BindableObjectProvider.GetBindableObjectClass (typeof (Role));
      IBusinessObjectReferenceProperty userProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("User");
      Assert.That (userProperty, Is.Not.Null);

      Role role = Role.NewObject ();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (userProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (role, userProperty, args)).Return (expected);

      Assert.That (userProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = userProperty.SearchAvailableObjects (role, args);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void DeleteRole_WithSubstitution()
    {
      OrganizationalStructureTestHelper testHelper = new OrganizationalStructureTestHelper ();
      using (testHelper.Transaction.EnterNonDiscardingScope ())
      {
        Tenant tenant = testHelper.CreateTenant ("TestTenant", "UID: testTenant");
        Group userGroup = testHelper.CreateGroup ("UserGroup", Guid.NewGuid ().ToString (), null, tenant);
        Group roleGroup = testHelper.CreateGroup ("RoleGroup", Guid.NewGuid ().ToString (), null, tenant);
        User user = testHelper.CreateUser ("user", "Firstname", "Lastname", "Title", userGroup, tenant);
        Position position = testHelper.CreatePosition ("Position");
        Role role = testHelper.CreateRole (user, roleGroup, position);

        Substitution substitution = Substitution.NewObject ();
        substitution.SubstitutedRole = role;

        role.Delete ();

        Assert.IsTrue (substitution.IsDiscarded);
      }
    }

    private void ExpectSecurityProviderGetAccessForGroup (string owningGroup, string owningTenant, IPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Group);
      string owner = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      List<Enum> abstractRoles = new List<Enum> ();
      SecurityContext securityContext = SecurityContext.Create(classType, owner, owningGroup, owningTenant, states, abstractRoles);

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
      SecurityContext securityContext = SecurityContext.Create(classType, owner, owningGroup, owningTenant, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll<Enum, AccessType> (returnedAccessTypeEnums, AccessType.Get);

      SetupResult.For (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }
  }
}
