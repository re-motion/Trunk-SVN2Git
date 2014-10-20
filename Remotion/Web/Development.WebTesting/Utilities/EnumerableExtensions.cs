using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Various extension methods for <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static class EnumerableExtensions
  {
    /// <summary>
    /// Returns the index of the first item in <paramref name="enumerable"/> matching the given <paramref name="predicate"/>.
    /// </summary>
    /// <remarks>
    /// This method enumerates the <paramref name="enumerable"/>.
    /// </remarks>
    /// <returns>The index of the matching item, or -1 if no item matches.</returns>
    public static int IndexOf<T> ([NotNull] this IEnumerable<T> enumerable, [NotNull] Func<T, bool> predicate)
    {
      ArgumentUtility.CheckNotNull ("enumerable", enumerable);
      ArgumentUtility.CheckNotNull ("predicate", predicate);

      var index = 0;
      foreach(var item in enumerable)
      {
        if (predicate (item))
          return index;

        index++;
      }

      return -1;
    }
  }
}