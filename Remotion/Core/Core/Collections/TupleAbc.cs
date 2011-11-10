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
#if NET_3_5
using System;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Represents a 3-tuple, or triple.
  /// </summary>
  /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
  /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
  /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
  [Serializable]
  public class Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
  {
    #region Obsolete

    [Obsolete ("Use Item1 instead. (Version 1.13.47)")]
    public T1 A
    {
      get { return Item1; }
    }

    [Obsolete ("Use Item2 instead. (Version 1.13.47)")]
    public T2 B
    {
      get { return Item2; }
    }

    [Obsolete ("Use Item3 instead. (Version 1.13.47)")]
    public T3 C
    {
      get { return Item3; }
    }

    #endregion

    private readonly T1 _item1;
    private readonly T2 _item2;
    private readonly T3 _item3;

    public Tuple (T1 item1, T2 item2, T3 item3)
    {
      _item1 = item1;
      _item2 = item2;
      _item3 = item3;
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

    public bool Equals (Tuple<T1, T2, T3> other)
    {
      return EqualityUtility.NotNullAndSameType (this, other)
             && EqualityUtility.Equals (_item1, other._item1)
             && EqualityUtility.Equals (_item2, other._item2)
             && EqualityUtility.Equals (_item3, other._item3);
    }

    public override bool Equals (object obj)
    {
      return EqualityUtility.EqualsEquatable (this, obj);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_item1, _item2, _item3);
    }

    public override string ToString ()
    {
      return string.Format ("<{0}, {1}, {2}>", _item1, _item2, _item3);
    }
  }
}
#endif
