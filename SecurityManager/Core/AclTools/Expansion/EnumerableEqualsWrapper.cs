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
        return enumerable.Cast<object> ().SequenceEqual (_enumerable.Cast<object> ());
      }

      //else if (obj is IEnumerable<TElement>)
      //{
      //  IEnumerable<TElement> enumerable = (IEnumerable<TElement>) obj;
      //  return enumerable.SequenceEqual (_enumerable);
      //}

      //else if (obj is IEnumerable)
      //{
      //  var enumerable = (IEnumerable) obj;
      //  return NonGenericEnumerableEquals (enumerable);
      //}

      else
      {
        return false;
      }
    }

    //private bool NonGenericEnumerableEquals (IEnumerable enumerable)
    //{
    //  IEnumerable<object> x = enumerable.Cast<object> ();
    //  //IEnumerable<TElement> enumerable = (IEnumerable<TElement>) obj;
    //  IEnumerator<TElement> enumerator0 = _enumerable.GetEnumerator ();
    //  IEnumerator enumerator1 = enumerable.GetEnumerator ();
    //  while (true)
    //  {
    //    bool hasNext0 = enumerator0.MoveNext ();
    //    bool hasNext1 = enumerator1.MoveNext ();

    //    if (hasNext0 && hasNext1)
    //    {
    //      // Both enumerators have next element => continue comparing
    //      //if (!enumerator0.Current.Equals (enumerator1.Current))
    //      if (!Object.Equals (enumerator0.Current, enumerator1.Current))
    //      {
    //        return false;
    //      }
    //    }
    //    else
    //    {
    //      // Only if both enumerators are false are the sequences equal
    //      return hasNext0 == hasNext1;
    //    }
    //  }
    //}


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
    public static EnumerableEqualsWrapper<T> New<T> (params T[] elements)
    {
      return new EnumerableEqualsWrapper<T> (elements);
    }
  }

}