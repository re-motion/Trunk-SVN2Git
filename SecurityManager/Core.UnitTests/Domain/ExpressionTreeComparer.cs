using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Reflection;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  //TODO Move into Remotion.Development.Data.DomainObjects.Linq
  public static class ExpressionTreeComparer
  {
    public static void Compare<T> (IQueryable<T> expected, IQueryable<T> actual)
        where T: DomainObject
    {
      CommandData expectedCommandData = GetCommandData (expected);
      CommandData actualCommandData = GetCommandData (actual);

      Assert.That (actualCommandData.Statement, Is.EqualTo (expectedCommandData.Statement));
      Assert.That (actualCommandData.Parameters, Is.EqualTo (expectedCommandData.Parameters));
    }

    private static CommandData GetCommandData<T> (IQueryable<T> queryable)
    {
      QueryModel queryModel = MethodCaller.CallFunc<QueryModel> ("GenerateQueryExpression", BindingFlags.Instance | BindingFlags.NonPublic)
          .With ((QueryProviderBase) queryable.Provider, queryable.Expression);

      return ((QueryExecutor<T>) ((QueryProviderBase) queryable.Provider).Executor).CreateStatement (queryModel);
    }
  }
}