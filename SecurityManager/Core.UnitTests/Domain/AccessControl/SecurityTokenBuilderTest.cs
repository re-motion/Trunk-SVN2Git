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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class SecurityTokenBuilderTest : DomainTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    [Test]
    public void Create_WithValidPrincipal ()
    {
      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = CreateTestPrincipal ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant, Is.SameAs (token.Principal.User.Tenant));
      Assert.That (token.Principal.Roles, Is.Not.Empty);
      Assert.That (token.Principal.Roles, Is.EquivalentTo (token.Principal.User.Roles));
    }

    [Test]
    public void Create_WithValidPrincipal_WithRole ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return ("test.user");
      var princialRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole>();
      princialRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialRoleStub.Stub (stub => stub.Position).Return ("UID: Official");
      principalStub.Stub (stub => stub.Role).Return (princialRoleStub);
      
      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant, Is.SameAs (token.Principal.User.Tenant));
      Assert.That (token.Principal.Roles.Count, Is.EqualTo (1));
      Assert.That (token.Principal.Roles[0].Group.UniqueIdentifier, Is.EqualTo ("UID: testGroup"));
      Assert.That (token.Principal.Roles[0].Position.UniqueIdentifier, Is.EqualTo ("UID: Official"));
      Assert.That (token.Principal.Roles[0].User, Is.SameAs (token.Principal.User));
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedRole ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Official");
      principalStub.Stub (stub => stub.SubstitutedRole).Return (princialSubstitutedRoleStub);

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles.Count, Is.EqualTo (1));
      Assert.That (token.Principal.Roles[0].Group.UniqueIdentifier, Is.EqualTo ("UID: testGroup"));
      Assert.That (token.Principal.Roles[0].Position.UniqueIdentifier, Is.EqualTo ("UID: Official"));
      Assert.That (token.Principal.Roles[0].User.UserName, Is.EqualTo ("test.user"));
    }

    [Test]
    public void Create_WithValidPrincipal_WithSubstitutedUser ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant.UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Not.Empty);
      Assert.That (token.Principal.Roles, Is.EquivalentTo (token.Principal.User.Roles));
    }

    [Test]
    public void Create_WithValidPrincipal_WithInactiveSubstitution ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Manager");
      principalStub.Stub (stub => stub.SubstitutedRole).Return (princialSubstitutedRoleStub);

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedUser ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("notexisting.user");

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedRoleFromGroup ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: notexisting.group");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Official");
      principalStub.Stub (stub => stub.SubstitutedRole).Return (princialSubstitutedRoleStub);

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
    }

    [Test]
    public void Create_WithValidPrincipal_WithInvalidSubstitutedRoleFromPosition ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: notexisting.position");
      principalStub.Stub (stub => stub.SubstitutedRole).Return (princialSubstitutedRoleStub);

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant.UniqueIdentifier, Is.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
    }

    [Test]
    public void Create_WithValidPrincipal_WithTenantMismatch ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("User.Tenant2");
      principalStub.Stub (stub => stub.SubstitutedUser).Return ("test.user");

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant, Is.Not.Null);
      Assert.That (token.Principal.Tenant.UniqueIdentifier, Is.Not.EqualTo ("UID: testTenant"));
      Assert.That (token.Principal.Roles, Is.Empty);
    }

    [Test]
    public void Create_WithNullPrincipal ()
    {
      SecurityContext context = CreateContext ();
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.IsNull).Return (true);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principalStub, context);

      Assert.That (token.Principal.User, Is.Null);
      Assert.That (token.Principal.Tenant, Is.Null);
      Assert.That (token.Principal.Roles, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The user 'notexisting.user' could not be found.")]
    public void Create_WithInvalidPrincipal_InvalidUserName ()
    {
      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = CreatePrincipal ("notexisting.user");

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "No principal was provided.")]
    public void Create_WithInvalidPrincipal_EmptyUserName ()
    {
      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = CreatePrincipal ("");

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "A substituted role was specified without a substituted user.")]
    public void Create_WithInvalidPrincipal_WithSubstitutedRoleButNoSubstitutedUser ()
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      principalStub.Stub (stub => stub.User).Return ("substituting.user");
      principalStub.Stub (stub => stub.SubstitutedUser).Return (null);
      var princialSubstitutedRoleStub = MockRepository.GenerateStub<ISecurityPrincipalRole> ();
      princialSubstitutedRoleStub.Stub (stub => stub.Group).Return ("UID: testGroup");
      princialSubstitutedRoleStub.Stub (stub => stub.Position).Return ("UID: Official");
      principalStub.Stub (stub => stub.SubstitutedRole).Return (princialSubstitutedRoleStub);

      SecurityContext context = CreateContext ();
      ISecurityPrincipal principal = principalStub;

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);
    }

    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      SecurityContext context = CreateContext();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestPrincipal(), context);

      Assert.IsEmpty (token.AbstractRoles);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      SecurityContext context = CreateContext (ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestPrincipal(), context);

      Assert.AreEqual (1, token.AbstractRoles.Count);
      Assert.AreEqual (
          "QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests", token.AbstractRoles[0].Name);
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      SecurityContext context = CreateContext (ProjectRoles.QualityManager, ProjectRoles.Developer);

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestPrincipal(), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage =
        "The abstract role 'Undefined|Remotion.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Remotion.SecurityManager.UnitTests' could not be found."
        )]
    public void Create_WithNotExistingAbstractRole ()
    {
      SecurityContext context = CreateContext (ProjectRoles.Developer, UndefinedAbstractRoles.Undefined, ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestPrincipal(), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidOwningTenant ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNotNull (token.OwningTenant);
      Assert.AreEqual ("UID: testTenant", token.OwningTenant.UniqueIdentifier);
    }

    [Test]
    public void Create_WithoutOwningTenant ()
    {
      SecurityContext context = CreateContextWithoutOwningTenant();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNull (token.OwningTenant);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The tenant 'UID: NotExistingTenant' could not be found.")]
    public void Create_WithNotExistingOwningTenant ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningTenant();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningGroup ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Group group = token.OwningGroup;
      Assert.That (group, Is.Not.Null);
      Assert.That (group.UniqueIdentifier, Is.EqualTo ("UID: testOwningGroup"));
    }

    [Test]
    public void Create_WithoutOwningGroup ()
    {
      SecurityContext context = CreateContextWithoutOwningGroup();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNull (token.OwningGroup);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The group 'UID: NotExistingGroup' could not be found.")]
    public void Create_WithNotExistingOwningGroup ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningGroup();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningUser ()
    {
      SecurityContext context = CreateContext();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      User owningUser = token.OwningUser;
      Assert.That (owningUser, Is.Not.Null);
      Assert.That (owningUser.UserName, Is.EqualTo ("group0/user1"));
    }

    [Test]
    public void Create_WithoutOwningUser ()
    {
      SecurityContext context = CreateContextWithoutOwningUser();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNull (token.OwningUser);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The user 'notExistingUser' could not be found.")]
    public void Create_WithNotExistingOwningUser ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningUser();
      ISecurityPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    private ISecurityPrincipal CreateTestPrincipal ()
    {
      return CreatePrincipal ("test.user");
    }

    private ISecurityPrincipal CreatePrincipal (string userName)
    {
      var principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      principalStub.Stub (stub => stub.User).Return (userName);
      return principalStub;
    }

    private SecurityContext CreateContext (params Enum[] abstractRoles)
    {
      return SecurityContext.Create (
          typeof (Order), "group0/user1", "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), abstractRoles);
    }

    private SecurityContext CreateContextWithoutOwningTenant ()
    {
      return SecurityContext.Create (typeof (Order), "group0/user1", "UID: testOwningGroup", null, new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningTenant ()
    {
      return SecurityContext.Create (
          typeof (Order), "group0/user1", "UID: testOwningGroup", "UID: NotExistingTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwningGroup ()
    {
      return SecurityContext.Create (typeof (Order), "group0/user1", null, "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningGroup ()
    {
      return SecurityContext.Create (
          typeof (Order), "group0/user1", "UID: NotExistingGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithoutOwningUser ()
    {
      return SecurityContext.Create (typeof (Order), null, "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateContextWithNotExistingOwningUser ()
    {
      return SecurityContext.Create (
          typeof (Order), "notExistingUser", "UID: testOwningGroup", "UID: testTenant", new Dictionary<string, Enum>(), new Enum[0]);
    }
  }
}