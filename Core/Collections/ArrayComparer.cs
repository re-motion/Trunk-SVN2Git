using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Collections
{
  /// <summary>
  /// Provides equality and hash codes for arrays. Use <see cref="GetComparer"/> to get an instance.
  /// </summary>
  public class ArrayComparer<T> : IEqualityComparer<T[]>
  {
    public struct Wrapper: IEquatable<Wrapper>
    {
      private T[] _array;
    
      public Wrapper (T[] array)
      {
        _array = array;
      }

      public T[] Array
      {
        get { return _array; }
      }

      public static implicit operator T[] (Wrapper wrapper)
      {
        return wrapper.Array; 
      }

      public static implicit operator Wrapper (T[] array)
      {
        return new Wrapper (array);
      }

      public override bool Equals (object obj)
      {
        if (! (obj is Wrapper))
          return false;
        return Equals ((Wrapper)obj);
      }

      public bool Equals (Wrapper other)
      {
        return ArrayComparer<T>.GetComparer().Equals (this._array, other._array);
      }

      public override int GetHashCode ()
      {
        return ArrayComparer<T>.GetComparer().GetHashCode (_array);
      }
    }

    private static ArrayComparer<T> _instance = new ArrayComparer<T>();

    public static ArrayComparer<T> GetComparer()
    {
      return _instance;
    }

    private ArrayComparer()
    {
    }

    public bool Equals (T[] x, T[] y)
    {
      if (ReferenceEquals (x, y))
        return true;

      if (ReferenceEquals (x, null) || ReferenceEquals (y, null))
        return false;

      if (x.Rank != 1 || y.Rank != 1)
        throw new NotSupportedException ("ArrayComparer does not support multidimensional arrays.");

      if (x.Length != y.Length)
        return false;

      for (int i = 0; i < x.Length; ++i)
      {
        if (!Equals (x[i], y[i]))
          return false;
      }

      return true;
    }

    public int GetHashCode (T[] array)
    {
      if (ReferenceEquals (array, null))
        return 0;

      if (array.Rank != 1)
        throw new NotSupportedException ("ArrayComparer does not support multidimensional arrays.");

      int hc = 0;
      for (int i = 0; i < array.Length; ++i)
      {
        T item = array[i];
        if (item != null)
          hc ^= item.GetHashCode ();
      }
      return hc;
    }
  }
}
