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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ReadOnlyDictionaryTest
  {
    private Dictionary<string, string> _dictionary;
    private ReadOnlyDictionary<string, string> _readOnlyDictionary;
    private IDictionary<string, string> _readOnlyDictionaryAsIDictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new Dictionary<string, string> { { "a", "Alfa" }, { "b", "Bravo" }, { "c", "Charlie" } };
      _readOnlyDictionary = new ReadOnlyDictionary<string, string> (_dictionary);
      _readOnlyDictionaryAsIDictionary = _readOnlyDictionary;
    }

    [Test]
    public void GetEnumerator_NonGeneric ()
    {
      var readOnlyDictionaryAsNonGenericEnumerable = _readOnlyDictionary;
      IEnumerator enumerator = readOnlyDictionaryAsNonGenericEnumerable.GetEnumerator();
      Assert.That (enumerator, Is.EqualTo (_dictionary.GetEnumerator()));
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      var readOnlyDictionaryAsGenericIEnumerable = _readOnlyDictionary;
      IEnumerator<KeyValuePair<string, string>> enumerator = readOnlyDictionaryAsGenericIEnumerable.GetEnumerator();
      Assert.That (enumerator, Is.EqualTo (_dictionary.GetEnumerator()));
    }


    [Test]
    public void ContainsKey ()
    {
      Assert.That (_readOnlyDictionary.ContainsKey ("a"), Is.True);
      Assert.That (_readOnlyDictionary.ContainsKey ("b"), Is.True);
      Assert.That (_readOnlyDictionary.ContainsKey ("c"), Is.True);

      Assert.That (_readOnlyDictionary.ContainsKey ("c0"), Is.False);
      Assert.That (_readOnlyDictionary.ContainsKey (""), Is.False);
      Assert.That (_readOnlyDictionary.ContainsKey ("x"), Is.False);
    }


    [Test]
    public void TryGetValue ()
    {
      AssertTryGetValue ("a", "Alfa", true);
      AssertTryGetValue ("b", "Bravo", true);
      AssertTryGetValue ("c", "Charlie", true);

      AssertTryGetValue ("", null, false);
      AssertTryGetValue ("alfa", null, false);
      AssertTryGetValue ("Charli", null, false);
    }


    [Test]
    public void Count ()
    {
      Assert.That (_readOnlyDictionary.Count, Is.EqualTo (_dictionary.Count));
    }


    [Test]
    public void Indexer_Get ()
    {
      AssertIndexer ("a", "Alfa");
      AssertIndexer ("b", "Bravo");
      AssertIndexer ("c", "Charlie");
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = "The given key was not present in the dictionary.")]
    public void Indexer_Get_Failure ()
    {
      Dev.Null = _readOnlyDictionary["non existing key"];
    }
    
    private void AssertTryGetValue (string key, string expectedValue, bool expectedHasValue)
    {
      string value;
      bool hasValue = _readOnlyDictionary.TryGetValue (key, out value);
      if (expectedHasValue)
      {
        Assert.That (hasValue, Is.True);
        Assert.That (value, Is.EqualTo (expectedValue));
      }
      else
        Assert.That (hasValue, Is.False);
    }

    private void AssertIndexer (string key, string expectedValue)
    {
      Assert.That (_readOnlyDictionary[key], Is.EqualTo (expectedValue));
    }


    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
    public void IDictionary_Add ()
    {
      _readOnlyDictionaryAsIDictionary.Add ("this", "fails");
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
    public void IDictionary_Remove ()
    {
      _readOnlyDictionaryAsIDictionary.Remove ("this_fails");
    }


    [Test]
    public void IDictionary_Indexer_Get ()
    {
      Assert.That (_readOnlyDictionaryAsIDictionary["c"], Is.EqualTo ("Charlie"));
    }


    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
    public void IDictionary_Indexer_Set_Fails ()
    {
      _readOnlyDictionaryAsIDictionary["this"] = "fails";
    }


    [Test]
    public void IDictionary_Keys ()
    {
      var keys = _readOnlyDictionaryAsIDictionary.Keys;

      Assert.That (keys, Is.TypeOf (typeof (ReadOnlyCollectionDecorator<string>)));
      Assert.That (keys, Is.EqualTo (new[] { "a", "b", "c" }));
    }

    [Test]
    public void IDictionary_Values ()
    {
      var values = _readOnlyDictionaryAsIDictionary.Values;

      Assert.That (values, Is.TypeOf (typeof (ReadOnlyCollectionDecorator<string>)));
      Assert.That (values, Is.EqualTo (new[] { "Alfa", "Bravo", "Charlie" }));
    }
  }
}
