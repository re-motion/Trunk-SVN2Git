using System;
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.DomainObjects.UnitTests.Security.TestDomain;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.UnitTests.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class PropertyValueChangingTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;
    private IDisposable _transactionScope;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
      _transactionScope = _testHelper.Transaction.EnterDiscardingScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
      _transactionScope.Dispose ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.PropertyValueChanging (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      DataContainer dataContainer = nonSecurableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.NonSecurableObject.StringProperty"], "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("StringProperty", TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate (ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        securableObject.OtherStringProperty = "dummy";
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.PropertyValueChanging (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], "old", "new");

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyWritePermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      securableObject.StringProperty = "new";

      _testHelper.VerifyAll ();
    }
  }
}