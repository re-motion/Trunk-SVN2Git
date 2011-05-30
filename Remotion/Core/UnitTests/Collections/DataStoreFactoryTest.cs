// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class DataStoreFactoryTest
  {
    [Test]
    public void Create ()
    {
      var result = DataStoreFactory.Create<string, int>();

      Assert.That (result, Is.TypeOf (typeof (SimpleDataStore<string, int>)));
    }

    [Test]
    public void Create_IEqualityComparerOverload ()
    {
      var result = DataStoreFactory.Create<string, int> (StringComparer.InvariantCultureIgnoreCase);

      Assert.That (result, Is.TypeOf (typeof (SimpleDataStore<string, int>)));
      Assert.That (
          ((Dictionary<string, int>) PrivateInvoke.GetNonPublicField (result, "_innerDictionary")).Comparer,
          Is.SameAs (StringComparer.InvariantCultureIgnoreCase));
    }

    [Test]
    public void CreateWithLocking ()
    {
      var result = DataStoreFactory.CreateWithLocking<string, int>();

      Assert.That (result, Is.TypeOf (typeof (LockingDataStoreDecorator<string, int>)));
      var innerStore = PrivateInvoke.GetNonPublicField (result, "_innerStore");
      Assert.That (innerStore, Is.TypeOf (typeof (SimpleDataStore<string, int>)));
    }

    [Test]
    public void CreateWithLocking_IEqualityComparerOverload ()
    {
      var result = DataStoreFactory.CreateWithLocking<string, int> (StringComparer.InvariantCultureIgnoreCase);

      Assert.That (result, Is.TypeOf (typeof (LockingDataStoreDecorator<string, int>)));
      var innerStore = PrivateInvoke.GetNonPublicField (result, "_innerStore");
      Assert.That (innerStore, Is.TypeOf (typeof (SimpleDataStore<string, int>)));
      Assert.That (
          ((Dictionary<string, int>) PrivateInvoke.GetNonPublicField (innerStore, "_innerDictionary")).Comparer,
          Is.SameAs (StringComparer.InvariantCultureIgnoreCase));
    }

    [Test]
    public void CreateWithLazyLocking ()
    {
      var result = DataStoreFactory.CreateWithLazyLocking<string, object>();

      Assert.That (result, Is.TypeOf (typeof (LazyLockingDataStoreAdapter<string, object>)));
      var innerStore = PrivateInvoke.GetNonPublicField (result, "_innerDataStore");
      Assert.That (innerStore, Is.TypeOf (typeof (LockingDataStoreDecorator<string, DoubleCheckedLockingContainer<object>>)));
      var innerDecoratorStore = PrivateInvoke.GetNonPublicField (innerStore, "_innerStore");
      Assert.That (innerDecoratorStore, Is.TypeOf (typeof (SimpleDataStore<string, DoubleCheckedLockingContainer<object>>)));
    }

    [Test]
    public void CreateWithLazyLocking_IEqualityComparerOverload ()
    {
      var result = DataStoreFactory.CreateWithLazyLocking<string, object> (StringComparer.InvariantCultureIgnoreCase);

      Assert.That (result, Is.TypeOf (typeof (LazyLockingDataStoreAdapter<string, object>)));
      var innerStore = PrivateInvoke.GetNonPublicField (result, "_innerDataStore");
      Assert.That (innerStore, Is.TypeOf (typeof (LockingDataStoreDecorator<string, DoubleCheckedLockingContainer<object>>)));
      var innerDecoratorStore = PrivateInvoke.GetNonPublicField (innerStore, "_innerStore");
      Assert.That (innerDecoratorStore, Is.TypeOf (typeof (SimpleDataStore<string, DoubleCheckedLockingContainer<object>>)));
      Assert.That (
          ((Dictionary<string, DoubleCheckedLockingContainer<object>>) PrivateInvoke.GetNonPublicField (innerDecoratorStore, "_innerDictionary")).Comparer,
          Is.SameAs (StringComparer.InvariantCultureIgnoreCase));
    }
  }
}