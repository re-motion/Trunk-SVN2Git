// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
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

     _service = new SecurityService (_mockAclFinder, _mockTokenBuilder);
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
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      SecurityService provider = new SecurityService ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetAccess_WithoutAccess ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        SecurityToken token = new SecurityToken (new Principal (_tenant, null, new Role[0]), null, null, null, new List<AbstractRoleDefinition> ());

        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Return (CreateAcl (_ace));
        Expect.Call (_mockTokenBuilder.CreateToken (ClientTransactionScope.CurrentTransaction, _principalStub, _context)).Return (token);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAccess_WithReadAccess ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        List<AbstractRoleDefinition> roles = new List<AbstractRoleDefinition>();
        roles.Add (_ace.SpecificAbstractRole);
        SecurityToken token = new SecurityToken (new Principal (_tenant, null, new Role[0]), null, null, null, roles);

        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Return (CreateAcl (_ace));
        Expect.Call (_mockTokenBuilder.CreateToken (ClientTransactionScope.CurrentTransaction, _principalStub, _context)).Return (token);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (AccessType.Get (new EnumWrapper ("Read|MyTypeName")), accessTypes);
    }

    [Test]
    public void GetAccess_WithReadAccessFromInterface ()
    {
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        List<AbstractRoleDefinition> roles = new List<AbstractRoleDefinition>();
        roles.Add (_ace.SpecificAbstractRole);
        SecurityToken token = new SecurityToken (new Principal (_tenant, null, new Role[0]), null, null, null, roles);

        Expect.Call (_mockAclFinder.Find (null, null)).Return (CreateAcl (_ace)).Constraints (
            Mocks_Is.NotNull(),
            Mocks_Is.Same (_context));
        Expect.Call (_mockTokenBuilder.CreateToken (null, null, null)).Return (token).Constraints (
            Mocks_Is.NotNull(),
            Mocks_Is.Same (_principalStub),
            Mocks_Is.Same (_context));
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (AccessType.Get (new EnumWrapper ("Read|MyTypeName")), accessTypes);
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
      Assert.AreEqual (0, accessTypes.Length);
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreSame (expectedException, events[0].ExceptionObject);
      Assert.AreEqual (Level.Error, events[0].Level);
    }

    [Test]
    public void GetAccess_WithAccessControlExcptionFromSecurityTokenBuilder ()
    {
      AccessControlException expectedException = new AccessControlException();
      using (_clientTransaction.EnterNonDiscardingScope ())
      {
        Expect.Call (_mockAclFinder.Find (ClientTransactionScope.CurrentTransaction, _context)).Return (CreateAcl (_ace));
        Expect.Call (_mockTokenBuilder.CreateToken (ClientTransactionScope.CurrentTransaction, _principalStub, _context)).Throw (expectedException);
      }
      _mocks.ReplayAll ();

      AccessType[] accessTypes = _service.GetAccess (_clientTransaction, _context, _principalStub);

      _mocks.VerifyAll ();
      Assert.AreEqual (0, accessTypes.Length);
      LoggingEvent[] events = _memoryAppender.GetEvents ();
      Assert.AreEqual (1, events.Length);
      Assert.AreSame (expectedException, events[0].ExceptionObject);
      Assert.AreEqual (Level.Error, events[0].Level);
    }

    [Test]
    public void GetRevision ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      Assert.AreEqual (0, _service.GetRevision ());
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (((IRevisionBasedSecurityProvider) _service).IsNull);
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

      ace.AttachAccessType (readAccessType);
      ace.AttachAccessType (writeAccessType);
      ace.AttachAccessType (deleteAccessType);

      ace.AllowAccess (readAccessType);

      return ace;
    }
  }
}
