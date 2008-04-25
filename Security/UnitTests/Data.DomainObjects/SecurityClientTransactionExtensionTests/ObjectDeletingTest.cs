using System;
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Security.Data.DomainObjects;
using Remotion.Security.UnitTests.Data.DomainObjects.TestDomain;

namespace Remotion.Security.UnitTests.Data.DomainObjects.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class ObjectDeletingTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
      _testHelper.Transaction.EnterDiscardingScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, true);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, false);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.ObjectDeleting (_testHelper.Transaction, securableObject);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, nonSecurableObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      SecurableObject otherObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      HasAccessDelegate hasAccess = delegate (ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        otherObject.Delete ();
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, hasAccess);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.Transaction.Commit ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Delete, true);
      _testHelper.ReplayAll ();

      securableObject.Delete ();

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNewObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.ObjectDeleting (_testHelper.Transaction, securableObject);

      _testHelper.VerifyAll ();
    }
  }
}