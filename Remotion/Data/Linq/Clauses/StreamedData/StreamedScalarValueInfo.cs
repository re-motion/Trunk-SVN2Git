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
using System;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.EagerFetching;

namespace Remotion.Data.Linq.Clauses.StreamedData
{
  /// <summary>
  /// Describes a scalar value streamed out of a <see cref="QueryModel"/> or <see cref="ResultOperatorBase"/>. A scalar value corresponds to a
  /// value calculated from the result set, as produced by <see cref="CountResultOperator"/> or <see cref="ContainsResultOperator"/>, for instance.
  /// </summary>
  public class StreamedScalarValueInfo : StreamedValueInfo
  {
    public StreamedScalarValueInfo (Type dataType)
        : base(dataType)
    {
    }

    public override IStreamedData ExecuteQueryModel (QueryModel queryModel, FetchRequestBase[] fetchRequests, IQueryExecutor executor)
    {
      throw new NotImplementedException();
    }
  }
}