/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
