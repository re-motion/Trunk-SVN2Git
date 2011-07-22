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
    private readonly IColumnDefinitionResolver _columnDefinitionResolver;
    private readonly IStorageNameProvider _storageNameProvider;
    private readonly IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefinitionProvider;
    private readonly IStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    public ForeignKeyConstraintDefinitionFactory (
        IStorageNameProvider storageNameProvider,
        IColumnDefinitionResolver columnDefinitionResolver,
        IInfrastructureStoragePropertyDefinitionProvider infrastructureStoragePropertyDefinitionProvider,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("infrastructureStoragePropertyDefinitionProvider", infrastructureStoragePropertyDefinitionProvider);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      _storageNameProvider = storageNameProvider;
      _columnDefinitionResolver = columnDefinitionResolver;
      _infrastructureStoragePropertyDefinitionProvider = infrastructureStoragePropertyDefinitionProvider;
      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
    }

    public IEnumerable<ForeignKeyConstraintDefinition> CreateForeignKeyConstraints (ClassDefinition classDefinition)
    {
      var foreignKeyConstraintDefinitions = new List<ForeignKeyConstraintDefinition>();

      var allClassDefinitions = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Concat (classDefinition.GetAllDerivedClasses ());
      var allRelationEndPointDefinitions = allClassDefinitions.SelectMany (cd => cd.MyRelationEndPointDefinitions);

      foreach (var endPoint in allRelationEndPointDefinitions)
      {
        if (endPoint.IsVirtual)
          continue;

        var oppositeClassDefinition = endPoint.ClassDefinition.GetMandatoryOppositeClassDefinition (endPoint.PropertyName);

        // Foreign keys can only be declared within the same storage provider
        if (GetStorageProviderDefinition (oppositeClassDefinition) != GetStorageProviderDefinition (endPoint.ClassDefinition))
          continue;

        // No foreign keys for non-persistent properties.
        var propertyDefinition = ((RelationEndPointDefinition) endPoint).PropertyDefinition;
        if (propertyDefinition.StorageClass != StorageClass.Persistent)
          continue;

        // Foreign keys can only be declared if we have an opposite table (not if we have an opposite union view, for example)
        if (FindTableName (oppositeClassDefinition) == null)
          continue;

        // We can't access the opposite ID column from here, but columns implement equality, so we can just recreate it
        var oppositeObjectIDColumnDefinition = _infrastructureStoragePropertyDefinitionProvider.GetIDColumnDefinition();
        var endPointColumnDefinition = _columnDefinitionResolver.GetColumnDefinition (propertyDefinition);
        var referencingColumn = oppositeObjectIDColumnDefinition;
        var referencedColumn = endPointColumnDefinition.GetColumnForForeignKey ();

        var foreignKeyConstraintDefinition = new ForeignKeyConstraintDefinition (
            _storageNameProvider.GetForeignKeyConstraintName (classDefinition, endPointColumnDefinition),
            new EntityNameDefinition(null, FindTableName(oppositeClassDefinition)),
            new[] { referencingColumn },
            new[] { referencedColumn });
        foreignKeyConstraintDefinitions.Add (foreignKeyConstraintDefinition);
      }

      return foreignKeyConstraintDefinitions;
    }

    private StorageProviderDefinition GetStorageProviderDefinition (ClassDefinition oppositeClassDefinition)
    {
      return _storageProviderDefinitionFinder.GetStorageProviderDefinition (oppositeClassDefinition.StorageGroupType, null);
    }

    private string FindTableName (ClassDefinition classDefinition)
    {
      return classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Select (cd => _storageNameProvider.GetTableName (cd))
          .FirstOrDefault (name => name != null);
    }
  }
}