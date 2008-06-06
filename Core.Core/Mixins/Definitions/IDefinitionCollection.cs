/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;

namespace Remotion.Mixins.Definitions
{
  public interface IDefinitionCollection<TKey, TValue> : IEnumerable<TValue>
  {
    TValue[] ToArray ();
    int Count { get; }
    bool ContainsKey (TKey key);
    TValue this[int index] { get; }
    TValue this [TKey key] { get; }
  }
}
