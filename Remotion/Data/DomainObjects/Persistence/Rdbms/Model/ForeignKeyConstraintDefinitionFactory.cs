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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="ForeignKeyConstraintDefinitionFactory"/> is responsible to create all <see cref="ForeignKeyConstraintDefinition"/>s for a 
  /// <see cref="ClassDefinition"/>.
  /// </summary>
  public class ForeignKeyConstraintDefinitionFactory : IForeignKeyConstraintDefinitionFactory
  {
    private readonly IColumnDefinitionResolver _columnDefinitionResolver;
    private readonly IStorageNameCalculator _storageNameCalculator;
    private readonly IColumnDefinitionFactory _columnDefinitionFactory;

    public ForeignKeyConstraintDefinitionFactory (
        IStorageNameCalculator storageNameCalculator,
        IColumnDefinitionResolver columnDefinitionResolver,
        IColumnDefinitionFactory columnDefinitionFactory)
    {
      ArgumentUtility.CheckNotNull ("storageNameCalculator", storageNameCalculator);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);

      _storageNameCalculator = storageNameCalculator;
      _columnDefinitionResolver = columnDefinitionResolver;
      _columnDefinitionFactory = columnDefinitionFactory;
    }

    public IEnumerable<ForeignKeyConstraintDefinition> CreateForeignKeyConstraints (ClassDefinition classDefinition)
    {
      var foreignKeyConstraintDefinitions = new List<ForeignKeyConstraintDefinition>();
      var allRelationEndPointDefinitions = classDefinition.GetRelationEndPointDefinitions().Concat (
          classDefinition.GetAllDerivedClasses().Cast<ClassDefinition>().SelectMany (cd => cd.MyRelationEndPointDefinitions));

      foreach (var endPoint in allRelationEndPointDefinitions)
      {
        if (endPoint.IsAnonymous)
          continue;

        var oppositeClassDefinition = endPoint.ClassDefinition.GetMandatoryOppositeClassDefinition (endPoint.PropertyName);
        if (!HasConstraint (endPoint, oppositeClassDefinition))
          continue;

        var propertyDefinition = endPoint.ClassDefinition.GetMandatoryPropertyDefinition (endPoint.PropertyName);
        if (propertyDefinition.StorageClass != StorageClass.Persistent)
          continue;

        var virtualConstraintColumnDefinitons = _columnDefinitionFactory.CreateIDColumnDefinition();

        //TODO 3626: change to as case and throw exception if it's null!
        var nonVirtualConstraintColumnDefinition = (IDColumnDefinition) _columnDefinitionResolver.GetColumnDefinition (propertyDefinition);

        var referencingColumns =
            SqlColumnDefinitionFindingVisitor.FindSimpleColumnDefinitions (new[] { virtualConstraintColumnDefinitons.ObjectIDColumn });
        var referencedColumns =
            SqlColumnDefinitionFindingVisitor.FindSimpleColumnDefinitions (new[] { nonVirtualConstraintColumnDefinition.ObjectIDColumn });

        var foreignKeyConstraintDefinition = new ForeignKeyConstraintDefinition (
            _storageNameCalculator.GetForeignKeyConstraintName (classDefinition, nonVirtualConstraintColumnDefinition),
            _storageNameCalculator.GetTableName (oppositeClassDefinition),
            referencingColumns,
            referencedColumns);
        foreignKeyConstraintDefinitions.Add (foreignKeyConstraintDefinition);
      }

      return foreignKeyConstraintDefinitions;
    }

    private bool HasConstraint (IRelationEndPointDefinition endPoint, ClassDefinition oppositeClassDefinition)
    {
      if (endPoint.IsVirtual)
        return false;

      if (oppositeClassDefinition.StorageEntityDefinition.StorageProviderDefinition.Name
          != endPoint.ClassDefinition.StorageEntityDefinition.StorageProviderDefinition.Name)
        return false;

      //TODO 3626: change to !(StorageEntityDefinition is TableDefinition) !?
      if (oppositeClassDefinition.GetEntityName() == null)
        return false;

      return true;
    }
  }
}