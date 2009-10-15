using System;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Defines a file pattern for the <see cref="FilePatternRootAssemblyFinder"/> as well as how to interpret that pattern.
  /// </summary>
  public struct FilePatternSpecification
  {
    public FilePatternSpecification (string filePattern, FilePatternSpecificationKind kind)
        : this()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filePattern", filePattern);

      FilePattern = filePattern;
      Kind = kind;
    }

    public string FilePattern { get; private set; }
    public FilePatternSpecificationKind Kind { get; private set; }
  }
}