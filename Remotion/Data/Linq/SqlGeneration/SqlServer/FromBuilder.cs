// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Collections.Generic;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Data.Linq.Parsing;
using Remotion.Utilities;


namespace Remotion.Data.Linq.SqlGeneration.SqlServer
{
  public class FromBuilder : IFromBuilder
  {
    private readonly CommandBuilder _commandBuilder;
    private readonly IDatabaseInfo _databaseInfo;

    public FromBuilder (CommandBuilder commandBuilder, IDatabaseInfo databaseInfo)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
      _databaseInfo = databaseInfo;
    }

    public void BuildFromPart (List<IColumnSource> fromSources, JoinCollection joins)
    {
      _commandBuilder.Append ("FROM ");

      bool first = true;
      foreach (IColumnSource fromSource in fromSources)
      {
        Table table = fromSource as Table;
        if (table != null)
        {
          if (!first)
            _commandBuilder.Append (", ");
          _commandBuilder.Append (SqlServerUtility.GetTableDeclaration (table));
        }
        else
          AppendCrossApply ((SubQuery) fromSource);

        AppendJoinPart (joins[fromSource]);
        first = false;
      }
    }
    
    private void AppendCrossApply (SubQuery subQuery)
    {
      _commandBuilder.Append (" CROSS APPLY (");
      ISqlGenerator subQueryGenerator = CreateSqlGeneratorForSubQuery(subQuery, _databaseInfo, _commandBuilder);
      subQueryGenerator.BuildCommand (subQuery.QueryModel);
      _commandBuilder.Append (") ");
      _commandBuilder.Append (SqlServerUtility.WrapSqlIdentifier (subQuery.Alias));
    }

    protected virtual ISqlGenerator CreateSqlGeneratorForSubQuery (SubQuery subQuery, IDatabaseInfo databaseInfo, CommandBuilder commandBuilder)
    {
      return new InlineSqlServerGenerator (databaseInfo, commandBuilder, ParseMode.SubQueryInFrom);
    }

    private void AppendJoinPart (IEnumerable<SingleJoin> joins)
    {
      foreach (SingleJoin join in joins)
        AppendJoinExpression (join);
    }

    private void AppendJoinExpression (SingleJoin join)
    {
      _commandBuilder.Append (" LEFT OUTER JOIN ");
      _commandBuilder.Append (SqlServerUtility.GetTableDeclaration ((Table) join.RightSide));
      _commandBuilder.Append (" ON ");
      _commandBuilder.Append (SqlServerUtility.GetColumnString (join.LeftColumn));
      _commandBuilder.Append (" = ");
      _commandBuilder.Append (SqlServerUtility.GetColumnString (join.RightColumn));
    }


    public void BuildLetPart (List<LetData> letDataCollection)
    {
      ArgumentUtility.CheckNotNull ("letData", letDataCollection);
      foreach (var letData in letDataCollection)
      {
        _commandBuilder.Append (" CROSS APPLY (SELECT ");
        _commandBuilder.AppendEvaluation (letData.Evaluation);
        
        if (!letData.CorrespondingColumnSource.IsTable)
          _commandBuilder.Append (" " + SqlServerUtility.WrapSqlIdentifier (letData.Name));

        _commandBuilder.Append (") [");
        _commandBuilder.Append (letData.Name);
        _commandBuilder.Append ("]");
      }
    }
  }
}
