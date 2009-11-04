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
using System.Text;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using System.IO;
using Remotion.Data.DomainObjects.CodeGenerator.Sql;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator
{
  public class SqlFileBuilder : SqlFileBuilderBase
  {
    // types

    // static members and constants

    // member fields


    // construction and disposing

    public SqlFileBuilder (MappingConfiguration mappingConfiguration, RdbmsProviderDefinition rdbmsProviderDefinition)
      : base (mappingConfiguration, rdbmsProviderDefinition)
    {
    }

    // methods and properties 

    public string GetDatabaseName ()
    {
      //TODO improve this logic
      string temp = RdbmsProviderDefinition.ConnectionString.Substring (RdbmsProviderDefinition.ConnectionString.IndexOf ("Initial Catalog=") + 16);
      return temp.Substring (0, temp.IndexOf (";"));
    }

    public override string GetScript ()
    {
      ViewBuilder viewBuilder = new ViewBuilder ();
      viewBuilder.AddViews (Classes);

      TableBuilder tableBuilder = new TableBuilder ();
      tableBuilder.AddTables (Classes);

      ConstraintBuilder constraintBuilder = new ConstraintBuilder ();
      constraintBuilder.AddConstraints (Classes);

      return string.Format ("-- Drop all views that will be created below\r\n"
          + "{1}\r\n"
          + "-- Drop foreign keys of all tables that will be created below\r\n"
          + "{2}\r\n"
          + "-- Drop all tables that will be created below\r\n"
          + "{3}\r\n" 
          + "-- Create all tables\r\n"
          + "{4}\r\n"
          + "-- Create constraints for tables that were created above\r\n"
          + "{5}\r\n"
          + "-- Create a view for every class\r\n"
          + "{6}", 
          GetDatabaseName (), 
          viewBuilder.GetDropViewScript (),
          constraintBuilder.GetDropConstraintScript (), 
          tableBuilder.GetDropTableScript (),
          tableBuilder.GetCreateTableScript (),
          constraintBuilder.GetAddConstraintScript (), 
          viewBuilder.GetCreateViewScript ());
    }
  }
}
