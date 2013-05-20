// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Specialized;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Rhino.Mocks;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.UnitTests.Domain;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;
using log4net.Filter;
using SecurityContext=Remotion.Security.SecurityContext;

namespace Remotion.SecurityManager.UnitTests
{
  [TestFixture]
  public class SecurityServiceTest : DomainTest
  {
    private MockRepository _mocks;
    private IAccessControlListFinder _mockAclFinder;
    private ISecurityTokenBuilder _mockTokenBuilder;

    private SecurityService _service;
    private SecurityContext _context;
    private Tenant _tenant;
    private AccessControlEntry _ace;
    private ISecurityPrincipal _principalStub;

    private MemoryAppender _memoryAppender;
    private ClientTransaction _clientTransaction;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

     _mocks = new MockRepository ();
     _mockAclFinder = _mocks.StrictMock<IAccessControlListFinder> ();
     _mockTokenBuilder = _mocks.StrictMock<ISecurityTokenBuilder> ();

     _service = new SecurityService ("name", new NameValueCollection(), _mockAclFinder, _mockTokenBuilder);
     _context = SecurityContext.Create(typeof (Order), "Owner", "UID: OwnerGroup", "OwnerTenant", new Dictionary<string, Enum>(), new Enum[0]);

      _clientTransaction = ClientTransaction.CreateRootTransaction ();
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        OrganizationalStructureFactory organizationalStructureFactory = new OrganizationalStructureFactory();
        _tenant = organizationalStructureFactory.CreateTenant();
        _ace = CreateAce();
      }
      
      _principalStub = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (_principalStub.User).Return ("group0/user1");
  
      _memoryAppender = new MemoryAppender();
      
      LoggerMatchFilter acceptFilter = new LoggerMatchFilter ();
      acceptFilter.LoggerToMatch = "Remotion.SecurityManager";
      acceptFilter.AcceptOnMatch = true;
      _memoryAppender.AddFilter (acceptFilter);

      DenyAllFilter denyFilter = new DenyAllFilter();
      _memoryAppender.AddFilter (denyFilter);

