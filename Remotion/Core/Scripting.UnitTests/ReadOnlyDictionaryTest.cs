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
    private IDictionary<string, string> _readOnlyDictionaryAsIDictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new Dictionary<string, string> { { "a", "Alfa" }, { "b", "Bravo" }, { "c", "Charlie" } };
      _readOnlyDictionary = new ReadOnlyDictionary<string, string> (_dictionary);
      _readOnlyDictionaryAsIDictionary = _readOnlyDictionary;
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
    [ExpectedException (ExceptionType = typeof (System.Collections.Generic.KeyNotFoundException), ExpectedMessage = "The given key was not present in the dictionary.")]
    public void Indexer_Get_Failure ()
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


    [Test]
    [ExpectedException (ExceptionType = typeof (System.NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
    public void IDictionary_Add ()
    {
      _readOnlyDictionaryAsIDictionary.Add ("this", "fails");
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (System.NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
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
    [ExpectedException (ExceptionType = typeof (System.NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
    public void IDictionary_Indexer_Set_Fails ()
    {
      _readOnlyDictionaryAsIDictionary["this"] = "fails";
    }


    [Test]
    [ExpectedException (ExceptionType = typeof (System.NotSupportedException), ExpectedMessage = "Dictionary is read-only (IDictionary.Keys does not guarantee immutability).")]
    public void IDictionary_Keys ()
    {
      var dummy = _readOnlyDictionaryAsIDictionary.Keys;
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (System.NotSupportedException), ExpectedMessage = "Dictionary is read-only (IDictionary.Values does not guarantee immutability).")]
    public void IDictionary_Values ()
    {
      var dummy = _readOnlyDictionaryAsIDictionary.Values;
    }
  }


}