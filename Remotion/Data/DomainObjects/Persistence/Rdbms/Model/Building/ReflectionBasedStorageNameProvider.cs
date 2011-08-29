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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="ReflectionBasedStorageNameProvider"/> provides methods to obtain names for RDBMS items (tables, columns, ...) using default names for
  /// system items ("ID", "ClassID", "Timestamp") and custom attributes (<see cref="DBTableAttribute"/>, <see cref="DBColumnAttribute"/>) for 
  /// user-defined names.
  /// </summary>
  public class ReflectionBasedStorageNameProvider : IStorageNameProvider
  {
    public string IDColumnName
    {
      get { return "ID"; }
    }

    public string ClassIDColumnName
    {
      get { return "ClassID"; }
    }

    public string TimestampColumnName
    {
      get { return "Timestamp"; }
    }

    public string GetTableName (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableAttribute = AttributeUtility.GetCustomAttribute<DBTableAttribute> (classDefinition.ClassType, false);
      if (tableAttribute == null)
        return null;

      return String.IsNullOrEmpty (tableAttribute.Name) ? classDefinition.ID : tableAttribute.Name;
    }

    public string GetViewName (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      return classDefinition.ID + "View";
    }

    public string GetColumnName (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var name = GetColumnNameFromAttribute(propertyDefinition);
      if (name != null)
        return name;

      return propertyDefinition.PropertyInfo.Name;
    }

    public string GetRelationColumnName (RelationEndPointDefinition relationEndPointDefinition)
    {
      var name = GetColumnNameFromAttribute (relationEndPointDefinition.PropertyDefinition);
      if (name != null)
        return name;

      return relationEndPointDefinition.PropertyInfo.Name + "ID";
    }

    public string GetRelationClassIDColumnName (RelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      return GetRelationColumnName (relationEndPointDefinition) + "ClassID";
    }

    public string GetPrimaryKeyConstraintName (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableName = GetTableName (classDefinition);

      return String.Format ("PK_{0}", tableName);
    }

    public string GetForeignKeyConstraintName (ClassDefinition classDefinition, ColumnDefinition foreignKeyColumn)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("foreignKeyColumn", foreignKeyColumn);
      
      var tableName = GetTableName (classDefinition);
      var columnName = foreignKeyColumn.Name;

      return String.Format ("FK_{0}_{1}", tableName, columnName);
    }

    private string GetColumnNameFromAttribute (PropertyDefinition propertyDefinition)
    {
      var attribute = propertyDefinition.PropertyInfo.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (false);
      return attribute != null ? attribute.Identifier : null;
    }
  }
}