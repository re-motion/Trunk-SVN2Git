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
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
    /// <summary>
    /// The <see cref="SqlSynonymScriptElementFactory"/> is responsible to create script-elements for synonyms in a sql-server database.
    /// </summary>
  public class SqlSynonymScriptElementFactory :
    IScriptElementFactory<Tuple<TableDefinition, EntityNameDefinition>>,
    IScriptElementFactory<Tuple<UnionViewDefinition, EntityNameDefinition>>,
    IScriptElementFactory<Tuple<FilterViewDefinition, EntityNameDefinition>>
  {
    public IScriptElement GetCreateElement (Tuple<TableDefinition, EntityNameDefinition> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return GetSynonymCreateScriptStatement (item.Item1.TableName, item.Item2);
    }

    public IScriptElement GetDropElement (Tuple<TableDefinition, EntityNameDefinition> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return GetSynonymDropScriptStatement (item.Item2);
    }

    public IScriptElement GetCreateElement (Tuple<UnionViewDefinition, EntityNameDefinition> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return GetSynonymCreateScriptStatement (item.Item1.ViewName, item.Item2);
    }

    public IScriptElement GetDropElement (Tuple<UnionViewDefinition, EntityNameDefinition> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return GetSynonymDropScriptStatement (item.Item2);
    }

    public IScriptElement GetCreateElement (Tuple<FilterViewDefinition, EntityNameDefinition> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return GetSynonymCreateScriptStatement (item.Item1.ViewName, item.Item2);
    }

    public IScriptElement GetDropElement (Tuple<FilterViewDefinition, EntityNameDefinition> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return GetSynonymDropScriptStatement (item.Item2);
    }

    private ScriptStatement GetSynonymCreateScriptStatement (EntityNameDefinition referencedEntityName, EntityNameDefinition synonymName)
    {
      return new ScriptStatement (
        string.Format (
            "CREATE SYNONYM [{0}].[{1}] FOR [{2}].[{3}]",
            synonymName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
            synonymName.EntityName,
            referencedEntityName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
            referencedEntityName.EntityName));
    }

    private ScriptStatement GetSynonymDropScriptStatement (EntityNameDefinition synonymName)
    {
      return new ScriptStatement (
        string.Format (
           "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = '{0}' AND SCHEMA_NAME(schema_id) = '{1}')\r\n"
           + "  DROP SYNONYM [{0}].[{1}]",
         synonymName.SchemaName ?? CompositeScriptBuilder.DefaultSchema,
         synonymName.EntityName));
    }
  }
}