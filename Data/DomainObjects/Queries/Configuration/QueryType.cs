using System;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Indicates the type of a <see cref="QueryDefinition"/>.
/// </summary>
public enum QueryType
{
  /// <summary>
  /// Instances of a <see cref="QueryDefinition"/> return a collection of <see cref="DomainObject"/>s.
  /// </summary>
  Collection = 0,

  /// <summary>
  /// Instances of a <see cref="QueryDefinition"/> return only a single value.
  /// </summary>
  Scalar = 1
}
}
