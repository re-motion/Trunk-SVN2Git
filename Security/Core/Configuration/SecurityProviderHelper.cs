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
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Reflection;
using Remotion.Configuration;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="ISecurityProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class SecurityProviderHelper : ProviderHelperBase<ISecurityProvider>
  {
    private const string c_nullSecurityProviderWellKnownName = "None";
    private const string c_securityManagerSecurityProviderWellKnownName = "SecurityManager";

    private readonly object _sync = new object();
    private Type _securityManagerSecurityServiceType;

    public SecurityProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultSecurityProvider", c_nullSecurityProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("securityProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_nullSecurityProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_securityManagerSecurityProviderWellKnownName);
      
      if (DefaultProviderName.Equals (c_securityManagerSecurityProviderWellKnownName, StringComparison.Ordinal))
        EnsureSecurityManagerServiceTypeInitialized();
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownNullSecurityProvider (collection);
      EnsureWellKnownSecurityManagerSecurityProvider (collection);
    }

    private void EnsureWellKnownNullSecurityProvider (ProviderCollection collection)
    {
      collection.Add (new NullSecurityProvider(c_nullSecurityProviderWellKnownName, new NameValueCollection()));
    }

    private void EnsureWellKnownSecurityManagerSecurityProvider (ProviderCollection collection)
    {
      if (_securityManagerSecurityServiceType != null)
      {
        collection.Add ((ExtendedProviderBase) Activator.CreateInstance (
            _securityManagerSecurityServiceType, 
            new object[] {c_securityManagerSecurityProviderWellKnownName, new NameValueCollection()}));
      }
    }

    private void EnsureSecurityManagerServiceTypeInitialized ()
    {
      if (_securityManagerSecurityServiceType == null)
      {
        lock (_sync)
        {
          if (_securityManagerSecurityServiceType == null)
          {
            _securityManagerSecurityServiceType = GetType (
                DefaultProviderNameProperty,
                new AssemblyName ("Remotion.SecurityManager"),
                "Remotion.SecurityManager.SecurityService");
          }
        }
      }
    }
  }
}
