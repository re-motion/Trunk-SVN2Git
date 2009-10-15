using System;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Defines how a file pattern specification should be interpreted.
  /// </summary>
  public enum FilePatternSpecificationKind
  {
    /// <summary>
    /// The files described by the pattern should be included, but referenced assemblies should not be followed.
    /// </summary>
    IncludeNoFollow,
    /// <summary>
    /// The files described by the pattern should be included, and their referenced assemblies should be followed.
    /// </summary>
    IncludeFollowReferences,
    /// <summary>
    /// The files described by the pattern should be excluded. This removes any files previously included, but it does not affect any files
    /// included by a subsequent specification.
    /// </summary>
    Exclude
  }
}