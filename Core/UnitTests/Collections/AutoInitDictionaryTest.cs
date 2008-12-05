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
using Remotion.Collections;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class AutoInitDictionaryTest
  {
    private MultiDictionary<string, string> _dictionary;

    [SetUp]
    public void SetUp ()
    {
      _dictionary = new MultiDictionary<string, string>();
    }

    [Test]
    public void Add ()
    {
      Assert.AreEqual (0, _dictionary["key"].Count);
      _dictionary.Add ("key", "value1");
      _dictionary.Add ("key", "value2");
      Assert.AreEqual (2, _dictionary["key"].Count);
      Assert.AreEqual ("value1", _dictionary["key"][0]);
      Assert.AreEqual ("value2", _dictionary["key"][1]);
    }

    [Test]
    public void Count()
    {
      object o = _dictionary["a"];
      o = _dictionary["b"];
      Assert.AreEqual (2, _dictionary.Count);
    }

    [Test]
    public void CountWithSameValues ()
    {
      _dictionary.Add ("key", "value1");
      _dictionary.Add ("key", "value2");
      _dictionary.Add ("key2", "value3");
      Assert.AreEqual (2, _dictionary.Count);
      Assert.AreEqual (2, _dictionary.KeyCount);
      Assert.AreEqual (3, _dictionary.CountValues());

    }
  }
}
