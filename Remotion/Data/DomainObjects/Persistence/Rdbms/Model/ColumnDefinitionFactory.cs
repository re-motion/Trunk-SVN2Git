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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ColumnDefinitionFactory"/> is responsible to create a <see cref="IColumnDefinition"/> objects for <see cref="PropertyDefinition"/>
  /// instances.
  /// </summary>
  public class ColumnDefinitionFactory : IColumnDefinitionFactory
  {
    private readonly StorageTypeCalculator _storageTypeCalculator;
    private readonly IStorageProviderDefinitionFinder _providerDefinitionFinder;
    private readonly IStorageNameProvider _storageNameProvider;

    public ColumnDefinitionFactory (
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

    public virtual IColumnDefinition CreateColumnDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var storageType = _storageTypeCalculator.GetStorageType (propertyDefinition);
      if (storageType == null)
        return new UnsupportedStorageTypeColumnDefinition();

      var columnDefinition = new SimpleColumnDefinition (
          _storageNameProvider.GetColumnName (propertyDefinition),
          propertyDefinition.PropertyType,
          storageType,
          propertyDefinition.IsNullable || MustBeNullable (propertyDefinition),
          false);

      var relationEndPointDefinition = propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (relationEndPointDefinition == null)
        return columnDefinition;

      return CreateRelationColumnDefinition (propertyDefinition, _providerDefinitionFinder, relationEndPointDefinition, columnDefinition);
    }

    public SimpleColumnDefinition CreateObjectIDColumnDefinition ()
    {
      return new SimpleColumnDefinition (
          _storageNameProvider.IDColumnName, typeof (ObjectID), _storageTypeCalculator.SqlDataTypeObjectID, false, true);
    }

    public SimpleColumnDefinition CreateClassIDColumnDefinition ()
    {
      return new SimpleColumnDefinition (
          _storageNameProvider.ClassIDColumnName, typeof (string), _storageTypeCalculator.SqlDataTypeClassID, false, false);
    }

    public virtual SimpleColumnDefinition CreateTimestampColumnDefinition ()
    {
      return new SimpleColumnDefinition (
          _storageNameProvider.TimestampColumnName, typeof (object), _storageTypeCalculator.SqlDataTypeTimestamp, false, false);
    }

    // TODO Review 4064: Change type to IDColumnDefinition
    protected virtual IColumnDefinition CreateRelationColumnDefinition (
        PropertyDefinition propertyDefinition,
        IStorageProviderDefinitionFinder providerDefinitionFinder,
        IRelationEndPointDefinition relationEndPointDefinition,
        SimpleColumnDefinition foreignKeyColumnDefinition)
    {
      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition();
      ClassDefinition classDefinition = oppositeEndPointDefinition.ClassDefinition;
      var oppositeClassDefinitionStorageProvider = providerDefinitionFinder.GetStorageProviderDefinition (classDefinition.StorageGroupType, null);
      ClassDefinition classDefinition1 = propertyDefinition.ClassDefinition;
      var classDefinitionStorageProvider = providerDefinitionFinder.GetStorageProviderDefinition (classDefinition1.StorageGroupType, null);

      if (oppositeEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy
          && classDefinitionStorageProvider.Name == oppositeClassDefinitionStorageProvider.Name)
      {
        var classIdColumnDefinition = new SimpleColumnDefinition (
            _storageNameProvider.GetRelationClassIDColumnName (propertyDefinition),
            typeof (string),
            _storageTypeCalculator.SqlDataTypeClassID,
            true,
            false);

        return new IDColumnDefinition (foreignKeyColumnDefinition, classIdColumnDefinition);
      }
      else
        return new IDColumnDefinition (foreignKeyColumnDefinition, null);
    }

    protected virtual bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      // CreateSequence can deal with null source objects
      var baseClasses = propertyDefinition.ClassDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
      return baseClasses.Any (cd => _storageNameProvider.GetTableName(cd)!=null);
    }
  }
}