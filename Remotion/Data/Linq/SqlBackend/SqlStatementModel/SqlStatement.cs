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
using System.Linq.Expressions;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlStatementModel
{
  /// <summary>
  /// <see cref="SqlStatement"/> represents a SQL database query. The <see cref="QueryModel"/> is translated to this model, and the 
  /// <see cref="SqlStatement"/> is transformed several times until it can easily be translated to SQL text.
  /// </summary>
  public class SqlStatement
  {
    private readonly SqlTable _fromExpression;

    private Expression _selectProjection;
    private Expression _whereCondition;

    public SqlStatement (Expression selectProjection, SqlTable fromExpression)
    {
      ArgumentUtility.CheckNotNull ("selectProjection", selectProjection);
      ArgumentUtility.CheckNotNull ("fromExpression", fromExpression);

      _selectProjection = selectProjection;
      _fromExpression = fromExpression;
    }

    public bool IsCountQuery { get; set; }
    public bool IsDistinctQuery { get; set; }
    
    public Expression TopExpression { get; set;}
    
    public Expression SelectProjection
    {
      get { return _selectProjection; }
      set { _selectProjection = ArgumentUtility.CheckNotNull ("value", value); }
    }

    public SqlTable FromExpression
    {
      get { return _fromExpression; }
    }

    public Expression WhereCondition
    {
      get { return _whereCondition; }
      set {
        if (value != null)
        {
          if (value.Type != typeof (bool))
            throw new ArgumentTypeException ("whereCondition", typeof (bool), value.Type);
          _whereCondition = value;
        }
      }
    }
  }
}