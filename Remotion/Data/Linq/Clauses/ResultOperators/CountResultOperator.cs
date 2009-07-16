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
using Remotion.Data.Linq.Parsing;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses.ResultOperators
{
  public class CountResultOperator : ResultOperatorBase, IQuerySource
  {
    private string _itemName;

    public CountResultOperator (string itemName)
        : base (ScalarExecutionStrategy.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemName", itemName);
      ItemName = itemName;
    }

    public string ItemName
    {
      get { return _itemName; }
      set { _itemName = ArgumentUtility.CheckNotNullOrEmpty ("value", value); }
    }

    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      return new CountResultOperator(ItemName);
    }

    public override object ExecuteInMemory (object input)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      return InvokeGenericOnEnumerable<int> (input, ExecuteInMemory);
    }

    public int ExecuteInMemory<T> (IEnumerable<T> input)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      return input.Count ();
    }

    public override Type GetResultType (Type inputResultType)
    {
      ArgumentUtility.CheckNotNull ("inputResultType", inputResultType);
      ReflectionUtility.GetItemTypeOfIEnumerable (inputResultType, "inputResultType"); // check whether inputResultType implements IEnumerable<T>

      return typeof (int);
    }

    public override string ToString ()
    {
      return "Count()";
    }
  }
}