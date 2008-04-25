using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullOrItemsNull
	{
    [Test]
    public void Succeed_ICollection ()
    {
      ArrayList list = new ArrayList ();
      ArrayList result = ArgumentUtility.CheckNotNullOrItemsNull ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }

	  [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullICollection ()
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", (ICollection) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentItemNullException))]
    public void Fail_zItemNullICollection ()
    {
      ArrayList list = new ArrayList ();
      list.Add (null);
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", list);
    }
	}
}
