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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.SqlBackend.SqlGeneration;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel;
using Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
{
  public class TestSqlGenerationStageMixin
  {
    [OverrideTarget]
    public virtual Expression<Func<IDatabaseResultRow, object>> GenerateTextForOuterSqlStatement (
        ISqlCommandBuilder commandBuilder, SqlStatement sqlStatement)
    {
      Assert.That (sqlStatement.SelectProjection, Is.TypeOf (typeof (SqlEntityDefinitionExpression)));
      Assert.That (sqlStatement.SelectProjection.Type, Is.EqualTo (typeof (int)));
      Assert.That (((SqlEntityDefinitionExpression) sqlStatement.SelectProjection).TableAlias, Is.EqualTo ("c"));
      Assert.That (((SqlEntityDefinitionExpression) sqlStatement.SelectProjection).Name, Is.EqualTo ("CookTable"));

      commandBuilder.Append ("Value added by generation mixin");

      return null;
    }
  }
}