using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
  [Obsolete ("The tested methods are obsolete.")]
	public class CheckTypeObsolete
	{
    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
    {
      ArgumentUtility.CheckType ("arg", 13, typeof (string));
    }

    [Test]
		public void Succeed_ValueType ()
    {
      Assert.AreEqual (1, ArgumentUtility.CheckType ("arg", (object) 1, typeof (int)));
    }

    [Test]
    public void Succeed_NullableValueTypeNull ()
    {
      Assert.AreEqual (null, ArgumentUtility.CheckType ("arg", (object) null, typeof (int?)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_ValueTypeNull ()
    {
      ArgumentUtility.CheckType ("arg", (object) null, typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Fail_ValueType ()
    {
      ArgumentUtility.CheckType ("arg", (object) DateTime.MinValue, typeof (int));
    }

    [Test]
		public void Succeed_ReferenceTypeNull ()
    {
      Assert.AreEqual (null, ArgumentUtility.CheckType ("arg", (object) null, typeof (string)));
    }

    [Test]
		public void Succeed_NotNull ()
    {
      Assert.AreEqual ("test", ArgumentUtility.CheckType ("arg", "test", typeof (string)));
    }
	}
}
