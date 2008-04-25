using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class PermissionTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedTrue ()
    {
      Permission permission = Permission.NewObject();
      permission.Allowed = true;

      Assert.IsTrue (permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedFalse ()
    {
      Permission permission = Permission.NewObject();
      permission.Allowed = false;

      Assert.IsFalse (permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedNull ()
    {
      Permission permission = Permission.NewObject();
      permission.Allowed = null;

      Assert.IsFalse (permission.BinaryAllowed);
    }

    [Test]
    public void SetBinaryAllowed_FromTrue()
    {
      Permission permission = Permission.NewObject();
      permission.BinaryAllowed = true;

      Assert.AreEqual (true, permission.Allowed);
    }

    [Test]
    public void SetBinaryAllowed_FromFalse ()
    {
      Permission permission = Permission.NewObject();
      permission.BinaryAllowed = false;

      Assert.IsNull (permission.Allowed);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      Permission permission = Permission.NewObject();

      permission.Index = 1;
      Assert.AreEqual (1, permission.Index);
    }
  }
}
