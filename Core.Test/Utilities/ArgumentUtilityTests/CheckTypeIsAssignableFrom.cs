using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckTypeIsAssignableFrom
	{
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail ()
    {
      ArgumentUtility.CheckTypeIsAssignableFrom ("arg", typeof (object), typeof (string));
    }
    [Test]
    public void Succeed_Null ()
    {
      Type result = ArgumentUtility.CheckTypeIsAssignableFrom ("arg", null, typeof (object));
      Assert.That (result, Is.Null);
    }

	  [Test]
    public void Succeed ()
    {
      Type result = ArgumentUtility.CheckTypeIsAssignableFrom ("arg", typeof (string), typeof (object));
      Assert.That (result, Is.SameAs (typeof (string)));
    }
	}
}
