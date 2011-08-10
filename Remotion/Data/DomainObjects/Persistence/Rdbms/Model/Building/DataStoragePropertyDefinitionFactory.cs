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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="DataStoragePropertyDefinitionFactory"/> is responsible to create <see cref="IRdbmsStoragePropertyDefinition"/> objects for 
  /// <see cref="PropertyDefinition"/> instances.
  /// </summary>
  public class DataStoragePropertyDefinitionFactory : IDataStoragePropertyDefinitionFactory
  {
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IStorageProviderDefinitionFinder _providerDefinitionFinder;
    private readonly IStorageNameProvider _storageNameProvider;

    public DataStoragePropertyDefinitionFactory (
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      _storageTypeInformationProvider = storageTypeInformationProvider;
      _storageNameProvider = storageNameProvider;
      _providerDefinitionFinder = providerDefinitionFinder;
    }

    public virtual IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var relationEndPointDefinition =
          (RelationEndPointDefinition) propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (relationEndPointDefinition != null)
        return CreateRelationStoragePropertyDefinition (propertyDefinition, relationEndPointDefinition);
      else
        return CreateValueStoragePropertyDefinition (propertyDefinition);
    }

    private IRdbmsStoragePropertyDefinition CreateValueStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      IStorageTypeInformation storageType;
      try
      {
        storageType = _storageTypeInformationProvider.GetStorageType (propertyDefinition);
      }
      catch (NotSupportedException ex)
      {
        return new UnsupportedStoragePropertyDefinition (ex.Message);
      }

      var columnDefinition = new ColumnDefinition (
          _storageNameProvider.GetColumnName (propertyDefinition),
          propertyDefinition.PropertyType,
          storageType,
          propertyDefinition.IsNullable || MustBeNullable (propertyDefinition),
          false);
        
      return new SimpleStoragePropertyDefinition (columnDefinition);
    }

    protected virtual IRdbmsStoragePropertyDefinition CreateRelationStoragePropertyDefinition (
        PropertyDefinition propertyDefinition,
        RelationEndPointDefinition relationEndPointDefinition)
    {
      var leftProvider = _providerDefinitionFinder.GetStorageProviderDefinition (propertyDefinition.ClassDefinition.StorageGroupType, null);
      var rightEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition();
      var rightProvider = _providerDefinitionFinder.GetStorageProviderDefinition (rightEndPointDefinition.ClassDefinition.StorageGroupType, null);

      if (leftProvider != rightProvider)
        return CreateCrossProviderRelationStoragePropertyDefinition(propertyDefinition);
      else
        return CreateSameProviderRelationStoragePropertyDefinition(propertyDefinition, rightEndPointDefinition);
    }

    protected virtual bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      // CreateSequence can deal with null source objects
      var baseClasses = propertyDefinition.ClassDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
      return baseClasses.Any (cd => _storageNameProvider.GetTableName (cd) != null);
    }

    private IRdbmsStoragePropertyDefinition CreateCrossProviderRelationStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      var columnDefinition = new ColumnDefinition (
          _storageNameProvider.GetRelationColumnName (propertyDefinition),
          propertyDefinition.PropertyType,
          _storageTypeInformationProvider.GetStorageTypeForSerializedObjectID(),
          true,
          false);
      return new SerializedObjectIDStoragePropertyDefinition (new SimpleStoragePropertyDefinition (columnDefinition));
    }

    private IRdbmsStoragePropertyDefinition CreateSameProviderRelationStoragePropertyDefinition (
        PropertyDefinition propertyDefinition, 
        IRelationEndPointDefinition rightEndPointDefinition)
    {
      var valueColumnDefinition = new ColumnDefinition (
          _storageNameProvider.GetRelationColumnName (propertyDefinition),
          propertyDefinition.PropertyType,
          _storageTypeInformationProvider.GetStorageTypeForObjectID(),
          // Relation properties are always nullable within the same storage provider
          true,
          false);

      if (!rightEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy)
      {
        // For ClassDefinitions without inheritance hierarchy, we don't include a ClassID relation column - the ClassID is known anyway
        return new ObjectIDWithoutClassIDStoragePropertyDefinition (
            new SimpleStoragePropertyDefinition (valueColumnDefinition), 
            rightEndPointDefinition.ClassDefinition);
      }
      
      var classIDColumnDefinition = new ColumnDefinition (
          _storageNameProvider.GetRelationClassIDColumnName (propertyDefinition),
          typeof (string),
          _storageTypeInformationProvider.GetStorageTypeForClassID(),
          true,
          false);
      return new ObjectIDStoragePropertyDefinition (
          new SimpleStoragePropertyDefinition (valueColumnDefinition), 
          new SimpleStoragePropertyDefinition (classIDColumnDefinition));
    }
  }
}