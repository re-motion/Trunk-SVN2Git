// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Configuration;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchPosition : SearchServiceTestBase
  {
    private ObjectID _expectedTenantID;
    private ObjectID _expectedRootGroupID;
    private ObjectID _expectedParentGroup0ID;
    private ObjectID _expectedGroup0ID;
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private IPrincipalProvider _mockPrincipalProvider;
    private IFunctionalSecurityStrategy _stubFunctionalSecurityStrategy;
    private RolePropertiesSearchService _searchService;
    private IBusinessObjectReferenceProperty _positionProperty;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransactionScope.CurrentTransaction);
        _expectedTenantID = tenant.ID;

        var groups = Group.FindByTenantID (_expectedTenantID);
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
      _mockPrincipalProvider = (IPrincipalProvider) _mocks.StrictMultiMock (typeof (ProviderBase), typeof (IPrincipalProvider));
      _stubFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy>();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.PrincipalProvider = _mockPrincipalProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _stubFunctionalSecurityStrategy;

      _searchService = new RolePropertiesSearchService();

      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      _positionProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Position");
      Assert.That (_positionProperty, Is.Not.Null);
    }

    public override void TearDown ()
    {
      base.TearDown();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void LinqTests ()
    {
      var group = Group.GetObject (_expectedParentGroup0ID);
      var groupType = group.GroupType;

      // Idea
      //Select p.* from Position
      //inner join GroupTypePosition gtp on gtp.PositionID = p.ID
      //where gtp.GroupTypeID = 'guid'
      //order by p.Name

      //#1
      var p1 = from p in Position.FindAll()
             where p.GroupTypes.Any (gtp => gtp.GroupType == groupType)
             select p;
      p1.ToArray();

      //SELECT [t0].[ID],[t0].[ClassID],[t0].[Timestamp],[t0].[Name],[t0].[UniqueIdentifier],[t0].[Delegation] 
      //FROM [PositionView] AS [t0] 
      //WHERE EXISTS((
      //    SELECT [t1].[ID] FROM [GroupTypePositionView] AS [t1] 
      //    WHERE (([t0].[ID] = [t1].[PositionID]) AND ([t1].[GroupTypeID] = 'guid'))
      //)) 
      //ORDER BY [t0].[Name] ASC

      //#2
      var p2 = Position.FindAll().SelectMany (p => p.GroupTypes).Where (gtp => gtp.GroupType == groupType).Select (gtp => gtp.Position);
       p2.ToArray();

      //SELECT [t2].[ID],[t2].[ClassID],[t2].[Timestamp],[t2].[Name],[t2].[UniqueIdentifier],[t2].[Delegation] 
      //FROM [PositionView] AS [t0] 
      //CROSS JOIN [GroupTypePositionView] AS [t1] 
      //LEFT OUTER JOIN [PositionView] AS [t2] ON [t1].[PositionID] = [t2].[ID] 
      //WHERE (([t0].[ID] = [t1].[PositionID]) AND ([t1].[GroupTypeID] = 'guid')) 
      //ORDER BY [t0].[Name] ASC
    }

    [Test]
    public void Search_WithoutGroup ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider();
      SecurityConfiguration.Current.PrincipalProvider = new ThreadPrincipalProvider();

      var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (null));

      Assert.AreEqual (3, positions.Length);
    }

    [Test]
    public void Search_WithGroupType ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider();
      SecurityConfiguration.Current.PrincipalProvider = new ThreadPrincipalProvider();
      Group parentGroup = Group.GetObject (_expectedParentGroup0ID);

       var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (parentGroup));

      Assert.AreEqual (2, positions.Length);
      foreach (string positionName in new[] { "Official", "Manager" })
      {
        Assert.IsTrue (
            Array.Exists (positions, current => positionName == ((Position) current).Name),
            "Position '{0}' was not found.",
            positionName);
      }
    }

    [Test]
    public void Search_WithoutGroupType ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider();
      SecurityConfiguration.Current.PrincipalProvider = new ThreadPrincipalProvider();
      Group rootGroup = Group.GetObject (_expectedRootGroupID);

       var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (rootGroup));

      Assert.AreEqual (3, positions.Length);
    }

    [Test]
    public void Search_WithoutGroupType_AndWithSecurityProvider ()
    {
      var principalStub = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (principalStub.User).Return ("group0/user1");
      SetupResult.For (_mockPrincipalProvider.GetPrincipal ()).Return (principalStub);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principalStub, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principalStub);
      Group rootGroup = Group.GetObject (_expectedRootGroupID);
      _mocks.ReplayAll();

      var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (rootGroup));

      _mocks.VerifyAll();
      Assert.AreEqual (2, positions.Length);
      foreach (string positionName in new[] { "Official", "Global" })
      {
        Assert.IsTrue (
            Array.Exists (positions, current => positionName == ((Position) current).Name),
            "Position '{0}' was not found.",
            positionName);
      }
    }

    [Test]
    public void Search_WithGroupType_AndWithSecurityProvider ()
    {
      var principalStub = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (principalStub.User).Return ("group0/user1");
      SetupResult.For (_mockPrincipalProvider.GetPrincipal ()).Return (principalStub);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principalStub, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principalStub);
      Group parentGroup = Group.GetObject (_expectedParentGroup0ID);
      _mocks.ReplayAll();

      var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (parentGroup));

      _mocks.VerifyAll();
      Assert.AreEqual (1, positions.Length);
      Assert.AreEqual ("Official", ((Position)positions[0]).Name);
    }

    private void SetupResultSecurityProviderGetAccessForPosition (Delegation delegation, ISecurityPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Position);
      string owner = string.Empty;
      string owningGroup = string.Empty;
      string owningTenant = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add ("Delegation", delegation);
      List<Enum> abstractRoles = new List<Enum>();
      SecurityContext securityContext = SecurityContext.Create (classType, owner, owningGroup, owningTenant, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll (returnedAccessTypeEnums, AccessType.Get);

      SetupResult.For (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }

    private ISearchAvailableObjectsArguments CreateSearchArguments (Group group)
    {
      if (group == null)
        return null;
      return new DefaultSearchArguments (group.ID.ToString());
    }
  }
}
