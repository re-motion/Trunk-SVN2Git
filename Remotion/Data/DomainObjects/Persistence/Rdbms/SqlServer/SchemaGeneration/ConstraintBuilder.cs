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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class ConstraintBuilder : ConstraintBuilderBase
  {
    public ConstraintBuilder () : base (SqlDialect.Instance)
    {
    }

    public override void AddToCreateConstraintScript (TableDefinition tableDefinition, StringBuilder createConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("createConstraintStringBuilder", createConstraintStringBuilder);

      var constraintStatement = GetForeignKeyConstraintStatement (tableDefinition);
      if (!string.IsNullOrEmpty (constraintStatement))
      {
        createConstraintStringBuilder.AppendFormat (
            "ALTER TABLE [{0}].[{1}] ADD\r\n {2}\r\n",
            tableDefinition.TableName.SchemaName ?? ScriptBuilder.DefaultSchema,
            tableDefinition.TableName.EntityName,
            constraintStatement);
      }
    }

    public override void AddToDropConstraintScript (List<EntityNameDefinition> entityNamesForDropConstraintScript, StringBuilder dropConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("entityNamesForDropConstraintScript", entityNamesForDropConstraintScript);
      ArgumentUtility.CheckNotNull ("dropConstraintStringBuilder", dropConstraintStringBuilder);

      dropConstraintStringBuilder.AppendFormat (
          "DECLARE @statement nvarchar (max)\r\n"
          + "SET @statement = ''\r\n"
          + "SELECT @statement = @statement + 'ALTER TABLE [' + schema_name(t.schema_id) + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + ']; ' \r\n"
          + "FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id \r\n"
          + "WHERE fk.type = 'F' AND schema_name (t.schema_id) + '.' + t.name IN ('{0}') \r\n"
          + "ORDER BY t.name, fk.name\r\n"
          + "exec sp_executesql @statement\r\n",
          string.Join("', '", entityNamesForDropConstraintScript.Select(en=>(en.SchemaName ?? ScriptBuilder.DefaultSchema)+"."+en.EntityName).ToArray()));
    }
  
  }
}
