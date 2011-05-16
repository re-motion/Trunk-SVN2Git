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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer
{
  public class ExtendedSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    public override IScriptBuilder CreateSchemaScriptBuilder (RdbmsProviderDefinition storageProviderDefinition)
    {
      var compositeScriptBuilder = new CompositeScriptBuilder (
          storageProviderDefinition,
          CreateTableBuilder(),
          CreateConstraintBuilder(),
          CreateViewBuilder(),
          CreateIndexBuilder(),
          CreateSynonymBuilder());

      var customCreateBeginningScript = "";
      var customCreateEndScript = "--Extendend file-builder comment at the end";
      var customDropBeginningScript = 
        "IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'Test') BEGIN EXEC('CREATE SCHEMA Test') END\r\nGO\r\n"
       + "--Extendend file-builder comment at the beginning\r\n";
      var customDropEndScript = "";

      return
          new CustomCommentScriptElementBuilder (
              new SqlDatabaseSelectionScriptElementBuilder (compositeScriptBuilder, storageProviderDefinition.ConnectionString),
              customCreateBeginningScript,
              customCreateEndScript,
              customDropBeginningScript,
              customDropEndScript);
    }
  }
}