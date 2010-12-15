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
    private readonly IStoragePropertyDefinitionFactory _storagePropertyDefinitionFactory;
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    public RdbmsPersistenceModelLoader (
        IStoragePropertyDefinitionFactory storagePropertyDefinitionFactory,
        StorageProviderDefinition storageProviderDefinition,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storagePropertyDefinitionFactory", storagePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      _storagePropertyDefinitionFactory = storagePropertyDefinitionFactory;
      _storageProviderDefinition = storageProviderDefinition;
      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public IStoragePropertyDefinitionFactory StoragePropertyDefinitionFactory
    {
      get { return _storagePropertyDefinitionFactory; }
    }

    public IPersistenceMappingValidator CreatePersistenceMappingValidator (ClassDefinition classDefinition)
    {
      return new PersistenceMappingValidator (
          new OnlyOneTablePerHierarchyValidationRule (),
          new TableNamesAreDistinctWithinConcreteTableInheritanceHierarchyValidationRule (),
          new ClassAboveTableIsAbstractValidationRule (),
          new ColumnNamesAreUniqueWithinInheritanceTreeValidationRule (),
          new PropertyTypeIsSupportedByStorageProviderValidationRule ());
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

      return new TableDefinition (
          _storageProviderDefinition, tableName, GetViewName (classDefinition), GetColumnDefinitionsForHierarchy (classDefinition));
    }

    private IStorageEntityDefinition CreateFilterViewDefinition (ClassDefinition classDefinition)
    {
      // The following call is potentially recursive (GetEntityDefinition -> EnsurePersistenceModelApplied -> CreateFilterViewDefinition), but this is
      // guaranteed to terminate because we know at this point that there is a class in the classDefinition's base hierarchy that will get a 
      // TableDefinition
      var baseStorageEntityDefinition = GetEntityDefinition (classDefinition.BaseClass);

      var actualAndBaseClassColumns = new HashSet<IColumnDefinition> (GetColumnDefinitionsForHierarchy (classDefinition));

      return new FilterViewDefinition (
          _storageProviderDefinition,
          GetViewName (classDefinition),
          baseStorageEntityDefinition,
          classDefinition.ID,
          actualAndBaseClassColumns.Contains);
    }

    private IStorageEntityDefinition CreateUnionViewDefinition (ClassDefinition classDefinition)
    {
      var derivedStorageEntityDefinitions =
          from ClassDefinition derivedClass in classDefinition.DerivedClasses
          select GetEntityDefinition (derivedClass);

      if(!derivedStorageEntityDefinitions.Any())
       GetColumnDefinitionsForHierarchy (classDefinition).ToArray();

      return new UnionViewDefinition (_storageProviderDefinition, GetViewName (classDefinition), derivedStorageEntityDefinitions);
    }

    private string GetViewName (ClassDefinition classDefinition)
    {
      return classDefinition.ID + "View";
    }

    private IEntityDefinition GetEntityDefinition (ClassDefinition classDefinition)
    {
      EnsurePersistenceModelApplied (classDefinition);

      return (IEntityDefinition) classDefinition.StorageEntityDefinition;
    }

    private IColumnDefinition GetColumnDefinition (PropertyDefinition propertyDefinition)
    {
      Assertion.IsTrue (propertyDefinition.StorageClass == StorageClass.Persistent);

      if (propertyDefinition.StoragePropertyDefinition == null)
      {
        var storageProperty = _storagePropertyDefinitionFactory.CreateStoragePropertyDefinition (propertyDefinition, _storageProviderDefinitionFinder);
        propertyDefinition.SetStorageProperty (storageProperty);
      }

      var columnDefinition = propertyDefinition.StoragePropertyDefinition as IColumnDefinition;
      if (columnDefinition==null)
      {
        throw new MappingException (
            string.Format (
                "Cannot have non-RDBMS storage properties in an RDBMS mapping.\r\nDeclaring type: '{0}'\r\nProperty: '{1}'",
                propertyDefinition.PropertyInfo.DeclaringType.FullName,
                propertyDefinition.PropertyInfo.Name));
      }

      return columnDefinition;
    }

    private IEnumerable<IColumnDefinition> GetColumnDefinitionsForHierarchy (ClassDefinition classDefinition)
    {
      var allClassesInHierarchy = classDefinition
        .CreateSequence (cd => cd.BaseClass)
        .Reverse()
        .Concat (classDefinition.GetAllDerivedClasses().Cast<ClassDefinition>());

      var columnDefinitions = from cd in allClassesInHierarchy
                              from PropertyDefinition pd in cd.MyPropertyDefinitions
                              where pd.StorageClass == StorageClass.Persistent
                              select GetColumnDefinition (pd);

      return columnDefinitions;
    }
  }
}