/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Linq;
using System.Linq.Expressions;

namespace Remotion.Data.Linq.UnitTests.TestQueryGenerators
{
  public static class FromTestQueryGenerator
  {
    public static IQueryable<Student> CreateMultiFromQuery (IQueryable<Student> source1, IQueryable<Student> source2)
    {
      return from s1 in source1 from s2 in source2 select s1;
    }

    public static IQueryable<Student> CreateThreeFromQuery (IQueryable<Student> source1, IQueryable<Student> source2, IQueryable<Student> source3)
    {
      return from s1 in source1 from s2 in source2 from s3 in source3 select s1;
    }

    public static MethodCallExpression CreateMultiFromQuery_SelectManyExpression (IQueryable<Student> source1, IQueryable<Student> source2)
    {
      IQueryable<Student> query = CreateMultiFromQuery (source1, source2);
      return (MethodCallExpression) query.Expression;
    }

    public static MethodCallExpression CreateThreeFromQuery_SelectManyExpression (IQueryable<Student> source1, IQueryable<Student> source2, IQueryable<Student> source3)
    {
      IQueryable<Student> query = CreateThreeFromQuery (source1, source2, source3);
      return (MethodCallExpression) query.Expression;
    }

  }
}
