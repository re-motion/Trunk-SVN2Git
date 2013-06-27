// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection ();
    private readonly ConfigurationProperty _xmlnsProperty;


    private readonly SecurityProviderHelper _securityProviderHelper;
    private readonly PrincipalProviderHelper _principalProviderHelper;
    private readonly List<ProviderHelperBase> _providerHelpers = new List<ProviderHelperBase>();

    // construction and disposing

    public SecurityConfiguration()
    {
      _securityProviderHelper = new SecurityProviderHelper (this);
      _providerHelpers.Add (_securityProviderHelper);
      
      _principalProviderHelper = new PrincipalProviderHelper (this);
      _providerHelpers.Add (_principalProviderHelper);

      _xmlnsProperty = new ConfigurationProperty ("xmlns", typeof (string), null, ConfigurationPropertyOptions.None);
      
      _properties.Add (_xmlnsProperty);
      _providerHelpers.ForEach (current => current.InitializeProperties (_properties));
    }

    // methods and properties

    protected override void PostDeserialize()
    {
      base.PostDeserialize();

      _providerHelpers.ForEach (current => current.PostDeserialze());
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

    public IPrincipalProvider PrincipalProvider
    {
      get { return _principalProviderHelper.Provider; }
      set { _principalProviderHelper.Provider = value; }
    }

    public ProviderCollection PrincipalProviders
    {
      get { return _principalProviderHelper.Providers; }
    }
  }
}
