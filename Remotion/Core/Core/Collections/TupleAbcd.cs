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
#if NET_3_5
using System;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Represents a 4-tuple, or quadruple.
  /// </summary>
  /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
  /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
  /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
  /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
  [Serializable]
  public class Tuple<T1, T2, T3, T4> : IEquatable<Tuple<T1, T2, T3, T4>>
  {
    private readonly T1 _item1;
    private readonly T2 _item2;
    private readonly T3 _item3;
    private readonly T4 _item4;

    public Tuple (T1 item1, T2 item2, T3 item3, T4 item4)
    {
      _item1 = item1;
      _item2 = item2;
      _item3 = item3;
      _item4 = item4;
    }

    public T1 Item1
    {
      get { return _item1; }
    }

    public T2 Item2
    {
      get { return _item2; }
    }

    public T3 Item3
    {
      get { return _item3; }
    }

    public T4 Item4
    {
      get { return _item4; }
    }

    public bool Equals (Tuple<T1, T2, T3, T4> other)
    {
      if (other == null)
        return false;

      return EqualityUtility.Equals (_item1, other._item1)
             && EqualityUtility.Equals (_item2, other._item2)
             && EqualityUtility.Equals (_item3, other._item3)
             && EqualityUtility.Equals (_item4, other._item4);
    }

    public override bool Equals (object obj)
    {
      Tuple<T1, T2, T3, T4> other = obj as Tuple<T1, T2, T3, T4>;
      if (other == null)
        return false;
      return Equals (other);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_item1, _item2, _item3, _item4);
    }

    public override string ToString ()
    {
      return string.Format ("<{0}, {1}, {2}, {3}>", _item1, _item2, _item3, _item4);
    }
  }
}
#endif
