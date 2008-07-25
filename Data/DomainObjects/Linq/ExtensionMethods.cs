using System.Collections.Generic;
using System.Linq;

namespace Remotion.Data.DomainObjects.Linq
{
  public static class ExtensionMethods
  {
    public static ObjectList<T> ToObjectList<T> (this IEnumerable<T> source) 
        where T : DomainObject
    {
      ObjectList<T> result = new ObjectList<T>();
      foreach (T item in source)
        result.Add (item);
      return result;
    }
  }
}