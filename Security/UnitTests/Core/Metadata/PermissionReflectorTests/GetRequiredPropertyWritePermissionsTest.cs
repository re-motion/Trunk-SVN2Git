using System;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  [TestFixture]
  public class GetRequiredPropertyWritePermissionsTest
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
    public void Test_PropertyWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsEnabled");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForPropertyWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsEnabled");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsEnabled"));
    }

    [Test]
    public void Test_PropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsVisible");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (TestAccessTypes.Fourth, requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForPropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsVisible");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "IsVisible"));
    }

    [Test]
    public void Test_NonPublicPropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), "NonPublicProperty");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (TestAccessTypes.Second, requiredAccessTypes);
    }

    [Test]
    public void Test_ExplicitInterfacePropertyWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), 
          typeof (IInterfaceWithProperty).FullName + ".InterfaceProperty");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.Contains (TestAccessTypes.Second, requiredAccessTypes);
    }
  }
}
