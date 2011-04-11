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
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  public class TestableTableBuilder : TableBuilderBase
  {
    public TestableTableBuilder (ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
    }

    public override void AddToCreateTableScript (TableDefinition tableDefinition, StringBuilder createTableStringBuilder)
    {
      createTableStringBuilder.Append ("CREATE TABLE ["+tableDefinition.TableName+"]");
    }

    public override void AddToDropTableScript (TableDefinition tableDefinition, StringBuilder dropTableStringBuilder)
    {
      dropTableStringBuilder.Append ("DROP TABLE [" + tableDefinition.TableName + "]");
    }
  }
}