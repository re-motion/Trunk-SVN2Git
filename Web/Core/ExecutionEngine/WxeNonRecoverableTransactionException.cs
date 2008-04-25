using System;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeNonRecoverableTransactionException : Exception
  {
    private static string CreateMessage (Exception originalException, Exception transactionException)
    {
      return string.Format ("An exception of type {0} caused a non-recoverable {1}.{2}"
          + "The original exception message was: '{3}'{2}"
          + "The transaction error message was: '{4}'",
          originalException.GetType ().Name, transactionException.GetType ().Name, Environment.NewLine,
          originalException.Message, transactionException.Message);
    }

    public readonly Exception TransactionException;

    public WxeNonRecoverableTransactionException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    public WxeNonRecoverableTransactionException (Exception originalException, Exception transactionException)
        : this (
            CreateMessage (ArgumentUtility.CheckNotNull ("originalException", originalException),
                ArgumentUtility.CheckNotNull ("transactionException", transactionException)),
            originalException)
    {
      TransactionException = transactionException;
    }
  }
}