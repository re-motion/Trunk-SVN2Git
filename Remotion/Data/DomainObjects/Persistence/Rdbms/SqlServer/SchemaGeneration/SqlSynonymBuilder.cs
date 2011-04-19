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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// Generates SQL Server-specific synonyms for the persistence model.
  /// </summary>
  public class SqlSynonymBuilder : SynonymBuilderBase
  {
    public SqlSynonymBuilder ()
    {
    }

    public override void AddToCreateSynonymScript (TableDefinition tableDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      AddToCreateSynonymScript (tableDefinition.Synonyms, tableDefinition.TableName, createTableStringBuilder);
    }

    public override void AddToCreateSynonymScript (FilterViewDefinition filterViewDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("filterViewDefinition", filterViewDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      AddToCreateSynonymScript (filterViewDefinition.Synonyms, filterViewDefinition.ViewName, createTableStringBuilder);
    }

    public override void AddToCreateSynonymScript (UnionViewDefinition unionViewDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("unionViewDefinition", unionViewDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      AddToCreateSynonymScript (unionViewDefinition.Synonyms, unionViewDefinition.ViewName, createTableStringBuilder);
    }

    private void AddToCreateSynonymScript (
        IEnumerable<EntityNameDefinition> synonyms, 
        EntityNameDefinition referencedEntityName, 
        StringBuilder createTableStringBuilder)
    {
      foreach (var synonym in synonyms)
      {
        if (createTableStringBuilder.Length != 0)
          createTableStringBuilder.Append ("\r\n");

        createTableStringBuilder.AppendFormat (
            "CREATE SYNONYM [{0}].[{1}] FOR [{2}].[{3}]\r\n",
            synonym.SchemaName ?? SqlScriptBuilder.DefaultSchema,
            synonym.EntityName,
            referencedEntityName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
            referencedEntityName.EntityName);
      }
    }

    public override void AddToDropSynonymScript (EntityDefinitionBase entityDefinition, StringBuilder dropTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("dropTableStringBuilder", dropTableStringBuilder);

      foreach (var synonym in entityDefinition.Synonyms)
      {
        if (dropTableStringBuilder.Length != 0)
          dropTableStringBuilder.Append ("\r\n");

        dropTableStringBuilder.AppendFormat (
           "IF EXISTS (SELECT * FROM sys.synonyms WHERE name = '{0}' AND SCHEMA_NAME(schema_id) = '{1}')\r\n"
           +"  DROP SYNONYM [{0}].[{1}]\r\n",
         synonym.SchemaName ?? SqlScriptBuilder.DefaultSchema,
         synonym.EntityName);
      }
    }
  }
}