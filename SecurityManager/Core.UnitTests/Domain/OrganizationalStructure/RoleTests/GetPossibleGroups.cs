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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Configuration;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RoleTests
{
  [TestFixture]
  public class GetPossibleGroups : RoleTestBase
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
      base.TestFixtureSetUp();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
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
      base.SetUp();

      _mocks = new MockRepository();
      _mockSecurityProvider = (ISecurityProvider) _mocks.StrictMultiMock (typeof (ProviderBase), typeof (ISecurityProvider));
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockUserProvider = (IUserProvider) _mocks.StrictMultiMock (typeof (ProviderBase), typeof (IUserProvider));
      _stubFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy>();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _mockUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _stubFunctionalSecurityStrategy;
    }

    public override void TearDown ()
    {
      base.TearDown();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void Test ()
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
      _mocks.ReplayAll();

      List<Group> groups = role.GetPossibleGroups (_expectedTenantID);

      _mocks.VerifyAll();
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
    public void Test_WithoutSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider();
      Role role = Role.NewObject();

      List<Group> groups = role.GetPossibleGroups (_expectedTenantID);

      Assert.AreEqual (9, groups.Count);
    }

    private void ExpectSecurityProviderGetAccessForGroup (
        string owningGroup, string owningTenant, IPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Group);
      string owner = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      List<Enum> abstractRoles = new List<Enum>();
      SecurityContext securityContext = SecurityContext.Create (classType, owner, owningGroup, owningTenant, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll<Enum, AccessType> (returnedAccessTypeEnums, AccessType.Get);

      Expect.Call (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }
  }
}