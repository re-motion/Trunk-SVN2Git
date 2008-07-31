using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Linq
{
  /// <summary>
  /// The <see cref="ExpressionTreeComparer"/> compares two expressions for as to wether they result in the same SQL statement and parameters.
  /// Use this comparer to unit test Linq expressions and manually build <see cref="Expression"/> trees.
  /// </summary>
  public class ExpressionTreeComparer
  {
    public delegate void AssertThatActualIsEqualToExpected (object actual, object expected);

    private readonly AssertThatActualIsEqualToExpected _assertThatActualIsEqualToExpected;

    public ExpressionTreeComparer (AssertThatActualIsEqualToExpected thatActualIsEqualToExpected)
    {
      ArgumentUtility.CheckNotNull ("thatActualIsEqualToExpected", thatActualIsEqualToExpected);

      _assertThatActualIsEqualToExpected = thatActualIsEqualToExpected;
    }

    public void Compare<T> (IQueryable<T> expected, IQueryable<T> actual)
        where T: DomainObject
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      CommandData expectedCommandData = GetCommandData (expected);
      CommandData actualCommandData = GetCommandData (actual);

      _assertThatActualIsEqualToExpected (actualCommandData.Statement, expectedCommandData.Statement);
      _assertThatActualIsEqualToExpected (actualCommandData.Parameters, expectedCommandData.Parameters);
    }

    private CommandData GetCommandData<T> (IQueryable<T> queryable)
    {
      QueryModel queryModel = MethodCaller.CallFunc<QueryModel> ("GenerateQueryExpression", BindingFlags.Instance | BindingFlags.NonPublic)
          .With ((QueryProviderBase) queryable.Provider, queryable.Expression);

      return ((QueryExecutor<T>) ((QueryProviderBase) queryable.Provider).Executor).CreateStatement (queryModel);
    }
  }
}