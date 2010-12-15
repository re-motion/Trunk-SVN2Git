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
  /// The <see cref="ColumnDefinitionFactory"/> is responsible to create a <see cref="SimpleColumnDefinition"/> from a <see cref="PropertyDefinition"/>.
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
      if (storageType != null)
      {
        var columnDefinition = new SimpleColumnDefinition (
            GetColumnName (propertyDefinition.PropertyInfo),
            propertyDefinition.PropertyType,
            storageType,
            propertyDefinition.IsNullable || MustBeNullable (propertyDefinition));

        if (!HasClassIDColumn (propertyDefinition, providerDefinitionFinder))
          return columnDefinition;
        
        var classIdColumnDefinition = new SimpleColumnDefinition ("ClassID", typeof (string), _storageTypeCalculator.SqlDataTypeClassID, false);
        return new ObjectIDWithClassIDColumnDefinition (columnDefinition, classIdColumnDefinition);
      }
      else
      {
        return new UnsupportedStorageTypeColumnDefinition (GetColumnName (propertyDefinition.PropertyInfo));
      }
    }

    public ObjectIDWithClassIDColumnDefinition CreateIDColumnDefinition ()
    {
      var objectIDColumn = new SimpleColumnDefinition ("ID", typeof (Guid), _storageTypeCalculator.SqlDataTypeObjectID, false);
      var classIdColumnDefinition = new SimpleColumnDefinition ("ClassID", typeof (string), _storageTypeCalculator.SqlDataTypeClassID, false);

      return new ObjectIDWithClassIDColumnDefinition (objectIDColumn, classIdColumnDefinition);
    }

    public SimpleColumnDefinition CreateTimestampColumnDefinition ()
    {
      return new SimpleColumnDefinition ("Timestamp", typeof (object), "rowversion", false);
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

    private bool HasClassIDColumn (PropertyDefinition propertyDefinition, IStorageProviderDefinitionFinder providerDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("providerDefinitionFinder", providerDefinitionFinder);

      var relationEndPointDefinition = propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (relationEndPointDefinition != null)
      {
        var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition ();
        var oppositeClassDefinitionStorageProvider = providerDefinitionFinder.GetStorageProviderDefinition (oppositeEndPointDefinition.ClassDefinition);
        var classDefinitionStorageProvider = providerDefinitionFinder.GetStorageProviderDefinition (propertyDefinition.ClassDefinition);
       
        if (oppositeEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy 
          && classDefinitionStorageProvider.Name == oppositeClassDefinitionStorageProvider.Name)
          return true;
      }
      return false;
    }

    private bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      return propertyDefinition.ClassDefinition.BaseClass != null
             && propertyDefinition.ClassDefinition.BaseClass.CreateSequence (cd => cd.BaseClass).Any (
                 cd => AttributeUtility.IsDefined (cd.ClassType, typeof (DBTableAttribute), false));
    }
  }
}