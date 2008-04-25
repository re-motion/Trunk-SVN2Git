using System;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  [TestFixture]
  public class GetRequiredStaticMethodPermissionsTest
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
    public void Test_MethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions");

      Assert.IsNotNull (requiredAccessTypes);
      Assert.IsEmpty (requiredAccessTypes);
    }

    [Test]
    public void Test_CacheForMethodWithoutAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CheckPermissions"));
    }

    [Test]
    public void Test_MethodWithOneAttribute ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Create, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_CacheForMethodWithOneAttribut ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase");

      Assert.AreSame (requiredAccessTypes, _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "CreateForSpecialCase"));
    }

    [Test]
    public void Test_OverloadedMethodWithOneAttributes ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "IsValid");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Read, requiredAccessTypes[0]);
    }

    [Test]
    public void Test_MethodOfDerivedClass ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (DerivedSecurableObject), "CreateForSpecialCase");

      Assert.AreEqual (1, requiredAccessTypes.Length);
      Assert.AreEqual (GeneralAccessTypes.Create, requiredAccessTypes[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The member 'Sve' could not be found.\r\nParameter name: memberName")]
    public void Test_NotExistingMethod ()
    {
      Enum[] requiredAccessTypes = _permissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), "Sve");
    }
  }
}
