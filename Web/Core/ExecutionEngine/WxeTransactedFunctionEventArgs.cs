using System;
using Remotion.Data;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeTransactedFunctionEventArgs<TTransaction> : EventArgs
    where TTransaction : class, ITransaction
  {
    // types

    // static members

    // member fields

    private TTransaction _transaction;

    // construction and disposing



    public WxeTransactedFunctionEventArgs (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      _transaction = transaction;
    }

    // methods and properties

    public TTransaction Transaction
    {
      get { return _transaction; }
    }
  }
}