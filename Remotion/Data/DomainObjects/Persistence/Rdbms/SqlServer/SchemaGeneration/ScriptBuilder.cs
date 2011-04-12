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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  public class ScriptBuilder : ScriptBuilderBase
  {
    public const string DefaultSchema = "dbo";

    public ScriptBuilder (RdbmsProviderDefinition rdbmsProviderDefinition)
        : base (rdbmsProviderDefinition)
    {
    }

    public string GetDatabaseName ()
    {
      //TODO improve this logic
      string temp = RdbmsProviderDefinition.ConnectionString.Substring (RdbmsProviderDefinition.ConnectionString.IndexOf ("Initial Catalog=") + 16);
      return temp.Substring (0, temp.IndexOf (";"));
    }

    public override string GetScript (IEnumerable<IEntityDefinition> entityDefinitions)
    {
      // TODO 3874: Add unit tests using mocks for view builder etc.
      var viewBuilder = CreateViewBuilder();
      var tableBuilder = CreateTableBuilder();
      var constraintBuilder = CreateConstraintBuilder();

      foreach (var entityDefinition in entityDefinitions)
      {
        viewBuilder.AddView (entityDefinition);
        tableBuilder.AddTable (entityDefinition);
        constraintBuilder.AddConstraint (entityDefinition);
      }

      return string.Format (
          "USE {0}\r\n"
          + "GO\r\n\r\n"
          + "-- Drop all views that will be created below\r\n"
          + "{1}GO\r\n\r\n"
          + "-- Drop foreign keys of all tables that will be created below\r\n"
          + "{2}GO\r\n\r\n"
          + "-- Drop all tables that will be created below\r\n"
          + "{3}GO\r\n\r\n"
          + "-- Create all tables\r\n"
          + "{4}GO\r\n\r\n"
          + "-- Create constraints for tables that were created above\r\n"
          + "{5}GO\r\n\r\n"
          + "-- Create a view for every class\r\n"
          + "{6}GO\r\n",
          GetDatabaseName(),
          viewBuilder.GetDropViewScript(),
          constraintBuilder.GetDropConstraintScript(),
          tableBuilder.GetDropTableScript(),
          tableBuilder.GetCreateTableScript(),
          constraintBuilder.GetAddConstraintScript(),
          viewBuilder.GetCreateViewScript());
    }

    protected virtual TableBuilder CreateTableBuilder ()
    {
      return new TableBuilder();
    }

    protected virtual ViewBuilder CreateViewBuilder ()
    {
      return new ViewBuilder();
    }

    protected virtual ConstraintBuilder CreateConstraintBuilder ()
    {
      return new ConstraintBuilder();
    }
  }
}