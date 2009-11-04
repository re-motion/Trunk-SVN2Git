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
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Backend.DataObjectModel;
using Remotion.Data.Linq.Backend.DetailParsing;
using Remotion.Data.Linq.Backend.FieldResolving;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.UnitTests.Linq.TestDomain;

namespace Remotion.Data.UnitTests.Linq.Backend.DetailParsing
{
  public abstract class DetailParserTestBase
  {
    protected QueryModel QueryModel;
    protected ParseContext ParseContext;

    private MainFromClause _studentClause;
    private QuerySourceReferenceExpression _studentReference;

    private MemberExpression _student_First_Expression;
    private MemberExpression _student_Last_Expression;
    private MemberExpression _student_ID_Expression;

    public MainFromClause StudentClause
    {
      get { return _studentClause; }
      set { _studentClause = value; }
    }

    public QuerySourceReferenceExpression StudentReference
    {
      get { return _studentReference; }
    }

    public MemberExpression Student_First_Expression
    {
      get { return _student_First_Expression; }
    }

    public MemberExpression Student_Last_Expression
    {
      get { return _student_Last_Expression; }
    }

    public MemberExpression Student_ID_Expression
    {
      get { return _student_ID_Expression; }
    }

    [SetUp]
    public virtual void SetUp ()
    {
      QueryModel = ExpressionHelper.CreateQueryModel_Student ();
      ParseContext = new ParseContext(QueryModel, new List<FieldDescriptor>(), new JoinedTableContext (StubDatabaseInfo.Instance));

      _studentClause = ExpressionHelper.CreateMainFromClause_Student ();
      _studentReference = new QuerySourceReferenceExpression (_studentClause);
      _student_First_Expression = Expression.MakeMemberAccess (_studentReference, typeof (Student).GetProperty ("First"));
      _student_Last_Expression = Expression.MakeMemberAccess (_studentReference, typeof (Student).GetProperty ("Last"));
      _student_ID_Expression = Expression.MakeMemberAccess (_studentReference, typeof (Student).GetProperty ("ID"));
    }
  }
}
