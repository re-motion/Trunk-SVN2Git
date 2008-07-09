/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Linq.Expressions;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Linq
{
  public class JoinClause
  {
    private readonly ParameterExpression _identifier;
    private readonly Expression _inExpression;
    private readonly Expression _onExpression;
    private readonly Expression _equalityExpression;
    private readonly ParameterExpression _intoIdentifier;

    public JoinClause (ParameterExpression identifier, Expression inExpression, Expression onExpression, Expression equalityExpression)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("inExpression", inExpression);
      ArgumentUtility.CheckNotNull ("onExpression", onExpression);
      ArgumentUtility.CheckNotNull ("equalityExpression", equalityExpression);

      _identifier = identifier;
      _inExpression = inExpression;
      _onExpression = onExpression;
      _equalityExpression = equalityExpression;
    }


    public JoinClause (ParameterExpression identifier, Expression inExpression, Expression onExpression, 
      Expression equalityExpression,ParameterExpression intoIdentifier)
    {
      ArgumentUtility.CheckNotNull ("identifier", identifier);
      ArgumentUtility.CheckNotNull ("inExpression", inExpression);
      ArgumentUtility.CheckNotNull ("onExpression", onExpression);
      ArgumentUtility.CheckNotNull ("equalityExpression", equalityExpression);
      ArgumentUtility.CheckNotNull ("intoIdentifier", intoIdentifier);

      _identifier = identifier;
      _inExpression = inExpression;
      _onExpression = onExpression;
      _equalityExpression = equalityExpression;
      _intoIdentifier = intoIdentifier;
    }


    public ParameterExpression Identifier
    {
      get { return _identifier; }
    }

    public Expression InExpression
    {
      get { return _inExpression; }
    }

    public Expression OnExpression
    {
      get { return _onExpression; }
    }

    public Expression EqualityExpression
    {
      get { return _equalityExpression; }
    }

    public ParameterExpression IntoIdentifier
    {
      get { return _intoIdentifier; }
    }
  }
}
