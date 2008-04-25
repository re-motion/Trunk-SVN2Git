using System;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// The <see cref="DBStorageGroupAttribute"/> is the standard <see cref="StorageGroupAttribute"/> for types persisted into a database.
  /// </summary>
  /// <remarks>
  /// The <see cref="DBStorageGroupAttribute"/> can be used whenever there is no need for a more granular distribution of types into different 
  /// storage groups.
  /// </remarks>
  public class DBStorageGroupAttribute : StorageGroupAttribute
  {
    public DBStorageGroupAttribute()
    {
    }
  }
}