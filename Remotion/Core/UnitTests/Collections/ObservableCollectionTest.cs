// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ObservableCollectionTest
  {
    private ObservableCollection<int> _collection;

    [SetUp]
    public void SetUp ()
    {
      _collection = new ObservableCollection<int>();
    }

    [Test]
    public void ItemsCleared ()
    {
      bool eventRaised = false;
      _collection.ItemsCleared += delegate { eventRaised = true; };
      
      _collection.Clear();
      
      Assert.That (eventRaised, Is.True);
    }

    [Test]
    public void ItemsCleared_TriggeredAfterOperation ()
    {
      _collection.Add (7);

      _collection.ItemsCleared += (sender, e) => Assert.That (_collection, Is.Empty);

      _collection.Clear();
    }

    [Test]
    public void Clear_WithoutSubscription ()
    {
      _collection.Clear ();
    }

    [Test]
    public void ItemInserted ()
    {
      _collection.Add (7);

      bool eventRaised = false;
      ObservableCollectionChangedEventArgs<int> eventArgs = null;
      _collection.ItemInserted += ((sender, e) => { eventRaised = true; eventArgs = e; });
      
      _collection.Add (8);

      Assert.That (eventRaised, Is.True);
      Assert.That (eventArgs.Index, Is.EqualTo (1));
      Assert.That (eventArgs.Item, Is.EqualTo (8));
    }

    [Test]
    public void ItemInserted_TriggeredAfterOperation ()
    {
      _collection.Add (7);

      _collection.ItemInserted += (sender, e) => Assert.That (_collection, List.Contains (8));

      _collection.Add (8);
    }

    [Test]
    public void ItemInserted_NotRaisedForReplace ()
    {
      _collection.Add (7);

      bool eventRaised = false;
      _collection.ItemInserted += delegate { eventRaised = true; };

      _collection[0] = 8;

      Assert.That (eventRaised, Is.False);
    }

    [Test]
    public void Insert_WithoutSubscription ()
    {
      _collection.Add (1);
    }

    [Test]
    public void ItemRemoved ()
    {
      _collection.Add (6);
      _collection.Add (7);

      bool eventRaised = false;
      ObservableCollectionChangedEventArgs<int> eventArgs = null;
      _collection.ItemRemoved += ((sender, e) => { eventRaised = true; eventArgs = e; });

      _collection.RemoveAt (1);

      Assert.That (eventRaised, Is.True);
      Assert.That (eventArgs.Index, Is.EqualTo (1));
      Assert.That (eventArgs.Item, Is.EqualTo (7));
    }

    [Test]
    public void ItemRemoved_TriggeredAfterOperation ()
    {
      _collection.Add (7);

      _collection.ItemRemoved += (sender, e) => Assert.That (_collection, List.Not.Contains (7));

      _collection.Remove (7);
    }

    [Test]
    public void ItemRemoved_NotRaisedForReplace ()
    {
      _collection.Add (7);

      bool eventRaised = false;
      _collection.ItemRemoved += delegate { eventRaised = true; };

      _collection[0] = 8;

      Assert.That (eventRaised, Is.False);
    }

    [Test]
    public void Remove_WithoutSubscription ()
    {
      _collection.Add (1);
      _collection.RemoveAt (0);
    }

    [Test]
    public void ItemSet ()
    {
      _collection.Add (6);
      _collection.Add (7);

      bool eventRaised = false;
      ObservableCollectionChangedEventArgs<int> eventArgs = null;
      _collection.ItemSet += ((sender, e) => { eventRaised = true; eventArgs = e; });

      _collection[1] = 8;

      Assert.That (eventRaised, Is.True);
      Assert.That (eventArgs.Index, Is.EqualTo (1));
      Assert.That (eventArgs.Item, Is.EqualTo (8));
    }

    [Test]
    public void ItemSet_TriggeredAfterOperation ()
    {
      _collection.Add (7);

      _collection.ItemSet += (sender, e) => Assert.That (_collection, List.Contains (8));

      _collection[0] = 8;
    }

    [Test]
    public void Set_WithoutSubscription ()
    {
      _collection.Add (1);
      _collection[0] = 8;
    }

    [Test]
    public void AsChangeResistantEnumerable ()
    {
      var enumerable = _collection.AsChangeResistantEnumerable ();
      using (var enumerator = enumerable.GetEnumerator ())
      {
        Assert.That (enumerator, Is.InstanceOfType (typeof (ChangeResistantObservableCollectionEnumerator<int>)));
      }
    }
  }
}