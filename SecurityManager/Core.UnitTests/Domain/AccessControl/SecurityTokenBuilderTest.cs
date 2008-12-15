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
using System.Linq;
using System.Security.Principal;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.TestDomain;

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
        "The abstract role 'Undefined|Remotion.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Remotion.SecurityManager.UnitTests' could not be found.")]
    public void Create_WithNotExistingAbstractRole ()
    {
      SecurityContext context = CreateContext (ProjectRoles.Developer, UndefinedAbstractRoles.Undefined, ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestPrincipal(), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidPrincipal ()
    {
      SecurityContext context = CreateContext();
      IPrincipal principal = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);

      Assert.That (token.Principal.User.UserName, Is.EqualTo ("test.user"));
      Assert.That (token.Principal.Tenant, Is.SameAs (token.Principal.User.Tenant));
      Assert.That (token.Principal.Roles, Is.EquivalentTo (token.Principal.User.Roles));
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The user 'notexisting.user' could not be found.")]
    public void Create_WithNotExistingPrincipal ()
    {
      SecurityContext context = CreateContext();
      IPrincipal principal = CreateNotExistingUser();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "No principal was provided.")]
    public void Create_WithEmptyPrincipal ()
    {
      SecurityContext context = CreateContext ();
      IPrincipal principal = CreateUser (string.Empty);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, principal, context);
    }

    [Test]
    public void Create_WithValidOwningTenant ()
    {
      SecurityContext context = CreateContext();
      IPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNotNull (token.OwningTenant);
      Assert.AreEqual ("UID: testTenant", token.OwningTenant.UniqueIdentifier);
    }

    [Test]
    public void Create_WithoutOwningTenant ()
    {
      SecurityContext context = CreateContextWithoutOwningTenant();
      IPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNull (token.OwningTenant);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The tenant 'UID: NotExistingTenant' could not be found.")]
    public void Create_WithNotExistingOwningTenant ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningTenant();
      IPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningGroup ()
    {
      SecurityContext context = CreateContext();
      IPrincipal user = CreateTestPrincipal();

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
      IPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNull (token.OwningGroup);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The group 'UID: NotExistingGroup' could not be found.")]
    public void Create_WithNotExistingOwningGroup ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningGroup();
      IPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningUser ()
    {
      SecurityContext context = CreateContext();
      IPrincipal user = CreateTestPrincipal();

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
      IPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNull (token.OwningUser);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The user 'notExistingUser' could not be found.")]
    public void Create_WithNotExistingOwningUser ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningUser();
      IPrincipal user = CreateTestPrincipal();

      SecurityTokenBuilder builder = new SecurityTokenBuilder();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    private IPrincipal CreateTestPrincipal ()
    {
      return CreateUser ("test.user");
    }

    private IPrincipal CreateNotExistingUser ()
    {
      return CreateUser ("notexisting.user");
    }

    private IPrincipal CreateUser (string userName)
    {
      return new GenericPrincipal (new GenericIdentity (userName), new string[0]);
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