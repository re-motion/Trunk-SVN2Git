using System;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Indicates the state of a <see cref="DomainObject"/>.
/// </summary>
public enum StateType
{
  /// <summary>
  /// The <see cref="DomainObject"/> has not changed since it was loaded.
  /// </summary>
  Unchanged = 0,
  /// <summary>
  /// The <see cref="DomainObject"/> has been changed since it was loaded.
  /// </summary>
  Changed = 1,
  /// <summary>
  /// The <see cref="DomainObject"/> has been instanciated and has not been committed.
  /// </summary>
  New = 2,
  /// <summary>
  /// The <see cref="DomainObject"/> has been deleted.
  /// </summary>
  Deleted = 3,
  /// <summary>
  /// The <see cref="DomainObject"/> does not exist any longer.
  /// </summary>
  Discarded = 4
}
}
