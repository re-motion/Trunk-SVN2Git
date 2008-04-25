using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullOrEmptyOrItemsNull
	{
	  [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullICollection ()
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", (ICollection) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException))]
    public void Fail_zItemNullICollection ()
    {
      ArrayList list = new ArrayList ();
      list.Add (null);
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", list);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyArray ()
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", new string[0]);
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ArgumentEmptyException))]
    public void Fail_EmptyCollection ()
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", new ArrayList ());
    }

    [Test]
    public void Succeed_Array ()
    {
      string[] array = new string[] { "test" };
      string[] result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", array);
      Assert.That (result, Is.SameAs (array));
    }

    [Test]
    public void Succeed_Collection ()
    {
      ArrayList list = new ArrayList ();
      list.Add ("test");
      ArrayList result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }
  }
}
