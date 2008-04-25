using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
	[TestFixture]
	public class CheckType
	{
		[Test]
		[ExpectedException (typeof (ArgumentTypeException))]
		public void Fail_Type ()
		{
			ArgumentUtility.CheckType<string> ("arg", 13);
		}

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_ValueType ()
    {
      ArgumentUtility.CheckType<int> ("arg", (object) null);
    }
    
    [Test]
		public void Succeed_Null ()
		{
			string result = ArgumentUtility.CheckType<string> ("arg", null);
			Assert.AreEqual (null, result);
		}

		[Test]
		public void Succeed_String ()
		{
			string result = ArgumentUtility.CheckType<string> ("arg", "test");
			Assert.AreEqual ("test", result);
		}

    [Test]
    public void Succeed_BaseType ()
    {
      string result = (string) ArgumentUtility.CheckType<object> ("arg", "test");
      Assert.AreEqual ("test", result);
    }
  }
}
