using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckNotNullAndTypeIsAssignableFrom
	{
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Null ()
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", null, typeof (string));
    }
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_Type ()
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", typeof (object), typeof (string));
    }
    [Test]
    public void Succeed ()
    {
      Type result = ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("arg", typeof (string), typeof (object));
      Assert.That (result, Is.SameAs (typeof (string)));
    }
	}
}
