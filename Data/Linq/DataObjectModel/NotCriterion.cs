/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;

namespace Remotion.Data.Linq.DataObjectModel
{
  public struct NotCriterion : ICriterion
  {
    public readonly ICriterion NegatedCriterion;

    public NotCriterion (ICriterion negatedCriterion)
    {
      ArgumentUtility.CheckNotNull ("negatedCriterion", negatedCriterion);
      NegatedCriterion = negatedCriterion;
    }


    public override string ToString ()
    {
      return "NOT (" + NegatedCriterion + ")";
    }

    public void Accept (IEvaluationVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.VisitNotCriterion (this);
    }

  }
}
