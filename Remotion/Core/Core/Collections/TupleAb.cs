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
using System;
using Remotion.Utilities;

namespace Remotion.Collections
{
  // TODO: Doc
   [Serializable]
  public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
  {
    private readonly T1 _item1;
    private readonly T2 _item2;

    public Tuple (T1 item1, T2 item2)
    {
      _item1 = item1;
      _item2 = item2;
    }

    public T1 Item1
    {
      get { return _item1; }
    }

    public T2 Item2
    {
      get { return _item2; }
    }

    public bool Equals (Tuple<T1, T2> other)
    {
      return EqualityUtility.NotNullAndSameType (this, other)
             && EqualityUtility.Equals (_item1, other._item1) 
             && EqualityUtility.Equals (_item2, other._item2);
    }

    public override bool Equals (object obj)
    {
      return EqualityUtility.EqualsEquatable (this, obj);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_item1, _item2);
    }

    public override string ToString ()
    {
      return string.Format ("<{0}, {1}>", _item1, _item2);
    }
  }
}
