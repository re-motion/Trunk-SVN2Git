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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ReadOnlyDictionaryTest
  {
    private Dictionary<string, string> _dictionary;
    private ReadOnlyDictionary<string, string> _readOnlyDictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new Dictionary<string, string> { { "a", "Alfa" }, { "b", "Bravo" }, { "c", "Charlie" } };
      _readOnlyDictionary = new ReadOnlyDictionary<string, string> (_dictionary);
    }

    [Test]
    public void GetEnumerator ()
    {
      Assert.That (_readOnlyDictionary.GetEnumerator (), Is.EqualTo (_dictionary.GetEnumerator ()));
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      var readOnlyDictionaryAsGenericIEnumerable = (IEnumerable<KeyValuePair<string, string>>) (_readOnlyDictionary);
      var dictionaryAsGenericIEnumerable = (IEnumerable<KeyValuePair<string, string>>) (_dictionary);
      Assert.That (readOnlyDictionaryAsGenericIEnumerable.GetEnumerator (), Is.EqualTo (dictionaryAsGenericIEnumerable.GetEnumerator ()));
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
    public void ContainsValue ()
    {
      Assert.That (_readOnlyDictionary.ContainsValue ("Alfa"), Is.True);
      Assert.That (_readOnlyDictionary.ContainsValue ("Bravo"), Is.True);
      Assert.That (_readOnlyDictionary.ContainsValue ("Charlie"), Is.True);

      Assert.That (_readOnlyDictionary.ContainsValue ("alfa"), Is.False);
      Assert.That (_readOnlyDictionary.ContainsValue (""), Is.False);
      Assert.That (_readOnlyDictionary.ContainsValue ("Charli"), Is.False);
    }


    [Test]
    public void TryGetValue ()
    {
      AssertTryGetValue("a","Alfa",true);
      AssertTryGetValue ("b", "Bravo", true);
      AssertTryGetValue ("c", "Charlie", true);

      AssertTryGetValue ("", null, false);
      AssertTryGetValue ("alfa", null, false);
      AssertTryGetValue ("Charli", null, false);
    }



    [Test]
    public void Comparer ()
    {
      Assert.That (_readOnlyDictionary.Comparer, Is.EqualTo (_dictionary.Comparer));
    }

    [Test]
    public void Count ()
    {
      Assert.That (_readOnlyDictionary.Count, Is.EqualTo (_dictionary.Count));
    }


    [Test]
    public void Indexer ()
    {
      AssertIndexer ("a", "Alfa");
      AssertIndexer ("b", "Bravo");
      AssertIndexer ("c", "Charlie");
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (System.Collections.Generic.KeyNotFoundException), ExpectedMessage = "The given key was not present in the dictionary.")]
    public void IndexerFailure ()
    {
      var dummy = _readOnlyDictionary["non existing key"];
    }

   
    void AssertTryGetValue (string key, string expectedValue, bool expectedHasValue)
    {
      string value;
      bool hasValue =_readOnlyDictionary.TryGetValue (key, out value);
      if (expectedHasValue)
      {
        Assert.That (hasValue, Is.True);
        Assert.That (value, Is.EqualTo (expectedValue));
      }
      else
      {
        Assert.That (hasValue, Is.False);
      }
    }

    void AssertIndexer (string key, string expectedValue)
    {
      Assert.That (_readOnlyDictionary[key], Is.EqualTo (expectedValue));
    }

  }


}