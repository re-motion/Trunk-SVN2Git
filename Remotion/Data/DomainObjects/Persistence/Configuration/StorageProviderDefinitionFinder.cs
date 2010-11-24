// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  /// <summary>
  /// The <see cref="StorageProviderDefinitionFinder"/> is responsible for finding the <see cref="StorageProviderDefinition"/> for a 
  /// <see cref="ClassDefinition"/>.
  /// </summary>
  public class StorageProviderDefinitionFinder : IStorageProviderDefinitionFinder
  {
    private readonly StorageConfiguration _storageConfiguration;

    public StorageProviderDefinitionFinder (StorageConfiguration storageConfiguration)
    {
      ArgumentUtility.CheckNotNull ("storageConfiguration", storageConfiguration);

      _storageConfiguration = storageConfiguration;
    }

    public StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (classDefinition.StorageGroupType == null)
        return GetDefaultStorageProviderDefinition();

      string storageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (classDefinition.StorageGroupType);
      var storageGroup = _storageConfiguration.StorageGroups[storageGroupName];
      if (storageGroup == null)
        return GetDefaultStorageProviderDefinition();

      return _storageConfiguration.StorageProviderDefinitions.GetMandatory (storageGroup.StorageProviderName);
    }

    private StorageProviderDefinition GetDefaultStorageProviderDefinition ()
    {
      var defaultStorageProviderDefinition = _storageConfiguration.DefaultStorageProviderDefinition;
      if (defaultStorageProviderDefinition == null)
        throw _storageConfiguration.CreateMissingDefaultProviderException (null);

      return defaultStorageProviderDefinition;
    }
  }
}