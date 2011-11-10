// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.Sql2005
{
  public class TestableSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    private readonly TableScriptBuilder _tableBuilder;
    private readonly ViewScriptBuilder _viewBuilder;
    private readonly ForeignKeyConstraintScriptBuilder _constraintBuilder;
    private readonly IndexScriptBuilder _indexBuilder;
    private readonly SynonymScriptBuilder _synonymBuilder;

    public TestableSqlStorageObjectFactory (
        TableScriptBuilder tableBuilder,
        ViewScriptBuilder viewBuilder,
        ForeignKeyConstraintScriptBuilder constraintBuilder,
        IndexScriptBuilder indexBuilder,
        SynonymScriptBuilder synonymBuilder)
    {
      _indexBuilder = indexBuilder;
      _constraintBuilder = constraintBuilder;
      _viewBuilder = viewBuilder;
      _tableBuilder = tableBuilder;
      _synonymBuilder = synonymBuilder;
    }

    protected override TableScriptBuilder CreateTableBuilder ()
    {
      return _tableBuilder;
    }

    protected override ViewScriptBuilder CreateViewBuilder ()
    {
      return _viewBuilder;
    }

    protected override ForeignKeyConstraintScriptBuilder CreateConstraintBuilder ()
    {
      return _constraintBuilder;
    }

    protected override IndexScriptBuilder CreateIndexBuilder ()
    {
      return _indexBuilder;
    }

    protected override SynonymScriptBuilder CreateSynonymBuilder ()
    {
      return _synonymBuilder;
    }
  }
}