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
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.Backend.SqlGeneration.SqlServer
{
  public class SelectBuilder : ISelectBuilder
  {
    private readonly CommandBuilder _commandBuilder;

    public SelectBuilder (CommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
    }

    public void BuildSelectPart (SqlGenerationData sqlGenerationData)
    {
      ArgumentUtility.CheckNotNull ("sqlGenerationData", sqlGenerationData);

      _commandBuilder.Append ("SELECT ");
      AppendSelectEvaluation (sqlGenerationData.SelectEvaluation, sqlGenerationData.ResultOperators);
    }

    public void AppendSelectEvaluation (IEvaluation selectEvaluation, List<ResultOperatorBase> resultOperators)
    {
      ArgumentUtility.CheckNotNull ("selectEvaluation", selectEvaluation);
      ArgumentUtility.CheckNotNull ("resultOperators", resultOperators);

      //at the moment, result operators may not be correctly combined; for example Take and First might not be correctly combined
      bool distinct = false;
      int? top = null;
      bool count = false;

      foreach (var operatorBase in resultOperators)
      {
        if (operatorBase is FirstResultOperator || operatorBase is SingleResultOperator)
          top = 1;
        else if (operatorBase is CountResultOperator)
          count = true;
        else if (operatorBase is DistinctResultOperator)
          distinct = true;
        else if (operatorBase is TakeResultOperator)
          top = ((TakeResultOperator) operatorBase).GetConstantCount ();
        else if (!(operatorBase is CastResultOperator))
        {
          throw new NotSupportedException (
              "Result operator type " + operatorBase.GetType ().Name + " is not supported by this SQL generator.");
        }
      }

      AppendSelectEvaluation (selectEvaluation, distinct, top, count);
    }

    public void AppendSelectEvaluation (IEvaluation selectEvaluation, bool distinct, int? top, bool count)
    {
      ArgumentUtility.CheckNotNull ("selectEvaluation", selectEvaluation);

      if (distinct)
        _commandBuilder.Append ("DISTINCT ");
      if (top != null)
        _commandBuilder.Append ("TOP " + top + " ");

      if (count)
        _commandBuilder.Append ("COUNT (*) ");
      else
        AppendEvaluation (selectEvaluation);
    }

    private void AppendEvaluation (IEvaluation selectEvaluation)
    {
      _commandBuilder.AppendEvaluation (selectEvaluation);
      _commandBuilder.Append (" ");
    }
  }
}
