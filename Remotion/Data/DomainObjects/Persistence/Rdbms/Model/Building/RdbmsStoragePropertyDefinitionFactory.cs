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
  /// The <see cref="RdbmsStoragePropertyDefinitionFactory"/> is responsible to create a <see cref="IRdbmsStoragePropertyDefinition"/> objects for <see cref="PropertyDefinition"/>
  /// instances.
  /// </summary>
  public class RdbmsStoragePropertyDefinitionFactory : IRdbmsStoragePropertyDefinitionFactory
  {
    private readonly StorageTypeCalculator _storageTypeCalculator;
    private readonly IStorageProviderDefinitionFinder _providerDefinitionFinder;
    private readonly IStorageNameProvider _storageNameProvider;

    public RdbmsStoragePropertyDefinitionFactory (
        StorageTypeCalculator storageTypeCalculator,
        IStorageNameProvider storageNameProvider,
        IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageTypeCalculator", storageTypeCalculator);
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      _storageTypeCalculator = storageTypeCalculator;
      _storageNameProvider = storageNameProvider;
      _providerDefinitionFinder = providerDefinitionFinder;
    }

    public virtual IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      // TODO Review 4127: Refactor to check for relationEndPointDefinition first, only if no relationEndPointDefinition, get storage type, column definition, etc.

      // var relationEndPointDefinition = propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      // if (relationEndPointDefinition == null)
      // {
      var storageType = _storageTypeCalculator.GetStorageType (propertyDefinition);
      if (storageType == null)
        return new UnsupportedStoragePropertyDefinition();

      var columnDefinition = new ColumnDefinition (
          _storageNameProvider.GetColumnName (propertyDefinition),
          propertyDefinition.PropertyType,
          storageType,
          propertyDefinition.IsNullable || MustBeNullable (propertyDefinition),
          false);

      var relationEndPointDefinition = 
          (RelationEndPointDefinition) propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (relationEndPointDefinition == null)
        return new SimpleStoragePropertyDefinition (columnDefinition);
      // }
      return CreateRelationStoragePropertyDefinition (propertyDefinition, relationEndPointDefinition);
    }

    public ColumnDefinition CreateObjectIDColumnDefinition ()
    {
      return new ColumnDefinition (
          _storageNameProvider.IDColumnName, typeof (ObjectID), _storageTypeCalculator.SqlDataTypeObjectID, false, true);
    }

    public ColumnDefinition CreateClassIDColumnDefinition ()
    {
      return new ColumnDefinition (
          _storageNameProvider.ClassIDColumnName, typeof (string), _storageTypeCalculator.SqlDataTypeClassID, false, false);
    }

    public virtual ColumnDefinition CreateTimestampColumnDefinition ()
    {
      return new ColumnDefinition (
          _storageNameProvider.TimestampColumnName, typeof (object), _storageTypeCalculator.SqlDataTypeTimestamp, false, false);
    }

    protected virtual IObjectIDStoragePropertyDefinition CreateRelationStoragePropertyDefinition (
        PropertyDefinition propertyDefinition,
        RelationEndPointDefinition relationEndPointDefinition)
    {
      var leftProvider = _providerDefinitionFinder.GetStorageProviderDefinition (propertyDefinition.ClassDefinition.StorageGroupType, null);
      var rightEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition();
      var rightProvider = _providerDefinitionFinder.GetStorageProviderDefinition (rightEndPointDefinition.ClassDefinition.StorageGroupType, null);

      if (leftProvider != rightProvider)
      {
        var columnDefinition = new ColumnDefinition (
            _storageNameProvider.GetColumnName (propertyDefinition), //TODO 4127: use GetRelationColumnName
            propertyDefinition.PropertyType,
            _storageTypeCalculator.SqlDataTypeSerializedObjectID,
            propertyDefinition.IsNullable || MustBeNullable (propertyDefinition),
            false);
        return new SerializedObjectIDStoragePropertyDefinition (new SimpleStoragePropertyDefinition (columnDefinition));
      }
      else
      {
        var valueColumnDefinition = new ColumnDefinition (
            _storageNameProvider.GetColumnName (propertyDefinition), //TODO 4127: use GetRelationColumnName
            propertyDefinition.PropertyType,
            _storageTypeCalculator.SqlDataTypeObjectID,
            true,
            false);

        if (!rightEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy)
        {
          return new ObjectIDWithoutClassIDStoragePropertyDefinition (
              new SimpleStoragePropertyDefinition (valueColumnDefinition), rightEndPointDefinition.ClassDefinition);
        }
        else
        {
          var classIDColumnDefinition = new ColumnDefinition (
              _storageNameProvider.GetRelationClassIDColumnName (propertyDefinition),
              typeof (string),
              _storageTypeCalculator.SqlDataTypeClassID,
              true,
              false);
          return new ObjectIDStoragePropertyDefinition (
              new SimpleStoragePropertyDefinition (valueColumnDefinition), new SimpleStoragePropertyDefinition (classIDColumnDefinition));
        }
      }
    }

    protected virtual bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      // CreateSequence can deal with null source objects
      var baseClasses = propertyDefinition.ClassDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
      return baseClasses.Any (cd => _storageNameProvider.GetTableName (cd) != null);
    }
  }
}