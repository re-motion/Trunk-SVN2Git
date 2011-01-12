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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="StorageNameCalculator"/> provides methods to obtain names for RDBMS items (tables, columns, ...) in a provider specific object.
  /// </summary>
  public class StorageNameCalculator : IStorageNameCalculator
  {
    public string GetTableName (ClassDefinition classDefinition)
    {
      var classDefinitionStorageEntityDefinitionAsTableDefiniton = classDefinition.StorageEntityDefinition as TableDefinition;
      if (classDefinitionStorageEntityDefinitionAsTableDefiniton == null)
      {
        //TODO 3607: throw exception !?
        return null;
      }

      return classDefinitionStorageEntityDefinitionAsTableDefiniton.TableName;
    }

    public string GetForeignKeyConstraintName (ClassDefinition classDefinition, IStoragePropertyDefinition storagePropertyDefinition)
    {
      var tableName = GetTableName (classDefinition);
      var propertyName = storagePropertyDefinition.Name;

      return string.Format ("FK_{0}_{1}", tableName, propertyName);
    }
  }
}