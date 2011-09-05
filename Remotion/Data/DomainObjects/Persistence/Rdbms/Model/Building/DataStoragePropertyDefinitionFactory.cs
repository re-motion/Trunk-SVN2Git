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
using Remotion.Data.DomainObjects.Persistence.Configuration;
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
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IStorageProviderDefinitionFinder _providerDefinitionFinder;
    private readonly IStorageNameProvider _storageNameProvider;

    public DataStoragePropertyDefinitionFactory (
        StorageProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      _storageProviderDefinition = storageProviderDefinition;
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
      {
        Assertion.IsTrue (propertyDefinition.PropertyType == typeof (ObjectID));
        var rightEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();
        var relationColumnName = _storageNameProvider.GetRelationColumnName (relationEndPointDefinition);
        var relationClassIDColumnName = _storageNameProvider.GetRelationClassIDColumnName (relationEndPointDefinition);
        return CreateRelationStoragePropertyDefinition (
            propertyDefinition, 
            rightEndPointDefinition.ClassDefinition, 
            relationColumnName, 
            relationClassIDColumnName);
      }
      else
        return CreateValueStoragePropertyDefinition (propertyDefinition);
    }

    private IRdbmsStoragePropertyDefinition CreateValueStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      IStorageTypeInformation storageType;
      try
      {
        storageType = _storageTypeInformationProvider.GetStorageType (propertyDefinition, MustBeNullable (propertyDefinition));
      }
      catch (NotSupportedException ex)
      {
        return new UnsupportedStoragePropertyDefinition (propertyDefinition.PropertyType, ex.Message);
      }

      var columnDefinition = new ColumnDefinition (_storageNameProvider.GetColumnName (propertyDefinition), storageType, false);
      return new SimpleStoragePropertyDefinition (propertyDefinition.PropertyType, columnDefinition);
    }

    protected virtual IRdbmsStoragePropertyDefinition CreateRelationStoragePropertyDefinition (
        PropertyDefinition propertyDefinition, 
        ClassDefinition relatedClassDefinition, string relationColumnName, string relationClassIDColumnName)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedClassDefinition", relatedClassDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("relationColumnName", relationColumnName);
      ArgumentUtility.CheckNotNullOrEmpty ("relationClassIDColumnName", relationClassIDColumnName);

      var relatedStorageProviderDefinition = _providerDefinitionFinder.GetStorageProviderDefinition (relatedClassDefinition.StorageGroupType, null);

      if (_storageProviderDefinition != relatedStorageProviderDefinition)
        return CreateCrossProviderRelationStoragePropertyDefinition (relationColumnName);
      else
        return CreateSameProviderRelationStoragePropertyDefinition (relatedClassDefinition, relationColumnName, relationClassIDColumnName);
    }

    protected virtual bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      // CreateSequence can deal with null source objects
      var baseClasses = propertyDefinition.ClassDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
      return baseClasses.Any (cd => _storageNameProvider.GetTableName (cd) != null);
    }

    private IRdbmsStoragePropertyDefinition CreateCrossProviderRelationStoragePropertyDefinition (string relationColumnName)
    {
      var storageTypeInfo = _storageTypeInformationProvider.GetStorageTypeForSerializedObjectID (true);
      var columnDefinition = new ColumnDefinition (relationColumnName, storageTypeInfo, false);
      return new SerializedObjectIDStoragePropertyDefinition (new SimpleStoragePropertyDefinition (typeof (ObjectID), columnDefinition));
    }

    private IRdbmsStoragePropertyDefinition CreateSameProviderRelationStoragePropertyDefinition (
        ClassDefinition relatedClassDefinition, 
        string relationColumnName, 
        string relationClassIDColumnName)
    {
      // Relation properties are always nullable within the same storage provider
      var storageTypeInfo = _storageTypeInformationProvider.GetStorageTypeForID (true);
      var valueColumnDefinition = new ColumnDefinition (relationColumnName, storageTypeInfo, false);

      if (!relatedClassDefinition.IsPartOfInheritanceHierarchy)
      {
        // For ClassDefinitions without inheritance hierarchy, we don't include a ClassID relation column - the ClassID is known anyway
        return new ObjectIDWithoutClassIDStoragePropertyDefinition (
            new SimpleStoragePropertyDefinition (typeof (object), valueColumnDefinition),
            relatedClassDefinition);
      }

      var storageTypeForClassID = _storageTypeInformationProvider.GetStorageTypeForClassID (true);
      var classIDColumnDefinition = new ColumnDefinition (relationClassIDColumnName, storageTypeForClassID, false);
      return new ObjectIDStoragePropertyDefinition (
          new SimpleStoragePropertyDefinition (typeof (object), valueColumnDefinition), 
          new SimpleStoragePropertyDefinition (typeof (string), classIDColumnDefinition));
    }
  }
}