using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class AttributeUsageTest
  {
    [Test]
    public void GetAttributeUsage ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (MultipleAttribute));
      Assert.AreEqual (typeof (MultipleAttribute).GetCustomAttributes (typeof (AttributeUsageAttribute), true)[0], attribute);
    }

    [Test]
    public void GetAttributeUsageNullIfNotDefined ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (ImplicitUsageAttribute));
      Assert.IsNotNull (attribute);
      Assert.AreEqual (new AttributeUsageAttribute(AttributeTargets.All), attribute);
    }

    [Test]
    public void GetAttributeUsageWithNoAttribute ()
    {
      AttributeUsageAttribute attribute = AttributeUtility.GetAttributeUsage (typeof (object));
      Assert.IsNull (attribute);
    }

    [Test]
    public void AllowMultipleTrue ()
    {
      Assert.IsTrue (AttributeUtility.IsAttributeAllowMultiple (typeof (MultipleAttribute)));
    }

    [Test]
    public void AllowMultipleFalse ()
    {
      Assert.IsFalse (AttributeUtility.IsAttributeAllowMultiple (typeof (NotInheritedNotMultipleAttribute)));
    }

    [Test]
    public void DefaultAllowMultiple ()
    {
      Assert.IsFalse (AttributeUtility.IsAttributeAllowMultiple (typeof (ImplicitUsageAttribute)));
    }

    [Test]
    public void InheritedTrue ()
    {
      Assert.IsTrue (AttributeUtility.IsAttributeInherited(typeof (InheritedAttribute)));
    }

    [Test]
    public void InheritedFalse ()
    {
      Assert.IsFalse (AttributeUtility.IsAttributeInherited (typeof (NotInheritedNotMultipleAttribute)));
    }

    [Test]
    public void DefaultInherited ()
    {
      Assert.IsTrue (AttributeUtility.IsAttributeInherited (typeof (ImplicitUsageAttribute)));
    }
  }
}