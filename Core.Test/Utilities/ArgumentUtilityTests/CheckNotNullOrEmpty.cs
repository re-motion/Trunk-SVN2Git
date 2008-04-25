using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullOrEmpty
  {
    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentNullException))]
    public void Fail_NullString()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", (string) null);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyString()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", "");
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyArray()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", new string[0]);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyCollection()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", new ArrayList());
    }

    [Test]
    public void Succeed_String()
    {
      string result = ArgumentUtility.CheckNotNullOrEmpty ("arg", "Test");
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void Succeed_Array()
    {
      string[] array = new string[] {"test"};
      string[] result = ArgumentUtility.CheckNotNullOrEmpty ("arg", array);
      Assert.That (result, Is.SameAs (array));
    }

    [Test]
    public void Succeed_Collection()
    {
      ArrayList list = new ArrayList();
      list.Add ("test");
      ArrayList result = ArgumentUtility.CheckNotNullOrEmpty ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }
  }
}