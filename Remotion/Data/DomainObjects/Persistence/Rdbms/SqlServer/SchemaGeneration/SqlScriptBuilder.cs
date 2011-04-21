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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class SqlScriptBuilder : ScriptBuilderBase
  {
    private readonly SqlTableScriptBuilder _tableBuilder;
    private readonly SqlViewScriptBuilder _viewBuilder;
    private readonly SqlConstraintScriptBuilder _constraintBuilder;
    private readonly SqlIndexScriptBuilder _indexBuilder;
    private readonly SqlSynonymScriptBuilder _synonymBuilder;
    public const string DefaultSchema = "dbo";

    // TODO: Add tests using mocks as soon as interfaces can be used for partial script builders
    public SqlScriptBuilder (
        RdbmsProviderDefinition rdbmsProviderDefinition,
        SqlTableScriptBuilder tableBuilder,
        SqlViewScriptBuilder viewBuilder,
        SqlConstraintScriptBuilder constraintBuilder,
        SqlIndexScriptBuilder indexBuilder,
        SqlSynonymScriptBuilder synonymBuilder)
      : base (ArgumentUtility.CheckNotNull ("rdbmsProviderDefinition", rdbmsProviderDefinition))
    {
      ArgumentUtility.CheckNotNull ("tableBuilder", tableBuilder);
      ArgumentUtility.CheckNotNull ("viewBuilder", viewBuilder);
      ArgumentUtility.CheckNotNull ("constraintBuilder", constraintBuilder);
      ArgumentUtility.CheckNotNull ("indexBuilder", indexBuilder);
      ArgumentUtility.CheckNotNull ("synonymBuilder", synonymBuilder);

      _tableBuilder = tableBuilder;
      _viewBuilder = viewBuilder;
      _constraintBuilder = constraintBuilder;
      _indexBuilder = indexBuilder;
      _synonymBuilder = synonymBuilder;
    }

    public SqlTableScriptBuilder TableBuilder
    {
      get { return _tableBuilder; }
    }

    public SqlViewScriptBuilder ViewBuilder
    {
      get { return _viewBuilder; }
    }

    public SqlConstraintScriptBuilder ConstraintBuilder
    {
      get { return _constraintBuilder; }
    }

    public SqlIndexScriptBuilder IndexBuilder
    {
      get { return _indexBuilder; }
    }

    public SqlSynonymScriptBuilder SynonymBuilder
    {
      get { return _synonymBuilder; }
    }

    public string GetDatabaseName ()
    {
      //TODO improve this logic
      var initialCatalogMarker = "Initial Catalog=";
      var startIndex = RdbmsProviderDefinition.ConnectionString.IndexOf (initialCatalogMarker) + initialCatalogMarker.Length;
      string temp = RdbmsProviderDefinition.ConnectionString.Substring (startIndex);
      return temp.Substring (0, temp.IndexOf (";"));
    }

    public override void AddEntityDefinition (IEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull ("entityDefinition", entityDefinition);

      _viewBuilder.AddEntityDefinition (entityDefinition);
      _tableBuilder.AddEntityDefinition (entityDefinition);
      _constraintBuilder.AddEntityDefinition (entityDefinition);
      _indexBuilder.AddEntityDefinition (entityDefinition);
      _synonymBuilder.AddEntityDefinition (entityDefinition);
    }

    public override string GetCreateScript (IEnumerable<IEntityDefinition> entityDefinitions)
    {
      return string.Format (
          "USE {0}\r\n"
          + "GO\r\n\r\n"
          + "{1}GO\r\n\r\n"
          + "{2}GO\r\n\r\n"
          + "{3}GO\r\n\r\n"
          + "{4}GO\r\n\r\n"
          + "{5}GO\r\n",
          GetDatabaseName (),
          _tableBuilder.GetCreateScript (),
          _constraintBuilder.GetCreateScript (),
          _viewBuilder.GetCreateScript (),
          _indexBuilder.GetCreateScript (),
          _synonymBuilder.GetCreateScript ());
    }

    public override string GetDropScript (IEnumerable<IEntityDefinition> entityDefinitions)
    {
      return string.Format (
          "USE {0}\r\n"
          + "GO\r\n\r\n"
          + "{1}GO\r\n\r\n"
          + "{2}GO\r\n\r\n"
          + "{3}GO\r\n\r\n"
          + "{4}GO\r\n\r\n"
          + "{5}GO\r\n",
          GetDatabaseName (),
          _synonymBuilder.GetDropScript (),
          _indexBuilder.GetDropScript (),
          _viewBuilder.GetDropScript (),
          _constraintBuilder.GetDropScript (),
          _tableBuilder.GetDropScript ());
    }
  }
}