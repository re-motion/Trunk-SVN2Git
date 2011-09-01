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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// The <see cref="ForeignKeyConstraintDefinitionFactory"/> is responsible to create all <see cref="ForeignKeyConstraintDefinition"/>s for a 
  /// <see cref="ClassDefinition"/>.
  /// </summary>
  public class ForeignKeyConstraintDefinitionFactory : IForeignKeyConstraintDefinitionFactory
  {
    private readonly IRdbmsPersistenceModelProvider _persistenceModelProvider;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private readonly IStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    public ForeignKeyConstraintDefinitionFactory (
        IStorageNameProvider storageNameProvider,
        IRdbmsPersistenceModelProvider persistenceModelProvider,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("persistenceModelProvider", persistenceModelProvider);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      _storageNameProvider = storageNameProvider;
      _persistenceModelProvider = persistenceModelProvider;
      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
    }

    public IEnumerable<ForeignKeyConstraintDefinition> CreateForeignKeyConstraints (ClassDefinition classDefinition)
    {
      var allClassDefinitionsInHierarchy = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Concat (classDefinition.GetAllDerivedClasses ());

      return (from classDefinitionInHierarchy in allClassDefinitionsInHierarchy
              from endPointDefinition in classDefinitionInHierarchy.MyRelationEndPointDefinitions
              where !endPointDefinition.IsVirtual
              let oppositeClassDefinition = endPointDefinition.ClassDefinition.GetMandatoryOppositeClassDefinition (endPointDefinition.PropertyName)
              where GetStorageProviderDefinition (oppositeClassDefinition) == GetStorageProviderDefinition (endPointDefinition.ClassDefinition)
              let propertyDefinition = ((RelationEndPointDefinition) endPointDefinition).PropertyDefinition
              where propertyDefinition.StorageClass == StorageClass.Persistent
              where FindTableName (oppositeClassDefinition) != null
              let oppositeStoragePropertyDefinition = _infrastructureStoragePropertyDefinitionProvider.GetObjectIDStoragePropertyDefinition()
              let endPointStorageProperty = _persistenceModelProvider.GetStoragePropertyDefinition (propertyDefinition)
              let referencingColumns = new[] { oppositeStoragePropertyDefinition.GetColumnForLookup() }
              let referencedColumns = new[] { endPointStorageProperty.GetColumnForLookup() }
              select new ForeignKeyConstraintDefinition (
                  _storageNameProvider.GetForeignKeyConstraintName (classDefinition, referencedColumns), 
                  new EntityNameDefinition (null, FindTableName (oppositeClassDefinition)), 
                  referencingColumns, 
                  referencedColumns)).ToList();
    }

    private StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition oppositeClassDefinition)
    {
      return _storageProviderDefinitionFinder.GetStorageProviderDefinition (oppositeClassDefinition.StorageGroupType, null);
    }

    private string FindTableName (ClassDefinition classDefinition)
    {
      var tableName = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Select (cd => _storageNameProvider.GetTableName (cd))
          .FirstOrDefault (name => name != null);
      return tableName != null ? tableName.EntityName : null;
    }
  }
}