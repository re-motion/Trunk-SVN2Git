using System;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Defines how type discovery should work.
  /// </summary>
  public enum TypeDiscoveryKind
  {
    /// <summary>
    /// Chooses automatic type discovery, the application's bin directory is searched for assemblies. The types are discovered from those assemblies
    /// and their referenced assemblies.
    /// </summary>
    Automatic,
    /// <summary>
    /// Chooses a custom root assembly finder which searches for root assemblies. The types are discovered from those assemblies. Whether types from
    /// referenced assemblies are also included is defined by the finder.
    /// </summary>
    CustomRootAssemblyFinder,
    /// <summary>
    /// Chooses a number of specific root assemblies. The types are discovered from those assemblies. Whether types from referenced assemblies are 
    /// also included is defined by the user.
    /// </summary>
    SpecificRootAssemblies,
    /// <summary>
    /// Chooses a custom type discovery service implementation. The types are discovered by that service.
    /// </summary>
    CustomTypeDiscoveryService
  }
}