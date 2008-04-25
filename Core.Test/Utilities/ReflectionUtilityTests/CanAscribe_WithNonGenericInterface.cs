using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class CanAscribe_WithNonGenericInterface
  {
    [Test]
    public void DerivedType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (IDerivedInterface), typeof (IDerivedInterface)));
    }

    [Test]
    public void BaseType ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (IBaseInterface), typeof (IDerivedInterface)));
    }
  }
}