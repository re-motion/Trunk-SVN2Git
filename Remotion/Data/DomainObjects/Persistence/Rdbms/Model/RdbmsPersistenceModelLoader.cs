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
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Validation;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="RdbmsPersistenceModelLoader"/> is responsible to load a persistence model for a relational database.
  /// </summary>
  public class RdbmsPersistenceModelLoader : IPersistenceModelLoader
  {
    private readonly IColumnDefinitionFactory _columnDefinitionFactory;
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IEntityDefinitionFactory _entityDefinitionFactory;

    public RdbmsPersistenceModelLoader (
        IEntityDefinitionFactory entityDefinitionFactory,
        IColumnDefinitionFactory columnDefinitionFactory,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinitionFactory", entityDefinitionFactory);
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _entityDefinitionFactory = entityDefinitionFactory;
      _columnDefinitionFactory = columnDefinitionFactory;
      _storageProviderDefinition = storageProviderDefinition;
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public IEntityDefinitionFactory EntityDefinitionFactory
    {
      get { return _entityDefinitionFactory; }
    }

    public IColumnDefinitionFactory ColumnDefinitionFactory
    {
      get { return _columnDefinitionFactory; }
    }

    public IPersistenceMappingValidator CreatePersistenceMappingValidator (ClassDefinition classDefinition)
    {
      return new PersistenceMappingValidator (
          new OnlyOneTablePerHierarchyValidationRule(),
          new TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule(),
          new ClassAboveTableIsAbstractValidationRule(),
          new ColumnNamesAreUniqueWithinInheritanceTreeValidationRule(),
          new PropertyTypeIsSupportedByStorageProviderValidationRule());
    }

    public void ApplyPersistenceModelToHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      EnsureAllStoragePropertiesCreated (classDefinition);
      EnsureAllStorageEntitiesCreated (classDefinition);
    }

    private void EnsureAllStorageEntitiesCreated (ClassDefinition classDefinition)
    {
      EnsureStorageEntitiesCreated (classDefinition);

      foreach (ClassDefinition derivedClass in classDefinition.DerivedClasses)
        EnsureAllStorageEntitiesCreated (derivedClass);
    }

    private void EnsureStorageEntitiesCreated (ClassDefinition classDefinition)
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
        return _entityDefinitionFactory.CreateTableDefinition (classDefinition);

      var hasBaseClassWithDBTableAttribute = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Where (cd => AttributeUtility.IsDefined<DBTableAttribute> (cd.ClassType, false))
          .Any();
      if (hasBaseClassWithDBTableAttribute)
        return CreateFilterViewDefinition (classDefinition);

      return CreateUnionViewDefinition (classDefinition);
    }

    private void EnsureAllStoragePropertiesCreated (ClassDefinition classDefinition)
    {
      var allClassDefinitions = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Concat (classDefinition.GetAllDerivedClasses ().Cast<ClassDefinition> ());
      
      foreach (var cd in allClassDefinitions)
        EnsureStoragePropertiesCreated (cd);
    }

    private void EnsureStoragePropertiesCreated (ClassDefinition classDefinition)
    {
      foreach (var propertyDefinition in classDefinition.MyPropertyDefinitions.Where(pd=>pd.StorageClass==StorageClass.Persistent))
      {
        if (propertyDefinition.StoragePropertyDefinition == null)
        {
          var storagePropertyDefinition = CreateStoragePropertyDefinition (propertyDefinition);
          propertyDefinition.SetStorageProperty (storagePropertyDefinition);
        }

        Assertion.IsNotNull (propertyDefinition.StoragePropertyDefinition);
      }
    }

    private IStoragePropertyDefinition CreateStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      Assertion.IsTrue (propertyDefinition.StorageClass == StorageClass.Persistent);

      return _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);
    }

    private IStorageEntityDefinition CreateFilterViewDefinition (ClassDefinition classDefinition)
    {
      // The following call is potentially recursive (GetEntityDefinition -> EnsureStorageEntitiesCreated -> CreateFilterViewDefinition), but this is
      // guaranteed to terminate because we know at this point that there is a class in the classDefinition's base hierarchy that will get a 
      // TableDefinition
      var baseStorageEntityDefinition = GetEntityDefinition (classDefinition.BaseClass);

      return _entityDefinitionFactory.CreateFilterViewDefinition (classDefinition, baseStorageEntityDefinition);
    }

    private IStorageEntityDefinition CreateUnionViewDefinition (ClassDefinition classDefinition)
    {
      var derivedStorageEntityDefinitions =
          from ClassDefinition derivedClass in classDefinition.DerivedClasses
          let entityDefinition = GetEntityDefinition (derivedClass)
          where !(entityDefinition.IsNull)
          select entityDefinition;

      if (!derivedStorageEntityDefinitions.Any ())
        return new NullEntityDefinition (_storageProviderDefinition);

      return _entityDefinitionFactory.CreateUnionViewDefinition (classDefinition, derivedStorageEntityDefinitions);
    }

    private IEntityDefinition GetEntityDefinition (ClassDefinition classDefinition)
    {
      EnsureStorageEntitiesCreated (classDefinition);

      return (IEntityDefinition) classDefinition.StorageEntityDefinition;
    }
    
  }
}