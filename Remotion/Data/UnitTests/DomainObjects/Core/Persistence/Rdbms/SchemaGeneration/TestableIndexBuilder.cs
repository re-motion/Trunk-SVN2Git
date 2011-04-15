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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  public class TestableIndexBuilder : IndexBuilderBase, ISqlIndexDefinitionVisitor
  {
    public TestableIndexBuilder (ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
    }

    public override string IndexStatementSeparator
    {
      get { throw new NotImplementedException (); }
    }

    public void VisitIndexDefinition (IndexDefinition indexDefinition)
    {
      CreateIndexStringBuilder.AppendLine ("CREATE INDEX " + indexDefinition.IndexName + " (IndexDefinition)");
      DropIndexStringBuilder.AppendLine ("DROP INDEX " + indexDefinition.IndexName + " (IndexDefinition)");
    }

    public void VisitPrimaryXmlIndexDefinition (PrimaryXmlIndexDefinition primaryXmlIndexDefinition)
    {
      CreateIndexStringBuilder.AppendLine ("CREATE INDEX " + primaryXmlIndexDefinition.IndexName + " (PrimaryXmlIndexDefinition)");
      DropIndexStringBuilder.AppendLine ("DROP INDEX " + primaryXmlIndexDefinition.IndexName + " (PrimaryXmlIndexDefinition)");
    }

    public void VisitSecondaryXmlIndexDefinition (SecondaryXmlIndexDefinition secondaryXmlIndexDefinition)
    {
      CreateIndexStringBuilder.AppendLine ("CREATE INDEX " + secondaryXmlIndexDefinition.IndexName + " (SecondaryXmlIndexDefinition)");
      DropIndexStringBuilder.AppendLine ("DROP INDEX " + secondaryXmlIndexDefinition.IndexName + " (SecondaryXmlIndexDefinition)");
    }
  }
}