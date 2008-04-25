using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  [TestFixture]
  public class GetRequiredMethodPermissionsTest
  {
    private IPermissionProvider _permissionReflector;

    [SetUp]
    public void SetUp ()
    {
      _permissionReflector = new PermissionReflector ();
    }

    [TearDown]
    public void TearDown ()
    {
      TestPermissionReflector.Cache.Clear ();
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new PermissionReflector ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void Test_MethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Save"));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Delete");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Delete, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_MethodWithTwoPermissions ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Create, requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Record"));
    }

    [Test]
    public void Test_MethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Show");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Create, requiredAccessTypes);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Sve' could not be found.\r\nParameter name: memberName")]
    public void Test_NotExistingMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Sve");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Send' has multiple DemandMethodPermissionAttribute defined.\r\nParameter name: memberName")]
    public void Test_PermissionsDeclaredOnBaseAndDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Send");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Load' has multiple DemandMethodPermissionAttribute defined.\r\nParameter name: memberName")]
    public void Test_PermissionsDeclaredOnOverloads ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Load");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
      ExpectedMessage = "The DemandMethodPermissionAttribute must not be defined on members overriden or redefined in derived classes. "
        + "A member 'Print' exists in class 'Remotion.Security.UnitTests.Core.SampleDomain.DerivedSecurableObject' and its base class."
        + "\r\nParameter name: memberName")]
    public void Test_VirtualMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (DerivedSecurableObject), "Print");
    }

    [Test]
    public void FilterMultipleAccessTypes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), "Close");

      Assert.AreEqual (2, requiredAccessTypes.Length);
      Assert.Contains (GeneralAccessTypes.Edit, requiredAccessTypes);
      Assert.Contains (GeneralAccessTypes.Find, requiredAccessTypes);
    }
  }
}
