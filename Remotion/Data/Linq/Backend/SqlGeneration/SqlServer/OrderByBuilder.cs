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
using Remotion.Data.Linq.Utilities;
using System.Linq;

namespace Remotion.Data.Linq.Backend.SqlGeneration.SqlServer
{
  public class OrderByBuilder : IOrderByBuilder
  {
    private static string GetDirectionString (OrderingDirection direction)
    {
      switch (direction)
      {
        case OrderingDirection.Asc:
          return "ASC";
        case OrderingDirection.Desc:
          return "DESC";
        default:
          throw new NotSupportedException ("OrderingDirection " + direction + " is not supported.");
      }
    }

    private readonly CommandBuilder _commandBuilder;

    public OrderByBuilder (CommandBuilder commandBuilder)
    {
      ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
      _commandBuilder = commandBuilder;
    }

    public void BuildOrderByPart (SqlGenerationData sqlGenerationData)
    {
      ArgumentUtility.CheckNotNull ("sqlGenerationData", sqlGenerationData);

      if (sqlGenerationData.OrderingFields.Count > 0)
      {
        _commandBuilder.Append (" ORDER BY ");
        AppendOrderingFields (sqlGenerationData.OrderingFields);
      }
    }

    public void AppendOrderingFields (IEnumerable<OrderingField> orderingFields)
    {
      ArgumentUtility.CheckNotNull ("orderingFields", orderingFields);

      _commandBuilder.AppendSeparatedItems (orderingFields, AppendOrderingField);
    }

    public void AppendOrderingField (OrderingField orderingField)
    {
      _commandBuilder.AppendEvaluation (orderingField.Column);
      _commandBuilder.Append (" ");
      _commandBuilder.Append (GetDirectionString (orderingField.Direction));
    }

  }
}
