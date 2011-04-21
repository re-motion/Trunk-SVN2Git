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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class SqlConstraintScriptBuilder : ConstraintScriptBuilderBase
  {
    public SqlConstraintScriptBuilder () : base (SqlDialect.Instance)
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
            tableDefinition.TableName.SchemaName ?? SqlCompositeScriptBuilder.DefaultSchema,
            tableDefinition.TableName.EntityName,
            constraintStatement);
      }
    }

    public override void AddToDropConstraintScript (TableDefinition tableDefinition, StringBuilder dropConstraintStringBuilder)
    {
      ArgumentUtility.CheckNotNull ("tableDefinition", tableDefinition);
      ArgumentUtility.CheckNotNull ("dropConstraintStringBuilder", dropConstraintStringBuilder);

      var count = 0;
      foreach (var foreignKeyConstraintDefinition in tableDefinition.Constraints.OfType<ForeignKeyConstraintDefinition>())
      {
        dropConstraintStringBuilder.AppendFormat (
            "{0}IF EXISTS (SELECT * FROM sys.objects fk INNER JOIN sys.objects t ON fk.parent_object_id = t.object_id where fk.type = 'F' AND "
            +"fk.name = '{3}' AND schema_name (t.schema_id) = '{1}' AND t.name = '{2}')\r\n"
            +"  ALTER TABLE [{1}].[{2}] DROP CONSTRAINT {3}\r\n",
            count>0 ? "\r\n" : string.Empty,
            tableDefinition.TableName.SchemaName ?? SqlCompositeScriptBuilder.DefaultSchema,
            tableDefinition.TableName.EntityName,
            foreignKeyConstraintDefinition.ConstraintName);
        count++;
      }
    }
  
  }
}
