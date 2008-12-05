// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using NUnit.Framework;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class GenericTypeInstantiatorTest
  {
    [Test]
    public void NonGenericTypeIsntTouched()
    {
      Assert.AreSame (typeof (GenericTypeInstantiatorTest), GenericTypeInstantiator.EnsureClosedType (typeof (GenericTypeInstantiatorTest)));
    }

    class NoConstraints<T> { }

    [Test]
    public void GenericArgsWithoutConstraints ()
    {
      Assert.AreEqual (typeof (NoConstraints<object>), GenericTypeInstantiator.EnsureClosedType (typeof(NoConstraints<>)));
    }

    class Outer<T>
    {
      public class Inner : Outer<T> { }
    }

    [Test]
    public void NestedWorks ()
    {
      Assert.AreEqual (typeof (Outer<object>.Inner), GenericTypeInstantiator.EnsureClosedType (typeof (Outer<>.Inner)));
    }

    class ClassContraint<T> where T : class {}

    [Test]
    public void ClassConstraint()
    {
      Assert.AreEqual (typeof (ClassContraint<object>), GenericTypeInstantiator.EnsureClosedType (typeof (ClassContraint<>)));
    }

    class ValueTypeContraint<T> where T : struct { }

    [Test]
    public void ValueTypeConstraint ()
    {
      Assert.AreEqual (typeof (ValueTypeContraint<int>), GenericTypeInstantiator.EnsureClosedType (typeof (ValueTypeContraint<>)));
    }

    class InterfaceConstraint<T> where T : IServiceProvider { }

    [Test]
    public void SingleInterfaceConstraint ()
    {
      Assert.AreEqual (typeof (InterfaceConstraint<IServiceProvider>), GenericTypeInstantiator.EnsureClosedType (typeof (InterfaceConstraint<>)));
      
      Type parameter = typeof (InterfaceConstraint<>).GetGenericArguments()[0];
      Assert.AreEqual (typeof (IServiceProvider), GenericTypeInstantiator.GetGenericParameterInstantiation(parameter, null));
    }

    class SPImpl : IServiceProvider
    {
      public object GetService (Type serviceType)
      {
        throw new Exception ("The method or operation is not implemented.");
      }
    }

    [Test]
    public void SingleConstraintWithHint ()
    {
      Type parameter = typeof (InterfaceConstraint<>).GetGenericArguments()[0];
      Assert.AreEqual (typeof (SPImpl), GenericTypeInstantiator.GetGenericParameterInstantiation (parameter, typeof (SPImpl)));
    }

    [Test]
    public void SingleConstraintWithUnfittingHint ()
    {
      Type parameter = typeof (InterfaceConstraint<>).GetGenericArguments()[0];
      Assert.AreEqual (typeof (IServiceProvider),
        GenericTypeInstantiator.GetGenericParameterInstantiation (parameter, typeof (GenericTypeInstantiatorTest)));
    }

    class BaseClassConstraint<T> where T : GenericTypeInstantiatorTest { }

    [Test]
    public void SingleBaseClassConstraint ()
    {
      Assert.AreEqual (typeof (BaseClassConstraint<GenericTypeInstantiatorTest>),
          GenericTypeInstantiator.EnsureClosedType (typeof (BaseClassConstraint<>)));
    }

    class CompatibleConstraints1<T> where T : BaseType3, IBaseType31, IBaseType32 { }

    [Test]
    public void MultipleCompatibleConstraints1 ()
    {
      Assert.AreEqual (typeof (CompatibleConstraints1<BaseType3>),
          GenericTypeInstantiator.EnsureClosedType (typeof (CompatibleConstraints1<>)));
    }

    class CompatibleConstraints2<T> where T : IBaseType33, IBaseType34 { }

    [Test]
    public void MultipleCompatibleConstraints2 ()
    {
      Assert.AreEqual (typeof (CompatibleConstraints2<IBaseType34>),
          GenericTypeInstantiator.EnsureClosedType (typeof (CompatibleConstraints2<>)));
    }

    public class IncompatibleConstraints1<T> where T : BaseType3, IBaseType31, IBaseType2 { }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter T has incompatible constraints",
        MatchType = MessageMatch.Contains)]
    public void MultipleIncompatibleConstraintsThrows1 ()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (IncompatibleConstraints1<>));
    }

    class IncompatibleConstraints2<T> where T : struct, IBaseType2 { }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter T has incompatible constraints",
        MatchType = MessageMatch.Contains)]
    public void MultipleIncompatibleConstraintsThrows2 ()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (IncompatibleConstraints2<>));
    }

    class IncompatibleConstraints3<T> where T : IBaseType31, IBaseType32 { }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The generic type parameter T has incompatible constraints",
        MatchType = MessageMatch.Contains)]
    public void MultipleIncompatibleConstraintsThrows3 ()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (IncompatibleConstraints3<>));
    }

    [Test]
    public void MultipleIncompatibleConstraintsWithHint ()
    {
      Type parameter = typeof (IncompatibleConstraints3<>).GetGenericArguments()[0];
      Assert.AreEqual (typeof (BaseType3), GenericTypeInstantiator.GetGenericParameterInstantiation (parameter, typeof (BaseType3)));
    }

    class Uninstantiable<T>
        where T : Uninstantiable<T>.IT
    {
      public interface IT {}
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "The generic type parameter T has a constraint IT which itself contains generic parameters",
        MatchType = MessageMatch.Contains)]
    public void UninstantiableClass()
    {
      GenericTypeInstantiator.EnsureClosedType (typeof (Uninstantiable<>));
    }
  }
}
