using System;

namespace Remotion.Data.DomainObjects.Queries
{
/// <summary>
/// Indicates the type of a <see cref="QueryParameter"/>.
/// </summary>
public enum QueryParameterType
{
  /// <summary>
  /// Instances of <see cref="QueryParameter"/> will be treated as parameters in the query.
  /// </summary>
  Value = 0,

  /// <summary>
  /// Instances of <see cref="QueryParameter"/> will replaced inline into the query.
  /// </summary>
  Text = 1
}
}
