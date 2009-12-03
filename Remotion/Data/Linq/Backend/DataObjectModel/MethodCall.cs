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
using System.Linq;
using System.Reflection;
using Remotion.Data.Linq.Utilities;
using Remotion.Text;

namespace Remotion.Data.Linq.Backend.DataObjectModel
{
  public class MethodCall : IEvaluation, ICriterion
  {
    public MethodCall (MethodInfo evaluationMethodInfo, IEvaluation evaluationObject, List<IEvaluation> evaluationArguments)
    {
      ArgumentUtility.CheckNotNull ("evaluationMethodInfo", evaluationMethodInfo);
      ArgumentUtility.CheckNotNull ("evaluationArguments", evaluationArguments);

      EvaluationMethodInfo = evaluationMethodInfo;
      TargetObject = evaluationObject;
      Arguments = evaluationArguments;
    }

    public MethodInfo EvaluationMethodInfo { get; private set; }
    public IEvaluation TargetObject { get; private set; }
    public List<IEvaluation> Arguments { get; private set; }

    public void Accept (IEvaluationVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitMethodCall (this);
    }

    public override string ToString ()
    {
      string argumentString = SeparatedStringBuilder.Build (", ", Arguments);
      if (TargetObject != null)
        return string.Format ("{0}.{1}({2})", TargetObject, EvaluationMethodInfo.Name, argumentString);
      else
        return string.Format ("{0}({1})", EvaluationMethodInfo.Name, argumentString);
    }

    public override bool Equals (object obj)
    {
      MethodCall other = obj as MethodCall;
      return other != null
             && Equals (EvaluationMethodInfo, other.EvaluationMethodInfo)
             && Equals (TargetObject, other.TargetObject)
             && Arguments.SequenceEqual (other.Arguments);
    }

    public override int GetHashCode ()
    {
      return Remotion.Utilities.EqualityUtility.GetRotatedHashCode (EvaluationMethodInfo, TargetObject, Remotion.Utilities.EqualityUtility.GetRotatedHashCode (Arguments));
    }
  }
}
