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
using System.Collections.Generic;
using System.Configuration;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class PersistenceConfiguration: ExtendedConfigurationSection
  {
    private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private StorageProviderDefinitionHelper _storageProviderDefinitionHelper;
    private List<ProviderHelperBase> _providerHelpers = new List<ProviderHelperBase>();
    private readonly ConfigurationProperty _storageProviderGroupsProperty;

    public PersistenceConfiguration()
    {
      _storageProviderGroupsProperty = new ConfigurationProperty (
          "groups",
          typeof (ConfigurationElementCollection<StorageGroupElement>),
          null,
          ConfigurationPropertyOptions.None);

      _storageProviderDefinitionHelper = new StorageProviderDefinitionHelper (this);
      _providerHelpers.Add (_storageProviderDefinitionHelper);

      _properties.Add (_storageProviderGroupsProperty);
      _providerHelpers.ForEach (delegate (ProviderHelperBase current) { current.InitializeProperties (_properties); });
    }

    public PersistenceConfiguration (ProviderCollection<StorageProviderDefinition> providers, StorageProviderDefinition provider)
        : this()
    {
      ArgumentUtility.CheckNotNull ("providers", providers);
      ArgumentUtility.CheckNotNull ("provider", provider);

      _storageProviderDefinitionHelper.Provider = provider;

      ProviderCollection<StorageProviderDefinition> providersCopy = CopyProvidersAsReadOnly (providers);
      _storageProviderDefinitionHelper.Providers = providersCopy;
    }

    public ConfigurationElementCollection<StorageGroupElement> StorageGroups
    {
      get { return (ConfigurationElementCollection<StorageGroupElement>) this[_storageProviderGroupsProperty]; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinitionHelper.Provider; }
    }

    public ProviderCollection<StorageProviderDefinition> StorageProviderDefinitions
    {
      get { return _storageProviderDefinitionHelper.Providers; }
    }

    protected override void PostDeserialize()
    {
      base.PostDeserialize();

      _providerHelpers.ForEach (delegate (ProviderHelperBase current) { current.PostDeserialze(); });
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    private ProviderCollection<StorageProviderDefinition> CopyProvidersAsReadOnly (ProviderCollection<StorageProviderDefinition> providers)
    {
      ProviderCollection<StorageProviderDefinition> providersCopy = new ProviderCollection<StorageProviderDefinition>();
      foreach (StorageProviderDefinition provider in providers)
        providersCopy.Add (provider);

      providersCopy.SetReadOnly();
      return providersCopy;
    }
  }
}
