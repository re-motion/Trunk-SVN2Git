using System;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
/// <summary>
/// Represents the common information all configuration classes provide.
/// </summary>
public class ConfigurationBase
{
  // types

  // static members and constants

  // member fields

  private bool _resolveTypes;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>ConfigurationBase</b> class from the specified <see cref="BaseFileLoader"/>.
  /// </summary>
  /// <param name="loader">The <see cref="BaseFileLoader"/> to be used for reading the configuration. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="loader"/> is <see langword="null"/>.</exception>
  protected ConfigurationBase (BaseFileLoader loader)
  {
    ArgumentUtility.CheckNotNull ("loader", loader);

    _resolveTypes = loader.ResolveTypes;
  }

  // methods and properties

  /// <summary>
  /// Gets a flag whether type names in the configuration file should be resolved to their corresponding .NET <see cref="Type"/>.
  /// </summary>
  public bool ResolveTypes
  {
    get { return _resolveTypes; }
  }
}
}
