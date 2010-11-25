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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="PersistenceModelLoader"/> is responsible to load a persistence model for a relational database.
  /// </summary>
  public class PersistenceModelLoader : IPersistenceModelLoader
  {
    private readonly IStoragePropertyDefinitionFactory _storagePropertyDefinitionFactory;
    private readonly string _storageProviderID;

    public PersistenceModelLoader (IStoragePropertyDefinitionFactory storagePropertyDefinitionFactory, string storageProviderID)
    {
      ArgumentUtility.CheckNotNull ("storagePropertyDefinitionFactory", storagePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNullOrEmpty ("storageProviderID", storageProviderID);

      _storagePropertyDefinitionFactory = storagePropertyDefinitionFactory;
      _storageProviderID = storageProviderID;
    }

    public void ApplyPersistenceModelToHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      EnsurePersistenceModelApplied (classDefinition);

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        ApplyPersistenceModelToHierarchy (derivedClass);
    }

    private void EnsurePersistenceModelApplied (ClassDefinition classDefinition)
    {
      if (classDefinition.StorageEntityDefinition == null)
      {
        var storageEntity = CreateStorageEntityDefinition (classDefinition);
        classDefinition.SetStorageEntity (storageEntity);
      }

      Assertion.IsNotNull (classDefinition.StorageEntityDefinition);
    }

    private IStorageEntityDefinition CreateStorageEntityDefinition (ClassDefinition classDefinition)
    {
      var tableAttribute = AttributeUtility.GetCustomAttribute<DBTableAttribute> (classDefinition.ClassType, false);
      if (tableAttribute != null)
        return CreateTableDefinition (classDefinition, tableAttribute);

      var hasBaseClassWithDBTableAttribute = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Where (cd => AttributeUtility.IsDefined<DBTableAttribute> (cd.ClassType, false))
          .Any();
      if (hasBaseClassWithDBTableAttribute)
        return CreateFilterViewDefinition (classDefinition);

      return CreateUnionViewDefinition (classDefinition);
    }

    private IStorageEntityDefinition CreateTableDefinition (ClassDefinition classDefinition, DBTableAttribute tableAttribute)
    {
      string tableName = string.IsNullOrEmpty (tableAttribute.Name) ? classDefinition.ID : tableAttribute.Name;

      return new TableDefinition (_storageProviderID, tableName, classDefinition.ID+"View", GetColumnDefinitionsForHierarchy (classDefinition));
    }

    private IStorageEntityDefinition CreateFilterViewDefinition (ClassDefinition classDefinition)
    {
      // The following call is potentially recursive (GetEntityDefinition -> EnsurePersistenceModelApplied -> CreateFilterViewDefinition), but this is
      // guaranteed to terminate because we know at this point that there is a class in the classDefinition's base hierarchy that will get a 
      // TableDefinition
      var baseStorageEntityDefinition = GetEntityDefinition (classDefinition.BaseClass);

      var actualAndBaseClassColumns = new HashSet<ColumnDefinition> (GetColumnDefinitionsForHierarchy (classDefinition));

      return new FilterViewDefinition (
          _storageProviderID,
          classDefinition.ID + "View",
          baseStorageEntityDefinition,
          classDefinition.ID,
          actualAndBaseClassColumns.Contains);
    }

    private IStorageEntityDefinition CreateUnionViewDefinition (ClassDefinition classDefinition)
    {
      var derivedStorageEntityDefinitions =
          from ClassDefinition derivedClass in classDefinition.DerivedClasses
          select GetEntityDefinition (derivedClass);

      return new UnionViewDefinition (_storageProviderID, classDefinition.ID + "View", derivedStorageEntityDefinitions);
    }

    private IEntityDefinition GetEntityDefinition (ClassDefinition classDefinition)
    {
      EnsurePersistenceModelApplied (classDefinition);
      
      return (IEntityDefinition) classDefinition.StorageEntityDefinition;
    }

    private ColumnDefinition GetColumnDefinition (PropertyDefinition propertyDefinition)
    {
      Assertion.IsTrue (propertyDefinition.StorageClass == StorageClass.Persistent);

      if (propertyDefinition.StoragePropertyDefinition == null)
      {
        var storageProperty = _storagePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);
        propertyDefinition.SetStorageProperty (storageProperty);
      }

      var columnDefinition = propertyDefinition.StoragePropertyDefinition as ColumnDefinition;
      if (columnDefinition == null)
      {
        throw new MappingException (
            string.Format (
                "Cannot have non-RDBMS storage properties in an RDBMS mapping.\r\nDeclaring type: '{0}'\r\nProperty: '{1}'",
                propertyDefinition.PropertyInfo.DeclaringType.FullName,
                propertyDefinition.PropertyInfo.Name));
      }

      return columnDefinition;
    }

    private IEnumerable<ColumnDefinition> GetColumnDefinitionsForHierarchy (ClassDefinition classDefinition)
    {
      var allClassesInHierarchy = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Concat (classDefinition.GetAllDerivedClasses ().Cast<ClassDefinition> ());

      var columnDefinitions = from cd in allClassesInHierarchy
                              from PropertyDefinition pd in cd.MyPropertyDefinitions
                              where pd.StorageClass == StorageClass.Persistent
                              select GetColumnDefinition (pd);

      return columnDefinitions;
    }
  }
}