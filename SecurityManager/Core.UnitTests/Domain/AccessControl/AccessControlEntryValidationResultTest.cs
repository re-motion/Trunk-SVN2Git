using System;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryValidationResultTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult ();

      Assert.IsTrue (result.IsValid);
      Assert.IsFalse (result.IsSpecificTenantMissing);
    }

    [Test]
    public void IsValid_IsSpecificTenantMissing ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult ();

      result.SetSpecificTenantMissing ();

      Assert.IsFalse (result.IsValid);
      Assert.IsTrue (result.IsSpecificTenantMissing);
    }

  }
}