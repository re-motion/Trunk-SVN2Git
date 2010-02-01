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
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  public static class ExpressionHelper
  {
    private static readonly IQueryExecutor s_executor = CreateExecutor();

    public static IQueryable<Student> CreateStudentQueryable ()
    {
      return CreateStudentQueryable (s_executor);
    }

    public static IQueryable<Student> CreateStudentQueryable (IQueryExecutor executor)
    {
      return new TestQueryable<Student> (executor);
    }

    public static IQueryable<Student_Detail> CreateStudentDetailQueryable ()
    {
      return CreateStudentDetailQueryable (s_executor);
    }

    public static IQueryable<Student_Detail> CreateStudentDetailQueryable (IQueryExecutor executor)
    {
      return new TestQueryable<Student_Detail> (executor);
    }

    public static Expression MakeExpression<TRet> (Expression<Func<TRet>> expression)
    {
      return expression.Body;
    }

    public static IQueryExecutor CreateExecutor ()
    {
      var repository = new MockRepository ();
      var executor = repository.StrictMock<IQueryExecutor> ();
      return executor;
    }
  }
}
