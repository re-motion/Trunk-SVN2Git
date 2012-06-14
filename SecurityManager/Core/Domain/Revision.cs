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
      statement.Append (sqlDialect.DelimitIdentifier ("Value"));
      statement.Append (" FROM ");
      statement.Append (sqlDialect.DelimitIdentifier ("Revision"));
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

      var statement = new StringBuilder();
      statement.Append ("Update ");
      statement.Append (sqlDialect.DelimitIdentifier ("Revision"));
      statement.Append (" SET ");
      statement.Append (sqlDialect.DelimitIdentifier ("Value"));
      statement.Append (" = ");
      statement.Append (sqlDialect.DelimitIdentifier ("Value"));
      statement.Append (" + 1");
      statement.Append (sqlDialect.StatementDelimiter);

      return QueryFactory.CreateQuery (
          new QueryDefinition (
              typeof (Revision) + "." + MethodBase.GetCurrentMethod().Name,
              storageProviderDefinition,
              statement.ToString(),
              QueryType.Scalar));
    }

    private static RdbmsProviderDefinition GetStorageProviderDefinition ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (SecurableClassDefinition));
      return (RdbmsProviderDefinition) classDefinition.StorageEntityDefinition.StorageProviderDefinition;
    }
  }
}
