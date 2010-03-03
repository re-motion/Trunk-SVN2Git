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

namespace Remotion.Data.Linq.SqlBackend.SqlStatementModel.Unresolved
{
  /// <summary>
  /// <see cref="ConstantTableSource"/> holds a <see cref="ConstantExpression"/> representing the data source defined by a LINQ query.
  /// </summary>
  public class ConstantTableSource : AbstractTableSource
  {
    public ConstantTableSource (ConstantExpression constantExpression)
    {
      ArgumentUtility.CheckNotNull ("constantExpression", constantExpression);

      ConstantExpression = constantExpression;
    }

    public ConstantExpression ConstantExpression { get; private set; }
    
    public override Type Type // TODO: Rename to "ItemType", initialize via ctor (from initial FromClause.ItemType)
    {
      get { return ConstantExpression.Type;  }
    }

    public override AbstractTableSource Accept (ITableSourceVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      return visitor.VisitConstantTableSource (this);
    }
  }
}