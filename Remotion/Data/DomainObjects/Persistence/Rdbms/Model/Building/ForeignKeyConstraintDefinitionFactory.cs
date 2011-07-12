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
    private readonly IRdbmsStoragePropertyDefinitionFactory _rdbmsStoragePropertyDefinitionFactory;
    private readonly IStorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    public ForeignKeyConstraintDefinitionFactory (
        IStorageNameProvider storageNameProvider,
        IColumnDefinitionResolver columnDefinitionResolver,
        IRdbmsStoragePropertyDefinitionFactory rdbmsStoragePropertyDefinitionFactory,
        IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
    {
      ArgumentUtility.CheckNotNull ("storageNameProvider", storageNameProvider);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("rdbmsStoragePropertyDefinitionFactory", rdbmsStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storageProviderDefinitionFinder", storageProviderDefinitionFinder);

      _storageNameProvider = storageNameProvider;
      _columnDefinitionResolver = columnDefinitionResolver;
      _rdbmsStoragePropertyDefinitionFactory = rdbmsStoragePropertyDefinitionFactory;
      _storageProviderDefinitionFinder = storageProviderDefinitionFinder;
    }

    public IEnumerable<ForeignKeyConstraintDefinition> CreateForeignKeyConstraints (ClassDefinition classDefinition)
    {
      var foreignKeyConstraintDefinitions = new List<ForeignKeyConstraintDefinition>();

      var allClassDefinitions = classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Concat (classDefinition.GetAllDerivedClasses ().Cast<ClassDefinition>());
      var allRelationEndPointDefinitions = allClassDefinitions.SelectMany (cd => cd.MyRelationEndPointDefinitions);

      foreach (var endPoint in allRelationEndPointDefinitions)
      {
        var oppositeClassDefinition = endPoint.ClassDefinition.GetMandatoryOppositeClassDefinition (endPoint.PropertyName);

        if (endPoint.IsVirtual)
          continue;

        if (!HasConstraint (endPoint, oppositeClassDefinition))
          continue;

        var propertyDefinition = ((RelationEndPointDefinition) endPoint).PropertyDefinition;
        if (propertyDefinition.StorageClass != StorageClass.Persistent)
          continue;

        var oppositeObjectIDColumnDefinition = _rdbmsStoragePropertyDefinitionFactory.CreateObjectIDColumnDefinition();

        var endPointColumnDefinition = _columnDefinitionResolver.GetColumnDefinition (propertyDefinition);
        var endPointIDColumnDefinition = endPointColumnDefinition as IObjectIDStoragePropertyDefinition;
        if (endPointIDColumnDefinition == null)
          throw new InvalidOperationException ("The non virtual constraint column definition has to be an ID column definition.");

        var referencingColumn = oppositeObjectIDColumnDefinition;
        var referencedColumn = endPointIDColumnDefinition.GetColumnForLookup();

        var foreignKeyConstraintDefinition = new ForeignKeyConstraintDefinition (
            _storageNameProvider.GetForeignKeyConstraintName (classDefinition, endPointColumnDefinition),
            new EntityNameDefinition(null, FindTableName(oppositeClassDefinition)),
            new[] { referencingColumn },
            new[] { referencedColumn });
        foreignKeyConstraintDefinitions.Add (foreignKeyConstraintDefinition);
      }

      return foreignKeyConstraintDefinitions;
    }

    private bool HasConstraint (IRelationEndPointDefinition endPoint, ClassDefinition oppositeClassDefinition)
    {
      if (GetStorageProviderDefinition (oppositeClassDefinition).Name != GetStorageProviderDefinition (endPoint.ClassDefinition).Name)
        return false;

      if (FindTableName (oppositeClassDefinition) == null)
        return false;

      return true;
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