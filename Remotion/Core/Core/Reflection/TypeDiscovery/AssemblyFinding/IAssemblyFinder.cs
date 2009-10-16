using System;
using System.Reflection;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Provides an interface for classes that find assemblies.
  /// </summary>
  public interface IAssemblyFinder
  {
    /// <summary>
    /// Finds assemblies as defined by implementers of this interface.
    /// </summary>
    Assembly[] FindAssemblies ();
  }
}