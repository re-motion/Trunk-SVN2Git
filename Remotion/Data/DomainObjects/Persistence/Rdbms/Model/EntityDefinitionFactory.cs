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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="EntityDefinitionFactory"/> provides factory methods to create <see cref="IEntityDefinition"/>s.
  /// </summary>
  public class EntityDefinitionFactory : IEntityDefinitionFactory
  {
    private readonly IColumnDefinitionFactory _columnDefinitionFactory;
    private readonly StorageProviderDefinition _storageProviderDefinition;
    private readonly IColumnDefinitionResolver _columnDefinitionResolver;
    private readonly IForeignKeyConstraintDefinitionFactory _foreignKeyConstraintDefinitionFactory;

    public EntityDefinitionFactory (
        IColumnDefinitionFactory columnDefinitionFactory,
        IForeignKeyConstraintDefinitionFactory foreignKeyConstraintDefinitionFactory,
        IColumnDefinitionResolver columnDefinitionResolver,
        StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);
      ArgumentUtility.CheckNotNull ("foreignKeyConstraintDefinitionFactory", foreignKeyConstraintDefinitionFactory);
      ArgumentUtility.CheckNotNull ("columnDefinitionResolver", columnDefinitionResolver);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _columnDefinitionFactory = columnDefinitionFactory;
      _foreignKeyConstraintDefinitionFactory = foreignKeyConstraintDefinitionFactory;
      _columnDefinitionResolver = columnDefinitionResolver;
      _storageProviderDefinition = storageProviderDefinition;
    }

    public virtual IEntityDefinition CreateTableDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableName = GetTableName (classDefinition);
      var columns = GetColumnsDefinitionForEntity (classDefinition);

      var clusteredPrimaryKeyConstraint = new PrimaryKeyConstraintDefinition (
          GetPrimaryKeyName (tableName),
          true,
          SqlColumnDefinitionFindingVisitor.FindSimpleColumnDefinitions (columns).Where (c => c.IsPartOfPrimaryKey).ToArray());

      var foreignKeyConstraints =
          _foreignKeyConstraintDefinitionFactory.CreateForeignKeyConstraints (classDefinition).Cast<ITableConstraintDefinition>();

      return new TableDefinition (
          _storageProviderDefinition,
          tableName,
          GetViewName (classDefinition),
          columns,
          new ITableConstraintDefinition[] { clusteredPrimaryKeyConstraint }.Concat (foreignKeyConstraints));
    }

    public virtual IEntityDefinition CreateFilterViewDefinition (ClassDefinition classDefinition, IEntityDefinition baseEntity)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);

      var columns = GetColumnsDefinitionForEntity (classDefinition);

      return new FilterViewDefinition (
          _storageProviderDefinition,
          GetViewName (classDefinition),
          baseEntity,
          GetClassIDsForBranch (classDefinition),
          columns);
    }

    public virtual IEntityDefinition CreateUnionViewDefinition (ClassDefinition classDefinition, IEnumerable<IEntityDefinition> unionedEntities)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("unionedEntities", unionedEntities);

      var columns = GetColumnsDefinitionForEntity (classDefinition);

      return new UnionViewDefinition (_storageProviderDefinition, GetViewName (classDefinition), unionedEntities, columns);
    }

    protected virtual string GetTableName (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableAttribute = AttributeUtility.GetCustomAttribute<DBTableAttribute> (classDefinition.ClassType, false);
      if (tableAttribute == null)
      {
        throw new MappingException (
            string.Format ("Class '{0}' has no '{1}' defined.", classDefinition.ID, typeof (DBTableAttribute).Name));
      }

      return string.IsNullOrEmpty (tableAttribute.Name) ? classDefinition.ID : tableAttribute.Name;
    }

    protected virtual string GetViewName (ClassDefinition classDefinition)
    {
      return classDefinition.ID + "View";
    }

    protected virtual string GetPrimaryKeyName (string tableName)
    {
      return string.Format ("PK_{0}", tableName);
    }

    protected IEnumerable<string> GetClassIDsForBranch (ClassDefinition classDefinition)
    {
      return new[] { classDefinition }.Concat (classDefinition.GetAllDerivedClasses().Cast<ClassDefinition>()).Select (cd => cd.ID);
    }

    protected virtual IEnumerable<IColumnDefinition> GetColumnsDefinitionForEntity (ClassDefinition classDefinition)
    {
      var idColumnDefinition = _columnDefinitionFactory.CreateIDColumnDefinition();
      var timestampColumnDefinition = _columnDefinitionFactory.CreateTimestampColumnDefinition();
      var columnDefinitionsForHierarchy = _columnDefinitionResolver.GetColumnDefinitionsForHierarchy (classDefinition);

      return new IColumnDefinition[] { idColumnDefinition, timestampColumnDefinition }.Concat (columnDefinitionsForHierarchy).ToList();
    }
  }
}