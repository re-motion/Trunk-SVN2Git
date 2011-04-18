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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class SqlSynonymBuilder : SynonymBuilderBase
  {
    public SqlSynonymBuilder ()
        : base(SqlDialect.Instance)
    {
    }

    public override void AddToCreateSynonymScript (EntityDefinitionBase entityDefinition, StringBuilder createTableStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);
      ArgumentUtility.CheckNotNull ("createTableStringBuilder", createTableStringBuilder);

      foreach (var synonym in entityDefinition.Synonyms)
      {
        if (createTableStringBuilder.Length != 0)
          createTableStringBuilder.Append ("\r\n");

        createTableStringBuilder.AppendFormat (
         "CREATE SYNONYM [{0}].[{1}] FOR [{2}].[{3}]\r\n",
         synonym.SchemaName ?? SqlScriptBuilder.DefaultSchema,
         synonym.EntityName,
         entityDefinition.ViewName.SchemaName ?? SqlScriptBuilder.DefaultSchema,
         entityDefinition.ViewName.EntityName);
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
         "DROP SYNONYM [{0}].[{1}]\r\n",
         synonym.SchemaName ?? SqlScriptBuilder.DefaultSchema,
         synonym.EntityName);
      }
    }
  }
}