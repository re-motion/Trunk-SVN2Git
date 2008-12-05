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
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class StorageConfiguration: ExtendedConfigurationSection
  {
    [Obsolete ("Use DefaultStorageProviderDefinition instead. (Version 1.11.14.0")]
    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return DefaultStorageProviderDefinition; }
    }

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly StorageProviderDefinitionHelper _defaultStorageProviderDefinitionHelper;
    private readonly List<ProviderHelperBase> _providerHelpers = new List<ProviderHelperBase>();
    private readonly ConfigurationProperty _storageProviderGroupsProperty;

    public StorageConfiguration()
    {
      _storageProviderGroupsProperty = new ConfigurationProperty (
          "groups",
          typeof (ConfigurationElementCollection<StorageGroupElement>),
          null,
          ConfigurationPropertyOptions.None);

      _defaultStorageProviderDefinitionHelper = new StorageProviderDefinitionHelper (this);
      _providerHelpers.Add (_defaultStorageProviderDefinitionHelper);

      _properties.Add (_storageProviderGroupsProperty);
      _providerHelpers.ForEach (current => current.InitializeProperties (_properties));
    }

    public StorageConfiguration (ProviderCollection<StorageProviderDefinition> providers, StorageProviderDefinition defaultProvider)
        : this()
    {
      ArgumentUtility.CheckNotNull ("providers", providers);
      ArgumentUtility.CheckNotNull ("defaultProvider", defaultProvider);

      _defaultStorageProviderDefinitionHelper.Provider = defaultProvider;

      ProviderCollection<StorageProviderDefinition> providersCopy = CopyProvidersAsReadOnly (providers);
      _defaultStorageProviderDefinitionHelper.Providers = providersCopy;
    }

    public ConfigurationElementCollection<StorageGroupElement> StorageGroups
    {
      get { return (ConfigurationElementCollection<StorageGroupElement>) this[_storageProviderGroupsProperty]; }
    }

    public StorageProviderDefinition DefaultStorageProviderDefinition
    {
      get { return _defaultStorageProviderDefinitionHelper.Provider; }
    }

    public ProviderCollection<StorageProviderDefinition> StorageProviderDefinitions
    {
      get { return _defaultStorageProviderDefinitionHelper.Providers; }
    }

    protected override void PostDeserialize()
    {
      base.PostDeserialize();

      _providerHelpers.ForEach (current => current.PostDeserialze());
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
