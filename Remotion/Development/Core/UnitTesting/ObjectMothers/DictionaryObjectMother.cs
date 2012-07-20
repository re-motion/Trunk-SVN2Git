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
using System.Collections.Generic;

namespace Remotion.Development.UnitTesting.ObjectMothers
{
  /// <summary>
  /// Supplies factories to easily create <see cref="Dictionary{TKey,TValue}"/> instances initialized with up to 4 key-value-pairs.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var d = DictionaryObjectMother.New("A",1, "B",2, "C",3); // d["A"]=1, d["B"]=2,...
  /// ]]>
  /// </code></example>
  public class DictionaryObjectMother
  {
    public static Dictionary<TKey, TValue> New<TKey, TValue> ()
    {
      var container = new Dictionary<TKey, TValue> ();
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0)
    {
      var container = new Dictionary<TKey, TValue> (1);
      container[key0] = value0;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1)
    {
      var container = new Dictionary<TKey, TValue> (2);
      container[key0] = value0;
      container[key1] = value1;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2)
    {
      var container = new Dictionary<TKey, TValue> (3);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      return container;
    }

    public static Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3)
    {
      var container = new Dictionary<TKey, TValue> (4);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      container[key3] = value3;
      return container;
    }
  }
}
