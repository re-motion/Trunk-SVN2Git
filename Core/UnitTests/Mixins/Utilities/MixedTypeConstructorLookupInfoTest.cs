/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Utilities;

namespace Remotion.UnitTests.Mixins.Utilities
{
  [TestFixture]
  public class MixedTypeConstructorLookupInfoTest
  {
    public class TargetTypeMock
    {
      public TargetTypeMock ()
      {
      }

      protected TargetTypeMock (int i)
      {
        Dev.Null = i;
      }

      public TargetTypeMock (string s)
      {
        Dev.Null = s;
      }
    }

    public class ConcreteTypeMock
    {
      public int? CtorArg = null;
      public MixedObjectInstantiationScope Scope;

      public ConcreteTypeMock ()
      {
        Scope = MixedObjectInstantiationScope.Current;
        Assert.That (Scope.IsDisposed, Is.False);
      }

      public ConcreteTypeMock (bool throwException)
        : this ()
      {
        if (throwException)
          throw new InvalidOperationException ("This exception is thrown on purpose.");
      }

      public ConcreteTypeMock (int i) : this()
      {
        CtorArg = i;
      }
    }

    [Test]
    public void Initialization ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      Assert.That (info.BindingFlags, Is.EqualTo (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
      Assert.That (info.Binder, Is.Null);
      Assert.That (info.CallingConvention, Is.EqualTo (CallingConventions.Any));
      Assert.That (info.MemberName, Is.EqualTo (".ctor"));
      Assert.That (info.ParameterModifiers, Is.Null);
    }

    [Test]
    public void CreateDelegate ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      Func<object[], ConcreteTypeMock> d = (Func<object[], ConcreteTypeMock>) info.GetDelegate (typeof (Func<object[], ConcreteTypeMock>));
      ConcreteTypeMock instance = d(new object[0]);
      Assert.That (instance, Is.Not.Null);
      Assert.That (instance, Is.InstanceOfType (typeof (ConcreteTypeMock)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The delegate type must have at least one argument, which must be of type "
        + "object[]. This argument will be used to pass pre-instantiated mixins to the instance creator.\r\nParameter name: delegateType")]
    public void CreateDelegate_NoArguments ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      info.GetDelegate (typeof (Func<ConcreteTypeMock>));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The delegate type must have at least one argument, which must be of type "
        + "object[]. This argument will be used to pass pre-instantiated mixins to the instance creator.\r\nParameter name: delegateType")]
    public void CreateDelegate_InvalidFirstArgument ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      info.GetDelegate (typeof (Func<int, ConcreteTypeMock>));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Utilities.MixedTypeConstructorLookupInfoTest"
       + "+TargetTypeMock does not contain a constructor with signature (System.Boolean) (allowNonPublic: False).")]
    public void CreateDelegate_NoCtorOnTargetType ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false);
      info.GetDelegate (typeof (Func<object[], bool, ConcreteTypeMock>));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Utilities.MixedTypeConstructorLookupInfoTest"
        + "+TargetTypeMock does not contain a constructor with signature (System.Int32) (allowNonPublic: False).")]
    public void CreateDelegate_ProtectedCtorOnTargetType_NoAllowNonPublic ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false);
      info.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));
    }

    [Test]
    public void GetDelegate_CachesDelegates ()
    {
      // insert value into cache
      MixedTypeConstructorLookupInfo info1 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d1 = info1.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));

      MixedTypeConstructorLookupInfo info2 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d2 = info2.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));
      Delegate d3 = info2.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));
      Assert.AreSame (d1, d2);
      Assert.AreSame (d1, d3);
    }

    [Test]
    public void GetDelegate_CachingDoesNotLeadToIgnoredAllowNonPublic ()
    {
      // insert value into cache
      MixedTypeConstructorLookupInfo info1 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d1 = info1.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));

      try
      {
        new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false)
            .GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));
        Assert.Fail ("Expected exception");
      }
      catch (MissingMethodException)
      {
        // ok
      }

      MixedTypeConstructorLookupInfo info2 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d2 = info2.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));
      Assert.AreSame (d1, d2);
    }

    [Test]
    public void CreateDelegate_ProtectedCtorOnTargetType_AllowNonPublic ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Func<object[], int, ConcreteTypeMock> d = (Func<object[], int, ConcreteTypeMock>) info.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));
      ConcreteTypeMock instance = d (new object[0], 12);
      Assert.AreEqual (12, instance.CtorArg);
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Concrete type Remotion.UnitTests.Mixins.Utilities."
        + "MixedTypeConstructorLookupInfoTest+TargetTypeMock does not contain a constructor with signature (System.String) (although target " 
        + "type 'TargetTypeMock' does).")]
    public void CreateDelegate_CtorOnTargetType_ButNotOnConcrete ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      info.GetDelegate (typeof (Func<object[], string, ConcreteTypeMock>));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This exception is thrown on purpose.")]
    public void CreateDelegate_DelegatePropagatesException ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      Func<object[], bool, ConcreteTypeMock> d = (Func<object[], bool, ConcreteTypeMock>) info.GetDelegate (typeof (Func<object[], bool, ConcreteTypeMock>));
      d (new object[0], true);
    }

    [Test]
    public void CreateDelegate_DelegateSetsScope ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      Func<object[], ConcreteTypeMock> d = (Func<object[], ConcreteTypeMock>) info.GetDelegate (typeof (Func<object[], ConcreteTypeMock>));
      object[] mixinInstances = new object[] {1, 2, "3"};
      ConcreteTypeMock instance = d (mixinInstances);
      Assert.That (instance.Scope.SuppliedMixinInstances, Is.SameAs (mixinInstances));
    }

    [Test]
    public void CreateDelegate_DelegateDisposesScope ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      Func<object[], ConcreteTypeMock> d = (Func<object[], ConcreteTypeMock>) info.GetDelegate (typeof (Func<object[], ConcreteTypeMock>));
      ConcreteTypeMock instance = d (new object[0]);
      Assert.That (instance.Scope.IsDisposed, Is.True);
    }

    [Test]
    public void CreateDelegate_DelegateDisposesScope_WithException ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      Func<object[], bool, ConcreteTypeMock> d = (Func<object[], bool, ConcreteTypeMock>) info.GetDelegate (typeof (Func<object[], bool, ConcreteTypeMock>));
      try
      {
        d (new object[0], true);
      }
      catch (InvalidOperationException)
      {
      }
      Assert.That (MixedObjectInstantiationScope.HasCurrent, Is.False);
    }

    [Test]
    public void CreateDelegate_DelegatePassesParameters ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (ConcreteTypeMock), false);
      Func<object[], int, ConcreteTypeMock> d = (Func<object[], int, ConcreteTypeMock>) info.GetDelegate (typeof (Func<object[], int, ConcreteTypeMock>));
      ConcreteTypeMock instance = d (new object[0], 43);
      Assert.That (instance.CtorArg, Is.EqualTo (43));
    }
  }
}
