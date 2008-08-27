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
using System.Collections.Generic;

namespace Remotion.Development.UnitTesting.ObjectMother
{
  /// <summary>
  /// Supplies factories to easily create <see cref="Dictionary{TKey,TValue}"/> instances initialized with up to 4 key-value-pairs.
  /// </summary>
  /// <example><code>
  /// <![CDATA[  
  /// var d = Dictionary.New("A",1, "B",2, "C",3); // d["A"]=1, d["B"]=2,...
  /// ]]>
  /// </code></example>
  public class Dictionary
  {
    public static System.Collections.Generic.Dictionary<TKey, TValue> New<TKey, TValue> ()
    {
      var container = new System.Collections.Generic.Dictionary<TKey, TValue> ();
      return container;
    }

    public static System.Collections.Generic.Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0)
    {
      var container = new System.Collections.Generic.Dictionary<TKey, TValue> (1);
      container[key0] = value0;
      return container;
    }

    public static System.Collections.Generic.Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1)
    {
      var container = new System.Collections.Generic.Dictionary<TKey, TValue> (2);
      container[key0] = value0;
      container[key1] = value1;
      return container;
    }

    public static System.Collections.Generic.Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2)
    {
      var container = new System.Collections.Generic.Dictionary<TKey, TValue> (3);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      return container;
    }

    public static System.Collections.Generic.Dictionary<TKey, TValue> New<TKey, TValue> (TKey key0, TValue value0, TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3)
    {
      var container = new System.Collections.Generic.Dictionary<TKey, TValue> (4);
      container[key0] = value0;
      container[key1] = value1;
      container[key2] = value2;
      container[key3] = value3;
      return container;
    }
  }
}