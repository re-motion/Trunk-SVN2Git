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
    private readonly SqlScriptTableBuilder _tableBuilder;
    private readonly SqlScriptViewBuilder _viewBuilder;
    private readonly SqlScriptConstraintBuilder _constraintBuilder;
    private readonly SqlScriptIndexBuilder _indexBuilder;
    private readonly SqlScriptSynonymBuilder _synonymBuilder;
    public const string DefaultSchema = "dbo";

    // TODO: Add tests using mocks as soon as interfaces can be used for partial script builders
    public SqlScriptBuilder (
        RdbmsProviderDefinition rdbmsProviderDefinition,
        SqlScriptTableBuilder tableBuilder,
        SqlScriptViewBuilder viewBuilder,
        SqlScriptConstraintBuilder constraintBuilder,
        SqlScriptIndexBuilder indexBuilder,
        SqlScriptSynonymBuilder synonymBuilder)
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

    public SqlScriptTableBuilder TableBuilder
    {
      get { return _tableBuilder; }
    }

    public SqlScriptViewBuilder ViewBuilder
    {
      get { return _viewBuilder; }
    }

    public SqlScriptConstraintBuilder ConstraintBuilder
    {
      get { return _constraintBuilder; }
    }

    public SqlScriptIndexBuilder IndexBuilder
    {
      get { return _indexBuilder; }
    }

    public SqlScriptSynonymBuilder SynonymBuilder
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
          + "-- Create all tables\r\n"
          + "{1}GO\r\n\r\n"
          + "-- Create constraints for tables that were created above\r\n"
          + "{2}GO\r\n\r\n"
          + "-- Create a view for every class\r\n"
          + "{3}GO\r\n\r\n"
          + "-- Create indexes for tables that were created above\r\n"
          + "{4}GO\r\n\r\n"
          + "-- Create synonyms for tables that were created above\r\n"
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
          + "-- Drop all synonyms that will be created below\r\n"
          + "{1}GO\r\n\r\n"
          + "-- Drop all indexes that will be created below\r\n"
          + "{2}GO\r\n\r\n"
          + "-- Drop all views that will be created below\r\n"
          + "{3}GO\r\n\r\n"
          + "-- Drop foreign keys of all tables that will be created below\r\n"
          + "{4}GO\r\n\r\n"
          + "-- Drop all tables that will be created below\r\n"
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