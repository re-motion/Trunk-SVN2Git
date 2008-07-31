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
    public delegate void AssertThatActualIsNull (object actual);
    public delegate void AssertThatActualIsNotNull (object actual);

    private readonly AssertThatActualIsEqualToExpected _assertThatActualIsEqualToExpected;
    private readonly AssertThatActualIsNull _assertThatActualIsNull;
    private readonly AssertThatActualIsNotNull _assertThatActualIsNotNull;

    public ExpressionTreeComparer (
        AssertThatActualIsEqualToExpected thatActualIsEqualToExpected,
        AssertThatActualIsNull assertThatActualIsNull,
        AssertThatActualIsNotNull assertThatActualIsNotNull)
    {
      ArgumentUtility.CheckNotNull ("thatActualIsEqualToExpected", thatActualIsEqualToExpected);
      ArgumentUtility.CheckNotNull ("assertThatActualIsNull", assertThatActualIsNull);
      ArgumentUtility.CheckNotNull ("assertThatActualIsNotNull", assertThatActualIsNotNull);

      _assertThatActualIsEqualToExpected = thatActualIsEqualToExpected;
      _assertThatActualIsNotNull = assertThatActualIsNotNull;
      _assertThatActualIsNull = assertThatActualIsNull;
    }

    public void Compare<T> (IQueryable<T> expected, IQueryable<T> actual)
        where T: DomainObject
    {
      if (expected == null)
      {
        _assertThatActualIsNull (actual);
        return;
      }

      _assertThatActualIsNotNull (actual);

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