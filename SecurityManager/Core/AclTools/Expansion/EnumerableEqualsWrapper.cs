// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class EnumerableEqualsWrapper<TElement> : IEnumerable<TElement>
  {
    private readonly IEnumerable<TElement> _enumerable;

    public EnumerableEqualsWrapper (IEnumerable<TElement> enumerable)
    {
      _enumerable = enumerable;
    }

    public IEnumerable<TElement> Enumerable
    {
      get { return _enumerable; }
    }


    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
      {
        return false;
      }
      else if (ReferenceEquals (this, obj))
      {
        return true;
      }
      else if (obj is IEnumerable)
      {
        var enumerable = (IEnumerable) obj;
        return enumerable.Cast<object> ().SequenceEqual (Enumerable.Cast<object> ());
      }
      else
      {
        return false;
      }
    }


    public IEnumerator<TElement> GetEnumerator ()
    {
      return _enumerable.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }


    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_enumerable);
    }
  }

  public static class EnumerableEqualsWrapper 
  {
    public static EnumerableEqualsWrapper<T> New<T> (IEnumerable<T> elements)
    {
      return new EnumerableEqualsWrapper<T> (elements);
    }
  }

}
