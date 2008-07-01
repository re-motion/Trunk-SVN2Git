using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Utilities;

namespace Remotion.Data.Linq.QueryProviderImplementation
{
  public abstract class QueryableBase<T> : IOrderedQueryable<T>
  {
    private readonly QueryProviderBase _queryProvider;
    
    public QueryableBase (QueryProviderBase provider)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);

      _queryProvider = provider;
      Expression = Expression.Constant (this);
    }

    public QueryableBase (QueryProviderBase provider, Expression expression)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("expression", expression);

      if (!typeof (IEnumerable<T>).IsAssignableFrom (expression.Type))
        throw new ArgumentTypeException ("expression", typeof (IEnumerable<T>), expression.Type);

      _queryProvider = provider;
      Expression = expression;
    }

    public Expression Expression { get; private set; }

    public IQueryProvider Provider { get
    {
      return _queryProvider;
    }
    }

    public Type ElementType
    {
      get { return typeof (T); }
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _queryProvider.ExecuteCollection<T> (Expression).GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return _queryProvider.ExecuteCollection (Expression).GetEnumerator ();
    }
  }
}