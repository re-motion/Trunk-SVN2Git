using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNull
  {
    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
    public void Nullable_Fail()
    {
      ArgumentUtility.CheckNotNull ("arg", (int?) null);
    }

    [Test]
    public void Nullable_Succeed()
    {
      int? result = ArgumentUtility.CheckNotNull ("arg", (int?) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Value_Succeed()
    {
      int result = ArgumentUtility.CheckNotNull ("arg", (int) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
    public void Reference_Fail()
    {
      ArgumentUtility.CheckNotNull ("arg", (string) null);
    }

    [Test]
    public void Reference_Succeed()
    {
      string result = ArgumentUtility.CheckNotNull ("arg", string.Empty);
      Assert.That (result, Is.SameAs (string.Empty));
    }
  }
}