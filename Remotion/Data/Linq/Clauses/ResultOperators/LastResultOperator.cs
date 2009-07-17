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
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.Linq.Clauses.ExecutionStrategies;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses.ResultOperators
{
  /// <summary>
  /// Represents the last part of a query. This is a result operator, operating on the whole result set of a query.
  /// </summary>
  /// <example>
  /// In C#, the "last" clause in the following example corresponds to a <see cref="LastResultOperator"/>.
  /// <code>
  /// var query = (from s in Students
  ///              select s).Last();
  /// </code>
  /// </example>
  public class LastResultOperator : ResultOperatorBase, IQuerySource
  {
    private string _itemName;

    /// <summary>
    /// Initializes a new instance of the <see cref="LastResultOperator"/>.
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="returnDefaultWhenEmpty">The flag defines if a default expression should be regarded.</param>
    public LastResultOperator (string itemName, bool returnDefaultWhenEmpty)
        : base (returnDefaultWhenEmpty ? SingleExecutionStrategy.InstanceWithDefaultWhenEmpty : SingleExecutionStrategy.InstanceNoDefaultWhenEmpty)
    {
      ArgumentUtility.CheckNotNull ("itemName", itemName);
      
      _itemName = itemName;
      ReturnDefaultWhenEmpty = returnDefaultWhenEmpty;
    }

    public string ItemName
    {
      get { return _itemName; }
      set { _itemName = ArgumentUtility.CheckNotNullOrEmpty ("value", value); }
    }

    public bool ReturnDefaultWhenEmpty { get; set; }

    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      return new LastResultOperator (ItemName, ReturnDefaultWhenEmpty);
    }

    public override object ExecuteInMemory (object input)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      return InvokeGenericOnEnumerable<object> (input, ExecuteInMemory);
    }

    public T ExecuteInMemory<T> (IEnumerable<T> input)
    {
      ArgumentUtility.CheckNotNull ("input", input);

      if (ReturnDefaultWhenEmpty)
        return input.LastOrDefault();
      else
        return input.Last();
    }

    public override Type GetResultType (Type inputResultType)
    {
      ArgumentUtility.CheckNotNull ("inputResultType", inputResultType);
      return ReflectionUtility.GetItemTypeOfIEnumerable (inputResultType, "inputResultType");
    }

    public override string ToString ()
    {
      if (ReturnDefaultWhenEmpty)
        return "LastOrDefault()";
      else
        return "Last()";
    }
  }
}