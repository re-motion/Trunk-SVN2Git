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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class ReadOnlyDictionarySpecificTest
  {
    private Dictionary<string, string> _dictionary;
    private ReadOnlyDictionarySpecific<string, string> _readOnlyDictionarySpecific;
    private IDictionary<string, string> _readOnlyDictionaryAsIDictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new Dictionary<string, string> { { "a", "Alfa" }, { "b", "Bravo" }, { "c", "Charlie" } };
      _readOnlyDictionarySpecific = new ReadOnlyDictionarySpecific<string, string> (_dictionary);
      _readOnlyDictionaryAsIDictionary = _readOnlyDictionarySpecific;
    }

    [Test]
    public void GetEnumerator ()
    {
      Assert.That (_readOnlyDictionarySpecific.GetEnumerator(), Is.EqualTo (_dictionary.GetEnumerator()));
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      var readOnlyDictionaryAsGenericIEnumerable = (IEnumerable<KeyValuePair<string, string>>) (_readOnlyDictionarySpecific);
      var dictionaryAsGenericIEnumerable = (IEnumerable<KeyValuePair<string, string>>) (_dictionary);
      Assert.That (readOnlyDictionaryAsGenericIEnumerable.GetEnumerator(), Is.EqualTo (dictionaryAsGenericIEnumerable.GetEnumerator()));
    }


    [Test]
    public void ContainsKey ()
    {
      Assert.That (_readOnlyDictionarySpecific.ContainsKey ("a"), Is.True);
      Assert.That (_readOnlyDictionarySpecific.ContainsKey ("b"), Is.True);
      Assert.That (_readOnlyDictionarySpecific.ContainsKey ("c"), Is.True);

      Assert.That (_readOnlyDictionarySpecific.ContainsKey ("c0"), Is.False);
      Assert.That (_readOnlyDictionarySpecific.ContainsKey (""), Is.False);
      Assert.That (_readOnlyDictionarySpecific.ContainsKey ("x"), Is.False);
    }

    [Test]
    public void ContainsValue ()
    {
      Assert.That (_readOnlyDictionarySpecific.ContainsValue ("Alfa"), Is.True);
      Assert.That (_readOnlyDictionarySpecific.ContainsValue ("Bravo"), Is.True);
      Assert.That (_readOnlyDictionarySpecific.ContainsValue ("Charlie"), Is.True);

      Assert.That (_readOnlyDictionarySpecific.ContainsValue ("alfa"), Is.False);
      Assert.That (_readOnlyDictionarySpecific.ContainsValue (""), Is.False);
      Assert.That (_readOnlyDictionarySpecific.ContainsValue ("Charli"), Is.False);
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
    public void Comparer ()
    {
      Assert.That (_readOnlyDictionarySpecific.Comparer, Is.EqualTo (_dictionary.Comparer));
    }

    [Test]
    public void Count ()
    {
      Assert.That (_readOnlyDictionarySpecific.Count, Is.EqualTo (_dictionary.Count));
    }


    [Test]
    public void Indexer_Get ()
    {
      AssertIndexer ("a", "Alfa");
      AssertIndexer ("b", "Bravo");
      AssertIndexer ("c", "Charlie");
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (KeyNotFoundException),
        ExpectedMessage = "The given key was not present in the dictionary.")]
    public void Indexer_Get_Failure ()
    {
      var dummy = _readOnlyDictionarySpecific["non existing key"];
    }


    private void AssertTryGetValue (string key, string expectedValue, bool expectedHasValue)
    {
      string value;
      bool hasValue = _readOnlyDictionarySpecific.TryGetValue (key, out value);
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
      Assert.That (_readOnlyDictionarySpecific[key], Is.EqualTo (expectedValue));
    }


    [Test]
    [ExpectedException (ExceptionType = typeof (NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
    public void IDictionary_Add ()
    {
      _readOnlyDictionaryAsIDictionary.Add ("this", "fails");
    }

    [Test]
    [ExpectedException (ExceptionType = typeof (NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
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
    [ExpectedException (ExceptionType = typeof (NotSupportedException), ExpectedMessage = "Dictionary is read-only.")]
    public void IDictionary_Indexer_Set_Fails ()
    {
      _readOnlyDictionaryAsIDictionary["this"] = "fails";
    }


    [Test]
    public void IDictionary_Keys ()
    {
      Assert.That (_readOnlyDictionaryAsIDictionary.Keys.ToArray(), Is.EqualTo (new[] { "a", "b", "c" }));
    }

    [Test]
    public void IDictionary_Values ()
    {
      Assert.That (_readOnlyDictionaryAsIDictionary.Values.ToArray(), Is.EqualTo (new[] { "Alfa", "Bravo", "Charlie" }));
    }
  }
}