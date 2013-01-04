// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
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

      public TargetTypeMock (Exception e)
      {
        throw e;
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

      public ConcreteTypeMock (bool dummy)
        : this ()
      {
      }

      public ConcreteTypeMock (Exception e)
        : this ()
      {
        throw e;
      }

      public ConcreteTypeMock (int i) : this()
      {
        CtorArg = i;
      }
    }

    [Test]
    public void Initialization ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false);
      Assert.That (info.BindingFlags, Is.EqualTo (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
      Assert.That (info.Binder, Is.Null);
      Assert.That (info.CallingConvention, Is.EqualTo (CallingConventions.Any));
      Assert.That (info.MemberName, Is.EqualTo (".ctor"));
      Assert.That (info.ParameterModifiers, Is.Null);
      Assert.That (info.TargetType, Is.SameAs (typeof (TargetTypeMock)));
      Assert.That (info.DefiningType, Is.SameAs (typeof (ConcreteTypeMock)));
      Assert.That (info.AllowNonPublic, Is.False);
    }

    [Test]
    public void CreateDelegate ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Func<ConcreteTypeMock> d = (Func<ConcreteTypeMock>) info.GetDelegate (typeof (Func<ConcreteTypeMock>));
      ConcreteTypeMock instance = d();
      Assert.That (instance, Is.Not.Null);
      Assert.That (instance, Is.InstanceOf (typeof (ConcreteTypeMock)));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.Mixins.UnitTests.Core.Utilities.MixedTypeConstructorLookupInfoTest"
       + "+TargetTypeMock does not contain a constructor with the following signature: (System.Boolean).")]
    public void CreateDelegate_NoCtorOnTargetType ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false);
      info.GetDelegate (typeof (Func<bool, ConcreteTypeMock>));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.Mixins.UnitTests.Core.Utilities.MixedTypeConstructorLookupInfoTest"
        + "+TargetTypeMock contains a constructor with the required signature, but it is not public (and the allowNonPublic flag is not set).")]
    public void CreateDelegate_ProtectedCtorOnTargetType_NoAllowNonPublic ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false);
      info.GetDelegate (typeof (Func<int, ConcreteTypeMock>));
    }

    [Test]
    public void GetDelegate_CachesDelegates ()
    {
      // insert value into cache
      MixedTypeConstructorLookupInfo info1 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d1 = info1.GetDelegate (typeof (Func<int, ConcreteTypeMock>));

      MixedTypeConstructorLookupInfo info2 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d2 = info2.GetDelegate (typeof (Func<int, ConcreteTypeMock>));
      Delegate d3 = info2.GetDelegate (typeof (Func<int, ConcreteTypeMock>));
      Assert.That (d2, Is.SameAs (d1));
      Assert.That (d3, Is.SameAs (d1));
    }

    [Test]
    public void GetDelegate_CachingDoesNotLeadToIgnoredAllowNonPublic ()
    {
      // insert value into cache
      MixedTypeConstructorLookupInfo info1 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d1 = info1.GetDelegate (typeof (Func<int, ConcreteTypeMock>));

      try
      {
        new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false)
            .GetDelegate (typeof (Func<int, ConcreteTypeMock>));
        Assert.Fail ("Expected exception");
      }
      catch (MissingMethodException)
      {
        // ok
      }

      MixedTypeConstructorLookupInfo info2 = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Delegate d2 = info2.GetDelegate (typeof (Func<int, ConcreteTypeMock>));
      Assert.That (d2, Is.SameAs (d1));
    }

    [Test]
    public void CreateDelegate_ProtectedCtorOnTargetType_AllowNonPublic ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Func<int, ConcreteTypeMock> d = (Func<int, ConcreteTypeMock>) info.GetDelegate (typeof (Func<int, ConcreteTypeMock>));
      ConcreteTypeMock instance = d (12);
      Assert.That (instance.CtorArg, Is.EqualTo (12));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = 
        "Type 'Remotion.Mixins.UnitTests.Core.Utilities.MixedTypeConstructorLookupInfoTest+ConcreteTypeMock' does not contain a constructor with the "
        + "following arguments types: System.String.")]
    public void CreateDelegate_CtorOnTargetType_ButNotOnConcrete ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      info.GetDelegate (typeof (Func<string, ConcreteTypeMock>));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This exception is thrown on purpose.")]
    public void CreateDelegate_DelegatePropagatesException ()
    {
      var exception = new InvalidOperationException ("This exception is thrown on purpose.");
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false);
      Func<Exception, ConcreteTypeMock> d = (Func<Exception, ConcreteTypeMock>) info.GetDelegate (typeof (Func<Exception, ConcreteTypeMock>));
      d (exception);
    }

    [Test]
    public void CreateDelegate_DelegateUsesScope ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), false);
      Func<ConcreteTypeMock> d = (Func<ConcreteTypeMock>) info.GetDelegate (typeof (Func<ConcreteTypeMock>));
      object[] mixinInstances = new object[] {1, 2, "3"};
      ConcreteTypeMock instance;
      using (new MixedObjectInstantiationScope (mixinInstances))
      {
        instance = d();
      }
      Assert.That (instance.Scope.SuppliedMixinInstances, Is.SameAs (mixinInstances));
    }

    [Test]
    public void CreateDelegate_DelegatePassesParameters ()
    {
      MixedTypeConstructorLookupInfo info = new MixedTypeConstructorLookupInfo (typeof (ConcreteTypeMock), typeof (TargetTypeMock), true);
      Func<int, ConcreteTypeMock> d = (Func<int, ConcreteTypeMock>) info.GetDelegate (typeof (Func<int, ConcreteTypeMock>));
      ConcreteTypeMock instance = d (43);
      Assert.That (instance.CtorArg, Is.EqualTo (43));
    }
  }
}
