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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// <see cref="StorageTypeCalculator"/> is the base class for type-calculator implementations which determine the storage-specific type for a 
  /// storable column definition.
  /// </summary>
  public abstract class StorageTypeCalculator
  {
    public abstract string SqlDataTypeObjectID { get; }
    public abstract string SqlDataTypeSerializedObjectID { get; }
    public abstract string SqlDataTypeClassID { get; }
    public abstract string SqlDataTypeTimestamp { get; }

    private readonly IStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    protected StorageTypeCalculator (IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
    }

    protected IStorageProviderDefinitionFinder StorageProviderDefinitionFinder
    {
      get { return _storageProviderDefinitionFinder; }
    }

    public virtual string GetStorageType (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      
      if (propertyDefinition.IsObjectID)
      {
        var oppositeClass = propertyDefinition.ClassDefinition.GetMandatoryOppositeClassDefinition (propertyDefinition.PropertyName);

        var classDefinition = propertyDefinition.ClassDefinition;
        var leftStorageProviderDefinition = _storageProviderDefinitionFinder.GetStorageProviderDefinition (classDefinition.StorageGroupType, null);
        var rightStorageProviderDefinition = _storageProviderDefinitionFinder.GetStorageProviderDefinition (oppositeClass.StorageGroupType, null);
        if (leftStorageProviderDefinition == rightStorageProviderDefinition)
          return SqlDataTypeObjectID;
        else
          return SqlDataTypeSerializedObjectID;
      }

      return null;
    }
  }
}