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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Wrapper around an <see cref="IEnumerable{T}"/> which supplies element-wise <see cref="Object.Equals(object)"/> and
  /// <see cref="Object.GetHashCode"/> semantics. Use in conjunction with <see cref="CompoundValueEqualityComparer{T}"/>
  /// to get value based semantics for container class members.
  /// </summary>
  /// <typeparam name="TElement"></typeparam>
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

    /// <summary>
    /// Compares the elements of the <see cref="IEnumerable{T}"/> for equality, if the passed <see cref="object"/> 
    /// is an <see cref="IEnumerable{T}"/> .
    /// </summary>
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

    /// <summary>
    /// Returns a hash code based on the members of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_enumerable);
    }
  }

  /// <summary>
  /// EnumerableEqualsWrapper-factory: EnumerableEqualsWrapper.New(<see cref="IEnumerable{T}"/>).
  /// </summary>
  public static class EnumerableEqualsWrapper 
  {
    public static EnumerableEqualsWrapper<T> New<T> (IEnumerable<T> elements)
    {
      return new EnumerableEqualsWrapper<T> (elements);
    }
  }
}