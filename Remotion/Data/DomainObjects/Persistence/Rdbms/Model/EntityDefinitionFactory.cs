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
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.FunctionalProgramming;
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

    public EntityDefinitionFactory (IColumnDefinitionFactory columnDefinitionFactory, StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("columnDefinitionFactory", columnDefinitionFactory);
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _columnDefinitionFactory = columnDefinitionFactory;
      _storageProviderDefinition = storageProviderDefinition;
    }

    public virtual IEntityDefinition CreateTableDefinition (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var tableName = GetTableName (classDefinition);
      var columns = GetColumnDefinitionsForHierarchy (classDefinition).ToArray();
      var clusteredPrimaryKeyConstraint = new PrimaryKeyConstraintDefinition (GetPrimaryKeyName(tableName), true, new[] { columns[0] });
      
      return new TableDefinition (
          _storageProviderDefinition,
          tableName,
          GetViewName (classDefinition),
          columns,
          new[] { clusteredPrimaryKeyConstraint });
    }

    public virtual IEntityDefinition CreateFilterViewDefinition (ClassDefinition classDefinition, IEntityDefinition baseEntity)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("baseEntity", baseEntity);

      return new FilterViewDefinition (
          _storageProviderDefinition,
          GetViewName (classDefinition),
          baseEntity,
          GetClassIDsForBranch (classDefinition),
          GetColumnDefinitionsForHierarchy (classDefinition));
    }

    public virtual IEntityDefinition CreateUnionViewDefinition (ClassDefinition classDefinition, IEnumerable<IEntityDefinition> unionedEntities)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("unionedEntities", unionedEntities);

      IColumnDefinition[] columns = GetColumnDefinitionsForHierarchy (classDefinition).ToArray();

      // TODO Review 3606: Move this to RdbmsPersistenceModelLoader (after 3629; include test)
      if (!unionedEntities.Any())
        return new NullEntityDefinition (_storageProviderDefinition);

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
      return new[] { classDefinition }.Concat (classDefinition.GetAllDerivedClasses ().Cast<ClassDefinition> ()).Select (cd => cd.ID);
    }

    protected virtual IColumnDefinition GetColumnDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      
      Assertion.IsTrue (propertyDefinition.StorageClass == StorageClass.Persistent); //TODO 3620: throw exception

      if (propertyDefinition.StoragePropertyDefinition == null)
      {

        var storageProperty = _columnDefinitionFactory.CreateColumnDefinition (propertyDefinition);
        propertyDefinition.SetStorageProperty (storageProperty);
      }

      var columnDefinition = propertyDefinition.StoragePropertyDefinition as IColumnDefinition;
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

    protected virtual IEnumerable<IColumnDefinition> GetColumnDefinitionsForHierarchy (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      var allClassesInHierarchy = GetAllClassesForHierarchy (classDefinition);

      var equalityComparer = new DelegateBasedEqualityComparer<Tuple<PropertyInfo, IColumnDefinition>> (
          (tuple1, tuple2) => tuple1.Item1 == tuple2.Item1,
          tuple => tuple.Item1.GetHashCode());

      var columnDefinitions =
          (from cd in allClassesInHierarchy
           from PropertyDefinition pd in cd.MyPropertyDefinitions
           where pd.StorageClass == StorageClass.Persistent
           select Tuple.Create (pd.PropertyInfo, GetColumnDefinition (pd)))
              .Distinct (equalityComparer)
              .Select (tuple => tuple.Item2);

      return new IColumnDefinition[]
             { _columnDefinitionFactory.CreateIDColumnDefinition(), _columnDefinitionFactory.CreateTimestampColumnDefinition() }
          .Concat (columnDefinitions);
    }

    private IEnumerable<ClassDefinition> GetAllClassesForHierarchy (ClassDefinition classDefinition)
    {
      return classDefinition
          .CreateSequence (cd => cd.BaseClass)
          .Reverse()
          .Concat (classDefinition.GetAllDerivedClasses().Cast<ClassDefinition>());
    }
  }
}