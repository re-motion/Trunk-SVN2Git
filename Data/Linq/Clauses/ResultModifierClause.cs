/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  public class ResultModifierClause : IClause
  {
    public ResultModifierClause (SelectClause selectClause, MethodCallExpression resultModifier)
    {
      ArgumentUtility.CheckNotNull ("selectClause", selectClause);
      ArgumentUtility.CheckNotNull ("resultModifier", resultModifier);

      SelectClause = selectClause;
      ResultModifier = resultModifier;
    }

    public SelectClause SelectClause { get; private set; }
    public MethodCallExpression ResultModifier { get; private set; }


    public void Accept (IQueryVisitor visitor)
    {
      throw new System.NotImplementedException();
    }

    public IClause PreviousClause
    {
      get { throw new System.NotImplementedException(); }
    }
  }
}