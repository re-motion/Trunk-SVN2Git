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
using System.Reflection;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
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

    public ColumnDefinitionFactory (StorageTypeCalculator storageTypeCalculator)
    {
      ArgumentUtility.CheckNotNull ("storageTypeCalculator", storageTypeCalculator);

      _storageTypeCalculator = storageTypeCalculator;
    }

    public IColumnDefinition CreateColumnDefinition (
        PropertyDefinition propertyDefinition, IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      var storageType = _storageTypeCalculator.GetStorageType (propertyDefinition, providerDefinitionFinder);
      if (storageType == null)
        return new UnsupportedStorageTypeColumnDefinition();

      var columnDefinition = new SimpleColumnDefinition (
          GetColumnName (propertyDefinition.PropertyInfo),
          propertyDefinition.PropertyType,
          storageType,
          propertyDefinition.IsNullable || MustBeNullable (propertyDefinition));

      var relationEndPointDefinition = propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (relationEndPointDefinition == null)
        return columnDefinition;

      return CreateRelationColumnDefinition (propertyDefinition, providerDefinitionFinder, relationEndPointDefinition, columnDefinition);
    }

    private IColumnDefinition CreateRelationColumnDefinition (
        PropertyDefinition propertyDefinition,
        IStorageProviderDefinitionFinder providerDefinitionFinder,
        IRelationEndPointDefinition relationEndPointDefinition,
        SimpleColumnDefinition foreignKeyColumnDefinition)
    {
      var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition();
      var oppositeClassDefinitionStorageProvider = providerDefinitionFinder.GetStorageProviderDefinition (oppositeEndPointDefinition.ClassDefinition);
      var classDefinitionStorageProvider = providerDefinitionFinder.GetStorageProviderDefinition (propertyDefinition.ClassDefinition);

      if (oppositeEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy
          && classDefinitionStorageProvider.Name == oppositeClassDefinitionStorageProvider.Name)
      {
        var classIdColumnDefinition = new SimpleColumnDefinition (
            RdbmsProvider.GetClassIDColumnName (GetColumnName(propertyDefinition.PropertyInfo)),
            typeof(string),
            _storageTypeCalculator.SqlDataTypeClassID,
            true);
        
        return new ObjectIDWithClassIDColumnDefinition (foreignKeyColumnDefinition, classIdColumnDefinition);
      }
      else
        // TODO Review 3588: Change to return ObjectIDWithClassIDColumnDefinition, but with null classID
        return foreignKeyColumnDefinition;
    }

    public ObjectIDWithClassIDColumnDefinition CreateIDColumnDefinition ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ID", typeof (Guid), _storageTypeCalculator.SqlDataTypeObjectID, false);
      var classIdColumnDefinition = new SimpleColumnDefinition ("ClassID", typeof (string), _storageTypeCalculator.SqlDataTypeClassID, false);

      return new ObjectIDWithClassIDColumnDefinition (objectIDColumn, classIdColumnDefinition);
    }

    public SimpleColumnDefinition CreateTimestampColumnDefinition ()
    {
      return new SimpleColumnDefinition ("Timestamp", typeof (object), _storageTypeCalculator.SqlDataTypeTimestamp, false);
    }

    private string GetColumnName (PropertyInfo propertyInfo)
    {
      var attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (propertyInfo, true);

      if (attribute != null)
        return attribute.Identifier;

      if (ReflectionUtility.IsDomainObject (propertyInfo.PropertyType))
        return propertyInfo.Name + "ID";

      return propertyInfo.Name;
    }

    private bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      // CreateSequence can deal with null source objects
      var baseClasses = propertyDefinition.ClassDefinition.BaseClass.CreateSequence (cd => cd.BaseClass);
      return baseClasses.Any (cd => AttributeUtility.IsDefined (cd.ClassType, typeof (DBTableAttribute), false));
    }
  }
}