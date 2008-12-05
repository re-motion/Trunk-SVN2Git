// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using Remotion.Configuration;
using Remotion.Security.Metadata;

namespace Remotion.Security.Configuration
{
  /// <summary> The configuration section for <see cref="Remotion.Security"/>. </summary>
  /// <threadsafety static="true" instance="true" />
  public class SecurityConfiguration : ExtendedConfigurationSection
  {
    // types

    // static members

    private static readonly DoubleCheckedLockingContainer<SecurityConfiguration> s_current;

    static SecurityConfiguration()
    {
      s_current = new DoubleCheckedLockingContainer<SecurityConfiguration> (
          delegate { return (SecurityConfiguration) ConfigurationManager.GetSection ("remotion.security") ?? new SecurityConfiguration(); });
    }

    public static SecurityConfiguration Current
    {
      get { return s_current.Value; }
    }

    protected static void SetCurrent (SecurityConfiguration configuration)
    {
      s_current.Value = configuration;
    }

    // member fields

    private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection ();
    private readonly ConfigurationProperty _xmlnsProperty;

    private readonly ConfigurationProperty _functionalSecurityStrategyProperty;
    private DoubleCheckedLockingContainer<IFunctionalSecurityStrategy> _functionalSecurityStrategy;

    private PermissionProviderHelper _permissionProviderHelper;
    private SecurityProviderHelper _securityProviderHelper;
    private UserProviderHelper _userProviderHelper;
    private GlobalAccessTypeCacheProviderHelper _globalAccessTypeCacheProviderHelper;
    private List<ProviderHelperBase> _providerHelpers = new List<ProviderHelperBase>();

    // construction and disposing

    public SecurityConfiguration()
    {
      _permissionProviderHelper = new PermissionProviderHelper (this);
      _providerHelpers.Add (_permissionProviderHelper);
      
      _securityProviderHelper = new SecurityProviderHelper (this);
      _providerHelpers.Add (_securityProviderHelper);
      
      _userProviderHelper = new UserProviderHelper (this);
      _providerHelpers.Add (_userProviderHelper);
      
      _globalAccessTypeCacheProviderHelper = new GlobalAccessTypeCacheProviderHelper (this);
      _providerHelpers.Add (_globalAccessTypeCacheProviderHelper);

      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);

      _functionalSecurityStrategy =
          new DoubleCheckedLockingContainer<IFunctionalSecurityStrategy> (delegate { return FunctionalSecurityStrategyElement.CreateInstance(); });
      _functionalSecurityStrategyProperty = new ConfigurationProperty (
          "functionalSecurityStrategy",
          typeof (TypeElement<IFunctionalSecurityStrategy, FunctionalSecurityStrategy>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_xmlnsProperty);
      _providerHelpers.ForEach (delegate (ProviderHelperBase current) { current.InitializeProperties (_properties); });
      _properties.Add (_functionalSecurityStrategyProperty);
    }

    // methods and properties

    protected override void PostDeserialize()
    {
      base.PostDeserialize();

      _providerHelpers.ForEach (delegate (ProviderHelperBase current) { current.PostDeserialze(); });
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public ISecurityProvider SecurityProvider
    {
      get { return _securityProviderHelper.Provider; }
      set { _securityProviderHelper.Provider = value; }
    }

    public ProviderCollection SecurityProviders
    {
      get { return _securityProviderHelper.Providers; }
    }

    public IUserProvider UserProvider
    {
      get { return _userProviderHelper.Provider; }
      set { _userProviderHelper.Provider = value; }
    }

    public ProviderCollection UserProviders
    {
      get { return _userProviderHelper.Providers; }
    }


    public IPermissionProvider PermissionProvider
    {
      get { return _permissionProviderHelper.Provider; }
      set { _permissionProviderHelper.Provider = value; }
    }

    public ProviderCollection PermissionProviders
    {
      get { return _permissionProviderHelper.Providers; }
    }


    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get { return _functionalSecurityStrategy.Value; }
      set { _functionalSecurityStrategy.Value = value; }
    }

    protected TypeElement<IFunctionalSecurityStrategy> FunctionalSecurityStrategyElement
    {
      get { return (TypeElement<IFunctionalSecurityStrategy>) this[_functionalSecurityStrategyProperty]; }
      set { this[_functionalSecurityStrategyProperty] = value; }
    }


    public IGlobalAccessTypeCacheProvider GlobalAccessTypeCacheProvider
    {
      get { return _globalAccessTypeCacheProviderHelper.Provider; }
      set { _globalAccessTypeCacheProviderHelper.Provider = value; }
    }

    public ProviderCollection GlobalAccessTypeCacheProviders
    {
      get { return _globalAccessTypeCacheProviderHelper.Providers; }
    }
  }
}
