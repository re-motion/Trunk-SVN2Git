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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses.ResultOperators
{
  /// <summary>
  /// Represents the "Cast" part of a query. This is a result operator, operating on the whole result set of a query.
  /// </summary>
  /// <example>
  /// In C#, the "Cast" clause in the following example corresponds to a <see cref="CastResultOperator"/>.
  /// <code>
  /// var query = (from s in Students
  ///              select s.ID).Cast&lt;int&gt;();
  /// </code>
  /// </example>
  public class CastResultOperator : ResultOperatorBase
  {
    private Type _castItemType;

    public CastResultOperator (Type castItemType)
        : base (CollectionExecutionStrategy.Instance)
    {
      ArgumentUtility.CheckNotNull ("castItemType", castItemType);
      CastItemType = castItemType;
    }

    public Type CastItemType
    {
      get { return _castItemType; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _castItemType = value;
      }
    }

    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      return new CastResultOperator(CastItemType);
    }

    public override object ExecuteInMemory (object input)
    {
      ArgumentUtility.CheckNotNull ("input", input);

      var method = (from m in typeof (CastResultOperator).GetMethods ()
                    where m.Name == "ExecuteInMemory" && m.IsGenericMethod
                    select m.MakeGenericMethod (CastItemType)).Single();
      return InvokeExecuteMethod (input, method);
    }

    public IEnumerable<TResult> ExecuteInMemory<TResult> (IEnumerable input)
    {
      return input.Cast<TResult>();
    }

    public override Type GetResultType (Type inputResultType)
    {
      ArgumentUtility.CheckNotNull ("inputResultType", inputResultType);
      ReflectionUtility.GetItemTypeOfIEnumerable (inputResultType, "inputResultType"); // check whether inputResultType implements IEnumerable<T>

      return typeof (IQueryable<>).MakeGenericType (CastItemType);
    }

    public override string ToString ()
    {
      return "Cast()";
    }
  }
}