using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.Linq.Parsing;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.Linq.UnitTests.TestQueryGenerators;

namespace Remotion.Data.Linq.UnitTests.ParsingTest.StructureTest.SelectManyExpressionParserTest
{
  [TestFixture]
  public class GeneralSelectManyExpressionParserTest
  {

    [Test]
    public void Initialize ()
    {
      IQueryable<Student> querySource1 = ExpressionHelper.CreateQuerySource();
      IQueryable<Student> querySource2 = ExpressionHelper.CreateQuerySource ();
      MethodCallExpression expression = FromTestQueryGenerator.CreateMultiFromQuery_SelectManyExpression (querySource1, querySource2);

      new SelectManyExpressionParser ().Parse (new ParseResultCollector (expression), expression);
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Expected one of 'SelectMany', but found 'Where' at TestQueryable<Student>()"
            +".Where(s => (s.Last = \"Garcia\")) in tree TestQueryable<Student>().Where(s => (s.Last = \"Garcia\")).")]
    public void Initialize_FromWrongExpression ()
    {
      MethodCallExpression expression = WhereTestQueryGenerator.CreateSimpleWhereQuery_WhereExpression (ExpressionHelper.CreateQuerySource ());
      new SelectManyExpressionParser ().Parse (new ParseResultCollector (expression), expression);
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Expected SelectMany call with three arguments for SelectMany expressions, "
        + "found Convert(null).SelectMany(student => null) (MethodCallExpression).")]
    public void Initialize_FromWrongExpressionInWhereExpression ()
    {
      Expression nonCallExpression = Expression.Convert (Expression.Constant (null), typeof (IQueryable<Student>));
      // Get method Queryable.SelectMany with two arguments whose second argument is of type Expression<Func<TSource, IEnumerable<TResult>>>
      // (rather than one of the many other overloads)
      MethodInfo method = (from m in typeof (Queryable).GetMethods () where m.Name == "SelectMany" && m.GetParameters().Length == 2 
          && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2 select m).First ();
      method = method.MakeGenericMethod (typeof (Student), typeof (Student));
      MethodCallExpression selectExpression = Expression.Call (method, nonCallExpression, Expression.Lambda (Expression.Constant (null, typeof (IEnumerable<Student>)), Expression.Parameter (typeof (Student), "student")));
      new SelectManyExpressionParser ().Parse (new ParseResultCollector (selectExpression), selectExpression);
    }
  }
}