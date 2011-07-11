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

      var storageType = _storageTypeCalculator.GetStorageType (propertyDefinition);
      if (storageType == null)
        return new UnsupportedStoragePropertyDefinition();

      var columnDefinition = new ColumnDefinition (
          _storageNameProvider.GetColumnName (propertyDefinition),
          propertyDefinition.PropertyType,
          storageType,
          propertyDefinition.IsNullable || MustBeNullable (propertyDefinition),
          false);

      var relationEndPointDefinition = propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (relationEndPointDefinition == null)
        return new SimpleStoragePropertyDefinition(columnDefinition);

      return CreateRelationStoragePropertyDefinition (propertyDefinition, relationEndPointDefinition, columnDefinition);
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

    protected virtual ObjectIDStoragePropertyDefinition CreateRelationStoragePropertyDefinition (
        PropertyDefinition propertyDefinition,
        IRelationEndPointDefinition relationEndPointDefinition,
        ColumnDefinition foreignKeyColumnDefinition)
    {
      var leftProvider = _providerDefinitionFinder.GetStorageProviderDefinition (propertyDefinition.ClassDefinition.StorageGroupType,  null);
      var rightEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();
      var rightProvider = _providerDefinitionFinder.GetStorageProviderDefinition (rightEndPointDefinition.ClassDefinition.StorageGroupType, null);

      if (rightEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy && leftProvider.Name == rightProvider.Name)
      {
        var classIdColumnDefinition = new ColumnDefinition (
            _storageNameProvider.GetRelationClassIDColumnName (propertyDefinition),
            typeof (string),
            _storageTypeCalculator.SqlDataTypeClassID,
            true,
            false);

        return new ObjectIDStoragePropertyDefinition (foreignKeyColumnDefinition, classIdColumnDefinition);
      }
      else
      {
        return new ObjectIDStoragePropertyDefinition (foreignKeyColumnDefinition, null);
      }
    }

    protected virtual bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      // CreateSequence can deal with null source objects
      var baseClasses = propertyDefinition.ClassDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
      return baseClasses.Any (cd => _storageNameProvider.GetTableName(cd)!=null);
    }
  }
}