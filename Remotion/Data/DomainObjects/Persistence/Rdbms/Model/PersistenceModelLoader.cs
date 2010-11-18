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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
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

    public PersistenceModelLoader (IStoragePropertyDefinitionFactory storagePropertyDefinitionFactory)
    {
      ArgumentUtility.CheckNotNull ("storagePropertyDefinitionFactory", storagePropertyDefinitionFactory);

      _storagePropertyDefinitionFactory = storagePropertyDefinitionFactory;
    }

    public void ApplyPersistenceModelToHierarchy (ClassDefinition classDefinition)
    {
      EnsurePersistenceModelApplied(classDefinition);

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        ApplyPersistenceModelToHierarchy (derivedClass);
    }

    private void EnsurePersistenceModelApplied (ClassDefinition classDefinition)
    {
      if (classDefinition.StorageEntityDefinition == null)
      {
        foreach (PropertyDefinition propertyDefinition in classDefinition.MyPropertyDefinitions)
        {
          var storageProperty = _storagePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition);
          propertyDefinition.SetStorageProperty (storageProperty);
        }

        var storageEntity = CreateStorageEntityDefinition (classDefinition);
        classDefinition.SetStorageEntity (storageEntity);
      }

      Assertion.IsNotNull (classDefinition.StorageEntityDefinition);
    }

    private IStorageEntityDefinition CreateStorageEntityDefinition (ClassDefinition classDefinition)
    {
      var tableAttribute = AttributeUtility.GetCustomAttribute<DBTableAttribute> (classDefinition.ClassType, false);
      if (tableAttribute != null)
      {
        return new TableDefinition (
            GetTableName(classDefinition.ClassType),
            classDefinition.MyPropertyDefinitions.Cast<PropertyDefinition>().Select (pd => pd.StoragePropertyDefinition).Cast<ColumnDefinition>());
      }

      var baseClassWithDBTableAttributeDefined =
          classDefinition.CreateSequence (cd => cd.BaseClass).Where (cd => AttributeUtility.IsDefined<DBTableAttribute> (cd.ClassType, false)).
              FirstOrDefault();
      if (baseClassWithDBTableAttributeDefined != null)
      {
        EnsurePersistenceModelApplied (baseClassWithDBTableAttributeDefined);
        var baseStorageEntityDefinition = baseClassWithDBTableAttributeDefined.StorageEntityDefinition;
        
        var actualAndBaseClassColumns = new HashSet<ColumnDefinition> (
            classDefinition.GetPropertyDefinitions().Cast<PropertyDefinition>().Select (pd => (ColumnDefinition) pd.StoragePropertyDefinition));

        return new FilterViewDefinition (
            classDefinition.ID + "View", (IEntityDefinition) baseStorageEntityDefinition, classDefinition.ID, actualAndBaseClassColumns.Contains);
      }

      var derivedStorageEntityDefinitions = new List<IEntityDefinition>();
      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
      {
        EnsurePersistenceModelApplied (derivedClass);
        var derivedStorageEntityDefinition = derivedClass.StorageEntityDefinition;
        derivedStorageEntityDefinitions.Add ((IEntityDefinition) derivedStorageEntityDefinition);
      }

      return new UnionViewDefinition (classDefinition.ID+"View", derivedStorageEntityDefinitions);
    }

    //TODO: move this two methods to another class !?
    //TODO: alse remove GetStorageSpecificIdentifier and GetID methods from ClassReflector ??

    private string GetTableName (Type type)
    {
      var attribute = AttributeUtility.GetCustomAttribute<IStorageSpecificIdentifierAttribute> (type, false);
      if (attribute != null && !string.IsNullOrEmpty (attribute.Identifier))
        return attribute.Identifier;
      return GetID (type);
    }

    private string GetID (Type type)
    {
      var attribute = AttributeUtility.GetCustomAttribute<ClassIDAttribute> (type, false);
      if (attribute != null)
        return attribute.ClassID;
      return type.Name;
    }
  }
}