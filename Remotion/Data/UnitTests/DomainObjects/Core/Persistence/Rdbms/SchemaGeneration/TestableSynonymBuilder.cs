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
  public class TestableSynonymBuilder : SynonymBuilderBase
  {
    public TestableSynonymBuilder (ISqlDialect sqlDialect)
        : base(sqlDialect)
    {
    }

    public override void AddToCreateSynonymScript (EntityDefinitionBase tableDefinition, StringBuilder createTableStringBuilder)
    {
      if(tableDefinition.Synonyms.Count>0)
        createTableStringBuilder.Append ("CREATE SYNONYM ["+ tableDefinition.Synonyms[0].EntityName +"] FOR [" + tableDefinition.ViewName.EntityName + "]");
    }

    public override void AddToDropSynonymScript (EntityDefinitionBase tableDefinition, StringBuilder dropTableStringBuilder)
    {
      if(tableDefinition.Synonyms.Count>0)
        dropTableStringBuilder.Append ("DROP SYNONYM [" + tableDefinition.Synonyms[0].EntityName + "]");
    }
  }
}