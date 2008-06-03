/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer
{
  public class FileBuilder : FileBuilderBase
  {
    public const string DefaultSchema = "dbo";

    public FileBuilder (MappingConfiguration mappingConfiguration, RdbmsProviderDefinition rdbmsProviderDefinition)
        : base (mappingConfiguration, rdbmsProviderDefinition)
    {
    }

    public string GetDatabaseName ()
    {
      //TODO improve this logic
      string temp = RdbmsProviderDefinition.ConnectionString.Substring (RdbmsProviderDefinition.ConnectionString.IndexOf ("Initial Catalog=") + 16);
      return temp.Substring (0, temp.IndexOf (";"));
    }

    public override string GetScript ()
    {
      ViewBuilder viewBuilder = CreateViewBuilder();
      viewBuilder.AddViews (Classes);

      TableBuilder tableBuilder = CreateTableBuilder();
      tableBuilder.AddTables (Classes);

      ConstraintBuilder constraintBuilder = CreateConstraintBuilder();
      constraintBuilder.AddConstraints (Classes);

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
