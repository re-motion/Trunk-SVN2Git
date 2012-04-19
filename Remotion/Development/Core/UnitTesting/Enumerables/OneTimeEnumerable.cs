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
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Enumerables
{
  /// <summary>
  /// A decorator for <see cref="IEnumerable{T}"/> instances ensuring that they are iterated only once.
  /// </summary>
  /// <typeparam name="T">The element type of the <see cref="IEnumerable{T}"/>.</typeparam>
  public class OneTimeEnumerable<T> : IEnumerable<T>
  {
    private class OneTimeEnumerator : IEnumerator<T>
    {
      private readonly IEnumerator<T> _enumerator;

      public OneTimeEnumerator (IEnumerator<T> enumerator)
      {
        ArgumentUtility.CheckNotNull ("enumerator", enumerator);
        _enumerator = enumerator;
      }

      public T Current
      {
        get { return _enumerator.Current; }
      }

      object IEnumerator.Current
      {
        get { return Current; }
      }

      public void Dispose ()
      {
        _enumerator.Dispose ();
      }

      public bool MoveNext ()
      {
        return _enumerator.MoveNext ();
      }

      public void Reset ()
      {
        throw new NotSupportedException ("OneTimeEnumerator does not support Reset().");
      }
    }

    private readonly IEnumerable<T> _enumerable;
    private bool _isUsed = false;

    public OneTimeEnumerable (IEnumerable<T> enumerable)
    {
      ArgumentUtility.CheckNotNull ("enumerable", enumerable);
      _enumerable = enumerable;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator ()
    {
      if (_isUsed)
        throw new InvalidOperationException ("OneTimeEnumerable can only be iterated once.");
      _isUsed = true;

      return new OneTimeEnumerator (_enumerable.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}