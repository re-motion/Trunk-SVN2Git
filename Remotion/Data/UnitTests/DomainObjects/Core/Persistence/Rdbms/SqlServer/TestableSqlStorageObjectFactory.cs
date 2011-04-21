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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer
{
  public class TestableSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    private readonly SqlTableScriptBuilder _tableBuilder;
    private readonly SqlViewScriptBuilder _viewBuilder;
    private readonly SqlConstraintScriptBuilder _constraintBuilder;
    private readonly SqlIndexScriptBuilder _indexBuilder;
    private readonly SqlSynonymScriptBuilder _synonymBuilder;

    public TestableSqlStorageObjectFactory (
        SqlTableScriptBuilder tableBuilder,
        SqlViewScriptBuilder viewBuilder,
        SqlConstraintScriptBuilder constraintBuilder,
        SqlIndexScriptBuilder indexBuilder,
        SqlSynonymScriptBuilder synonymBuilder)
    {
      _indexBuilder = indexBuilder;
      _constraintBuilder = constraintBuilder;
      _viewBuilder = viewBuilder;
      _tableBuilder = tableBuilder;
      _synonymBuilder = synonymBuilder;
    }

    protected override SqlTableScriptBuilder CreateTableBuilder ()
    {
      return _tableBuilder;
    }

    protected override SqlViewScriptBuilder CreateViewBuilder ()
    {
      return _viewBuilder;
    }

    protected override SqlConstraintScriptBuilder CreateConstraintBuilder ()
    {
      return _constraintBuilder;
    }

    protected override SqlIndexScriptBuilder CreateIndexBuilder ()
    {
      return _indexBuilder;
    }

    protected override SqlSynonymScriptBuilder CreateSynonymBuilder ()
    {
      return _synonymBuilder;
    }
  }
}