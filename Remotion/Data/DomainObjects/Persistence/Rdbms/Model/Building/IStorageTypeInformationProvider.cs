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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// <see cref="IStorageTypeInformationProvider"/> is the base class for type-calculator implementations which determine the storage-specific type for a 
  /// storable column definition.
  /// </summary>
  public interface IStorageTypeInformationProvider
  {
    StorageTypeInformation ObjectIDStorageType { get; }
    StorageTypeInformation SerializedObjectIDStorageType { get; }
    StorageTypeInformation ClassIDStorageType { get; }
    StorageTypeInformation TimestampStorageType { get; }
    
    bool IsTypeSupported (Type type);
    StorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition);
    StorageTypeInformation GetStorageType (Type type);
  }
}