using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Holds an <see cref="System.Reflection.AssemblyName"/> for the <see cref="NamedRootAssemblyFinder"/> as well as a flag indicating whether to 
  /// include referenced assemblies.
  /// </summary>
  public struct AssemblyNameSpecification
  {
    public AssemblyNameSpecification (AssemblyName assemblyName, bool followReferences)
        : this()
    {
      ArgumentUtility.CheckNotNull ("assemblyName", assemblyName);

      AssemblyName = assemblyName;
      FollowReferences = followReferences;
    }

    public AssemblyName AssemblyName { get; private set; }
    public bool FollowReferences { get; private set; }

    public override string ToString ()
    {
      return "Specification: " + AssemblyName;
    }
  }
}