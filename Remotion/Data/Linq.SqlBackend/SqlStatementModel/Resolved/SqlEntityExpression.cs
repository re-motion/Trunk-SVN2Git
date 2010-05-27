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
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Utilities;

namespace Remotion.Data.Linq.SqlBackend.SqlStatementModel.Resolved
{
  /// <summary>
  /// <see cref="SqlEntityExpression"/> represents an entity in a SQL expression. It consists of a list of columns, a primary key (which is usually
  /// part of the columns list), and a table alias identifying the table or substatement the entity stems from. An entity can have a name, which
  /// is used to prefix all of its columns with in the generated SQL. 
  /// </summary>
  public abstract class SqlEntityExpression : ExtensionExpression
  {
    private readonly string _tableAlias;
    private readonly string _name;

    protected SqlEntityExpression (Type entityType, string tableAlias, string entityName)
      : base (ArgumentUtility.CheckNotNull ("entityType", entityType))
    {
      ArgumentUtility.CheckNotNull ("tableAlias", tableAlias);
      
      _tableAlias = tableAlias;
      _name = entityName;
    }

    public abstract SqlColumnExpression PrimaryKeyColumn { get; }
    public abstract ReadOnlyCollection<SqlColumnExpression> Columns { get; }

    public string TableAlias
    {
      get { return _tableAlias; }
    }

    public string Name
    {
      get { return _name; }
    }

    public abstract SqlColumnExpression GetColumn (Type type, string columnName, bool isPrimaryKeyColumn);
    public abstract SqlEntityExpression CreateReference (string newTableAlias);
    
    public abstract SqlEntityExpression Update (Type itemType, string tableAlias, string entityName);
    
    public override Expression Accept (ExpressionTreeVisitor visitor)
    {
      var specificVisitor = visitor as IResolvedSqlExpressionVisitor;
      if (specificVisitor != null)
        return specificVisitor.VisitSqlEntityExpression (this);
      else
        return base.Accept (visitor);
    }
  }
}