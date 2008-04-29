using System;
using System.Security.Principal;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.Security.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.Data.DomainObjects.UnitTests.Security.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class PropertyValueReadingTest
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
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], ValueAccess.Current);
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
        _extension.PropertyValueReading (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], ValueAccess.Current);
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

      _extension.PropertyValueReading (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.NonSecurableObject.StringProperty"], ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate (ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        Dev.Null = securableObject.OtherStringProperty;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (new RootClientTransaction(), dataContainer, dataContainer.PropertyValues["Remotion.Data.DomainObjects.UnitTests.Security.TestDomain.SecurableObject.StringProperty"], ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      Dev.Null = securableObject.StringProperty;

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_ID ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      Dev.Null = securableObject.ID;

      _testHelper.VerifyAll ();
    }
  }
}