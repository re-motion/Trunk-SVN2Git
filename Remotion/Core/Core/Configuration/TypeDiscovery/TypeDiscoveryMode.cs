using System;
using System.ComponentModel.Design;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Defines how type discovery should work.
  /// </summary>
  public enum TypeDiscoveryMode
  {
    /// <summary>
    /// Chooses automatic type discovery - the application's bin directory is searched for assemblies. The types are discovered from those assemblies
    /// and their referenced assemblies.
    /// </summary>
    Automatic,
    /// <summary>
    /// Chooses a custom <see cref="IRootAssemblyFinder"/> which searches for root assemblies. The types are discovered from those assemblies. 
    /// Whether types from referenced assemblies are also included is defined by the <see cref="IRootAssemblyFinder"/>. 
    /// See <see cref="TypeDiscoveryConfiguration.CustomRootAssemblyFinder"/>.
    /// </summary>
    CustomRootAssemblyFinder,
    /// <summary>
    /// Chooses a number of specific root assemblies. The types are discovered from those assemblies. Whether types from referenced assemblies are 
    /// also included is defined by the user.
    /// See <see cref="TypeDiscoveryConfiguration.SpecificRootAssemblies"/>.
    /// </summary>
    SpecificRootAssemblies,
    /// <summary>
    /// Chooses a custom <see cref="ITypeDiscoveryService"/> implementation. The types are discovered by that service.
    /// See <see cref="TypeDiscoveryConfiguration.CustomTypeDiscoveryService"/>.
    /// </summary>
    CustomTypeDiscoveryService
  }
}