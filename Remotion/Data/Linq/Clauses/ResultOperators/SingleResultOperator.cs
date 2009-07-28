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
using System.Linq;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses.ResultOperators
{
  /// <summary>
  /// Represents the single part of a query. This is a result operator, operating on the whole result set of a query.
  /// </summary>
  /// <example>
  /// In C#, the "single" clause in the following example corresponds to a <see cref="SingleResultOperator"/>.
  /// <code>
  /// var query = (from s in Students
  ///              select s).Single();
  /// </code>
  /// </example>
  public class SingleResultOperator : ChoiceResultOperatorBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SingleResultOperator"/>.
    /// </summary>
    /// <param name="returnDefaultWhenEmpty">The flag defines if a default expression should be regarded.</param>
    public SingleResultOperator (bool returnDefaultWhenEmpty)
      : base (returnDefaultWhenEmpty)
    {
      ReturnDefaultWhenEmpty = returnDefaultWhenEmpty;
    }

    public bool ReturnDefaultWhenEmpty { get; set; }

    public override ResultOperatorBase Clone (CloneContext cloneContext)
    {
      return new SingleResultOperator (ReturnDefaultWhenEmpty);
    }

    public override StreamedValue ExecuteInMemory<T> (StreamedSequence input)
    {
      var sequence = input.GetTypedSequence<T> ();
      if (ReturnDefaultWhenEmpty)
      {
        var result = sequence.SingleOrDefault();
        return new StreamedValue (result);
      }
      else
      {
        var result = sequence.Single();
        return new StreamedValue (result);
      }
    }

    public override Type GetResultType (Type inputResultType)
    {
      ArgumentUtility.CheckNotNull ("inputResultType", inputResultType);
      return ReflectionUtility.GetItemTypeOfIEnumerable (inputResultType, "inputResultType");
    }

    public override string ToString ()
    {
      if (ReturnDefaultWhenEmpty)
        return "SingleOrDefault()";
      else
        return "Single()";
    }
  }
}