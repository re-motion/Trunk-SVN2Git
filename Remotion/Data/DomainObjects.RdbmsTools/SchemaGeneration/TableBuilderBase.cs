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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration
{
  public abstract class TableBuilderBase
  {
    // types

    // static members and constants

    public static bool IsConcreteTable (ClassDefinition classDefinition)
    {
      return classDefinition.StorageEntityDefinition.LegacyEntityName != null
             && (classDefinition.BaseClass == null || classDefinition.BaseClass.GetEntityName() == null);
    }

    public static bool HasClassIDColumn (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var relationEndPointDefinition = propertyDefinition.ClassDefinition.GetRelationEndPointDefinition (propertyDefinition.PropertyName);
      if (relationEndPointDefinition != null)
      {
        var oppositeEndPointDefinition = relationEndPointDefinition.GetOppositeEndPointDefinition(); 

        if (oppositeEndPointDefinition.ClassDefinition.IsPartOfInheritanceHierarchy
            && propertyDefinition.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition.Name 
                == oppositeEndPointDefinition.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition.Name)
          return true;
      }
      return false;
    }

    // member fields

    private readonly StringBuilder _createTableStringBuilder;
    private readonly StringBuilder _dropTableStringBuilder;

    // construction and disposing

    protected TableBuilderBase ()
    {
      _createTableStringBuilder = new StringBuilder();
      _dropTableStringBuilder = new StringBuilder();
    }

    // methods and properties

    public abstract string GetSqlDataType (PropertyDefinition propertyDefinition);
    public abstract void AddToCreateTableScript (ClassDefinition concreteTableClassDefinition, StringBuilder createTableStringBuilder);
    public abstract void AddToDropTableScript (ClassDefinition concreteTableClassDefinition, StringBuilder dropTableStringBuilder);
    public abstract string GetColumn (PropertyDefinition propertyDefinition, bool forceNullable);
    protected abstract string ColumnListOfParticularClassFormatString { get; }
    protected abstract string SqlDataTypeClassID { get; }

    public string GetCreateTableScript ()
    {
      return _createTableStringBuilder.ToString();
    }

    public string GetDropTableScript ()
    {
      return _dropTableStringBuilder.ToString();
    }

    public void AddTables (ClassDefinitionCollection classes)
    {
      ArgumentUtility.CheckNotNull ("classes", classes);

      foreach (ClassDefinition currentClass in classes)
        AddTable (currentClass);
    }

    public void AddTable (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (IsConcreteTable (classDefinition))
      {
        AddToCreateTableScript (classDefinition);
        AddToDropTableScript (classDefinition);
      }
    }

    private void AddToCreateTableScript (ClassDefinition classDefinition)
    {
      if (_createTableStringBuilder.Length != 0)
        _createTableStringBuilder.Append ("\r\n");

      AddToCreateTableScript (classDefinition, _createTableStringBuilder);
    }

    private void AddToDropTableScript (ClassDefinition classDefinition)
    {
      if (_dropTableStringBuilder.Length != 0)
        _dropTableStringBuilder.Append ("\r\n");

      AddToDropTableScript (classDefinition, _dropTableStringBuilder);
    }

    protected string GetColumnList (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      string columnList = string.Empty;
      ClassDefinition currentClassDefinition = classDefinition;
      var addedStorageProperties = new HashSet<string>();

      while (currentClassDefinition != null)
      {
        columnList = GetColumnListOfParticularClass (currentClassDefinition, false, addedStorageProperties) + columnList;

        currentClassDefinition = currentClassDefinition.BaseClass;
      }

      var columnListStringBuilder = new StringBuilder();
      AppendColumnListOfDerivedClasses (classDefinition, columnListStringBuilder, addedStorageProperties);
      columnList += columnListStringBuilder.ToString();
      return columnList;
    }

    private void AppendColumnListOfDerivedClasses (
        ClassDefinition classDefinition, StringBuilder columnListStringBuilder, HashSet<string> addedStorageProperties)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("columnListStringBuilder", columnListStringBuilder);
      ArgumentUtility.CheckNotNull ("addedStorageProperties", addedStorageProperties);

      foreach (ClassDefinition derivedClassDefinition in classDefinition.DerivedClasses)
      {
        columnListStringBuilder.Append (GetColumnListOfParticularClass (derivedClassDefinition, true, addedStorageProperties));
        AppendColumnListOfDerivedClasses (derivedClassDefinition, columnListStringBuilder, addedStorageProperties);
      }
    }

    private string GetColumnListOfParticularClass (ClassDefinition classDefinition, bool forceNullable, HashSet<string> addedStorageProperties)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("addedStorageProperties", addedStorageProperties);

      var columnListStringBuilder = new StringBuilder();

      foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions.GetAllPersistent())
      {
        if (addedStorageProperties.Add (propertyDefinition.StoragePropertyDefinition.Name))
          columnListStringBuilder.Append (GetColumn (propertyDefinition, forceNullable));
      }

      return string.Format (ColumnListOfParticularClassFormatString, classDefinition.ID, columnListStringBuilder);
    }
  }
}