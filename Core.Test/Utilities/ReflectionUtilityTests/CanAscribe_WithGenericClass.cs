using System;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.ReflectionUtilityTests
{
  [TestFixture]
  public class CanAscribe_WithGenericClass
  {
    [Test]
    public void ClosedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType>), typeof (GenericType<object>)));
    }

    [Test]
    public void ClosedGenericType_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<,>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<ParameterType, int>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<ParameterType, int>), typeof (GenericType<object, int>)));
    }

    [Test]
    public void OpenGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void OpenGenericType_WithTwoTypeParameters ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericType<>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void OpenGenericType_WithOneOpenTypeParameter ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedOpenGenericType<>), typeof (GenericType<,>)));
    }

    [Test]
    public void ClosedDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<ParameterType>), typeof (GenericType<object>)));
    }

    [Test]
    public void OpenDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<>), typeof (GenericType<>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (DerivedGenericType<>), typeof (GenericType<ParameterType>)));
    }

    [Test]
    public void NonGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (DerivedGenericType), typeof (GenericType<object>)));
    }

    [Test]
    public void ClosedGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<int>), typeof (GenericType<object>)));
    }

    [Test]
    public void OpenGenericDerivedGenericType ()
    {
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<>)));
      Assert.IsTrue (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<ParameterType>)));
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (GenericDerivedGenericType<>), typeof (GenericType<object>)));
    }

    [Test]
    public void BaseType ()
    {
      Assert.IsFalse (ReflectionUtility.CanAscribe (typeof (BaseType), typeof (GenericType<>)));
    }
  }
}