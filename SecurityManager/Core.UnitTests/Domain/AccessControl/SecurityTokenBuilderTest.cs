using System;
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.UnitTests.TestDomain;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class SecurityTokenBuilderTest : DomainTest
  {
    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransaction.NewRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp ();

      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void Create_AbstractRolesEmpty ()
    {
      SecurityContext context = CreateContext ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestUser (), context);

      Assert.IsEmpty (token.AbstractRoles);
    }

    [Test]
    public void Create_WithValidAbstractRole ()
    {
      SecurityContext context = CreateContext (ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestUser (), context);

      Assert.AreEqual (1, token.AbstractRoles.Count);
      Assert.AreEqual ("QualityManager|Remotion.SecurityManager.UnitTests.TestDomain.ProjectRoles, Remotion.SecurityManager.UnitTests", token.AbstractRoles[0].Name);
    }

    [Test]
    public void Create_WithValidAbstractRoles ()
    {
      SecurityContext context = CreateContext (ProjectRoles.QualityManager, ProjectRoles.Developer);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The abstract role 'Undefined|Remotion.SecurityManager.UnitTests.TestDomain.UndefinedAbstractRoles, Remotion.SecurityManager.UnitTests' could not be found.")]
    public void Create_WithNotExistingAbstractRole ()
    {
      SecurityContext context = CreateContext (ProjectRoles.Developer, UndefinedAbstractRoles.Undefined, ProjectRoles.QualityManager);

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, CreateTestUser (), context);

      Assert.AreEqual (2, token.AbstractRoles.Count);
    }

    [Test]
    public void Create_WithValidUser ()
    {
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.AreEqual ("test.user", token.User.UserName);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The user 'notexisting.user' could not be found.")]
    public void Create_WithNotExistingUser ()
    {
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateNotExistingUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningTenant ()
    {
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNotNull (token.OwningTenant);
      Assert.AreEqual ("UID: testTenant", token.OwningTenant.UniqueIdentifier);
    }

    [Test]
    public void Create_WithoutOwningTenant ()
    {
      SecurityContext context = CreateContextWithoutOwningTenant ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsNull (token.OwningTenant);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The tenant 'UID: NotExistingTenant' could not be found.")]
    public void Create_WithNotExistingOwningTenant ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningTenant ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    [Test]
    public void Create_WithValidOwningGroup ()
    {
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("UID: testOwningGroup", token.OwningGroups);
    }

    [Test]
    public void Create_WithoutOwningGroup ()
    {
      SecurityContext context = CreateContextWithoutOwningGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      Assert.IsEmpty (token.OwningGroups);
    }

    [Test]
    [ExpectedException (typeof (AccessControlException), ExpectedMessage = "The group 'UID: NotExistingGroup' could not be found.")]
    public void Create_WithNotExistingOwningGroup ()
    {
      SecurityContext context = CreateContextWithNotExistingOwningGroup ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);
    }

    [Test]
    public void Create_WithParentOwningGroup ()
    {
      SecurityContext context = CreateContext ();
      IPrincipal user = CreateTestUser ();

      SecurityTokenBuilder builder = new SecurityTokenBuilder ();
      SecurityToken token = builder.CreateToken (ClientTransactionScope.CurrentTransaction, user, context);

      AccessControlObjectAssert.ContainsGroup ("UID: testRootGroup", token.OwningGroups);
    }

    private IPrincipal CreateTestUser ()
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
      return new SecurityContext (typeof (Order), "owner", "UID: testOwningGroup", "UID: testTenant", null, abstractRoles);
    }

    private SecurityContext CreateContextWithoutOwningTenant ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: testOwningGroup", null, null, null);
    }

    private SecurityContext CreateContextWithNotExistingOwningTenant ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: testOwningGroup", "UID: NotExistingTenant", null, null);
    }

    private SecurityContext CreateContextWithoutOwningGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", null, "UID: testTenant", null, null);
    }

    private SecurityContext CreateContextWithNotExistingOwningGroup ()
    {
      return new SecurityContext (typeof (Order), "owner", "UID: NotExistingGroup", "UID: testTenant", null, null);
    }
  }
}
