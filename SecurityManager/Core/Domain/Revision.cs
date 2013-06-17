// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Reflection;
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain
{
  public static class Revision
  {
    public static IQuery GetGetRevisionQuery ()
    {
      var storageProviderDefinition = GetStorageProviderDefinition();
      var sqlDialect = storageProviderDefinition.Factory.CreateSqlDialect (storageProviderDefinition);

      var statement = new StringBuilder();
      statement.Append ("SELECT ");
      statement.Append (GetRevisionColumnIdentifier (sqlDialect));
      statement.Append (" FROM ");
      statement.Append (GetRevisionTableIdentifier (sqlDialect));
      statement.Append (sqlDialect.StatementDelimiter);

      return QueryFactory.CreateQuery (
          new QueryDefinition (
              typeof (Revision) + "." + MethodBase.GetCurrentMethod().Name,
              storageProviderDefinition,
              statement.ToString(),
              QueryType.Scalar));
    }

    public static IQuery GetIncrementRevisionQuery ()
    {
      var storageProviderDefinition = GetStorageProviderDefinition();
      var sqlDialect = storageProviderDefinition.Factory.CreateSqlDialect (storageProviderDefinition);

      const string incrementrevision = "IncrementRevision";
      string revisionTable = GetRevisionTableIdentifier (sqlDialect);
      string revisionColumn = GetRevisionColumnIdentifier (sqlDialect);
      string revisionValueParameter = sqlDialect.GetParameterName ("value");

      var statement = new StringBuilder();
      statement.Append ("BEGIN TRANSACTION " + incrementrevision);
      statement.Append (sqlDialect.StatementDelimiter);
      statement.AppendLine();
      statement.Append ("IF EXISTS (SELECT 0 FROM ");
      statement.Append (revisionTable);
      statement.Append (")");
      statement.AppendLine();

      statement.Append ("UPDATE ");
      statement.Append (revisionTable);
      statement.Append (" SET ");
      statement.Append (revisionColumn);
      statement.Append (" = ");
      statement.Append (revisionColumn);
      statement.Append (" + 1");
      statement.AppendLine();

      statement.Append ("ELSE");
      statement.AppendLine();

      statement.Append ("INSERT INTO ");
      statement.Append (revisionTable);
      statement.Append ("(");
      statement.Append (revisionColumn);
      statement.Append (") VALUES (");
      statement.Append (revisionValueParameter);
      statement.Append (")");
      statement.Append (sqlDialect.StatementDelimiter);
      statement.AppendLine();
      statement.Append ("COMMIT TRANSACTION " + incrementrevision);
      statement.Append (sqlDialect.StatementDelimiter);
      statement.AppendLine();

      var parameters = new QueryParameterCollection();
      parameters.Add (revisionValueParameter, 1);

      return QueryFactory.CreateQuery (
          new QueryDefinition (
              typeof (Revision) + "." + MethodBase.GetCurrentMethod().Name,
              storageProviderDefinition,
              statement.ToString(),
              QueryType.Scalar), parameters);
    }

    private static RdbmsProviderDefinition GetStorageProviderDefinition ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (SecurableClassDefinition));
      return (RdbmsProviderDefinition) classDefinition.StorageEntityDefinition.StorageProviderDefinition;
    }

    private static string GetRevisionTableIdentifier (ISqlDialect sqlDialect)
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (SecurableClassDefinition));
      var tableDefinition = (TableDefinition) classDefinition.StorageEntityDefinition;

      if (tableDefinition.TableName.SchemaName == null)
        return sqlDialect.DelimitIdentifier ("Revision");
      else
        return sqlDialect.DelimitIdentifier (tableDefinition.TableName.SchemaName) + "." + sqlDialect.DelimitIdentifier ("Revision");
    }

    private static string GetRevisionColumnIdentifier (ISqlDialect sqlDialect)
    {
      return sqlDialect.DelimitIdentifier ("Value");
    }
  }
}