      BasicConfigurator.Configure(_memoryAppender); 
    }

    public override void TearDown()
    {
      base.TearDown();
      LogManager.ResetConfiguration();
      SecurityConfiguration.Current.SecurityProvider = null;
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      SecurityService provider = new SecurityService ("Provider", config);

      Assert.That (provider.Name, Is.EqualTo ("Provider"));
      Assert.That (provider.Description, Is.EqualTo ("The Description"));
    }

    [Test]
    public void GetAccess_WithoutAccess ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        SecurityToken token = SecurityToken.Create(Principal.Create (_tenant, null, new Role[0]),
                                     null,
                                     null,
                                     null,
                                     Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());

        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Return (CreateAcl (_ace));
        Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context)).Return (token);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.That (accessTypes.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetAccess_WithReadAccess ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var roles = new List<IDomainObjectHandle<AbstractRoleDefinition>>();
        roles.Add (_ace.SpecificAbstractRole.GetHandle());
        SecurityToken token = SecurityToken.Create(Principal.Create (_tenant, null, new Role[0]), null, null, null, roles);

        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Return (CreateAcl (_ace));
        Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context)).Return (token);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.That (accessTypes.Length, Is.EqualTo (1));
      Assert.That (accessTypes, Has.Member (AccessType.Get (EnumWrapper.Get ("Read|MyTypeName"))));
    }

    [Test]
    public void GetAccess_WithReadAccessFromInterface ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var roles = new List<IDomainObjectHandle<AbstractRoleDefinition>>();
        roles.Add (_ace.SpecificAbstractRole.GetHandle());
        SecurityToken token = SecurityToken.Create(Principal.Create (_tenant, null, new Role[0]), null, null, null, roles);

        Expect.Call (_mockAclFinder.Find (null, null)).Return (CreateAcl (_ace)).Constraints (
            Mocks_Is.NotNull(),
            Mocks_Is.Same (_context));
        Expect.Call (_mockTokenBuilder.CreateToken (null, null)).Return (token).Constraints (
            Mocks_Is.Same (_principalStub),
            Mocks_Is.Same (_context));
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.That (accessTypes.Length, Is.EqualTo (1));
      Assert.That (accessTypes, Has.Member (AccessType.Get (EnumWrapper.Get ("Read|MyTypeName"))));
    }

    [Test]
    public void GetAccess_WithAccessControlExcptionFromAccessControlListFinder ()
    {
      AccessControlException expectedException = new AccessControlException();
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Throw (expectedException);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.That (accessTypes.Length, Is.EqualTo (0));
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].ExceptionObject, Is.SameAs (expectedException));
      Assert.That (events[0].Level, Is.EqualTo (Level.Error));
    }

    [Test]
    public void GetAccess_WithAccessControlExcptionFromSecurityTokenBuilder ()
    {
      AccessControlException expectedException = new AccessControlException();
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Return (CreateAcl (_ace));
        Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context)).Throw (expectedException);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.That (accessTypes.Length, Is.EqualTo (0));
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.That (events.Length, Is.EqualTo (1));
      Assert.That (events[0].ExceptionObject, Is.SameAs (expectedException));
      Assert.That (events[0].Level, Is.EqualTo (Level.Error));
    }

    [Test]
    [Ignore ("RM-5522: Temporarily disabled")]
    public void GetAccess_UsesSecurityFreeSection ()
    {
      ClientTransaction subTransaction;
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        var abstractRoles = new List<IDomainObjectHandle<AbstractRoleDefinition>>();
        abstractRoles.Add (_ace.SpecificAbstractRole.GetHandle());

        _ace.GroupCondition = GroupCondition.AnyGroupWithSpecificGroupType;
        _ace.SpecificGroupType = GroupType.NewObject();
        OrganizationalStructureFactory organizationalStructureFactory = new OrganizationalStructureFactory();
        var role = Role.NewObject();
        role.Group = organizationalStructureFactory.CreateGroup();
        role.Group.Tenant = _tenant;
        role.Position = organizationalStructureFactory.CreatePosition();

        var token = SecurityToken.Create(Principal.Create (_tenant, null, new[] { role }), null, null, null, abstractRoles);

        subTransaction = _clientTransaction.CreateSubTransaction ();
        using (subTransaction.EnterNonDiscardingScope())
        {
          Expect.Call (_mockAclFinder.Find (subTransaction, _context))
              .WhenCalled (invocation => Assert.That (SecurityFreeSection.IsActive, Is.True))
              .Return (CreateAcl (_ace));
          Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context))
              .WhenCalled (invocation => Assert.That (SecurityFreeSection.IsActive, Is.True))
              .Return (token);
        }
      }
      SecurityConfiguration.Current.SecurityProvider = MockRepository.GenerateStub<ISecurityProvider> ();
      _clientTransaction.Extensions.Add (new SecurityClientTransactionExtension ());

      _mocks.ReplayAll ();

      _service.GetAccess (subTransaction, _context, _principalStub);

      _mocks.VerifyAll();
    }

    [Test]
    public void GetRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      Assert.That (_service.GetRevision (), Is.EqualTo (0));
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That (((IRevisionBasedSecurityProvider) _service).IsNull, Is.False);
    }

    [Test]
    public void GetAccess_WithInactiveTransaction ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        SecurityToken token = SecurityToken.Create (
            Principal.Create (_tenant, null, new Role[0]),
            null,
            null,
            null,
            Enumerable.Empty<IDomainObjectHandle<AbstractRoleDefinition>>());

        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Return (CreateAcl (_ace));
        Expect.Call (_mockTokenBuilder.CreateToken (_principalStub, _context)).Return (token);
      }
      _mocks.ReplayAll ();

      using (ClientTransactionTestHelper.MakeInactive (_clientTransaction))
      {
        Assert.That (() => _service.GetAccess (_clientTransaction, _context, _principalStub), Throws.Nothing);
      }

      _mocks.VerifyAll ();
    }


    private AccessControlList CreateAcl (AccessControlEntry ace)
    {
      AccessControlList acl = StatefulAccessControlList.NewObject ();
      acl.AccessControlEntries.Add (ace);

      return acl;
    }

    private AccessControlEntry CreateAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AbstractRoleDefinition abstractRole = AbstractRoleDefinition.NewObject (Guid.NewGuid (), "QualityManager", 0);
      ace.SpecificAbstractRole = abstractRole;

      AccessTypeDefinition readAccessType = AccessTypeDefinition.NewObject (Guid.NewGuid (), "Read|MyTypeName", 0);
      AccessTypeDefinition writeAccessType = AccessTypeDefinition.NewObject (Guid.NewGuid (), "Write|MyTypeName", 1);
      AccessTypeDefinition deleteAccessType = AccessTypeDefinition.NewObject (Guid.NewGuid (), "Delete|MyTypeName", 2);

      ace.AddAccessType (readAccessType);
      ace.AddAccessType (writeAccessType);
      ace.AddAccessType (deleteAccessType);

      ace.AllowAccess (readAccessType);

      return ace;
    }
  }
}
