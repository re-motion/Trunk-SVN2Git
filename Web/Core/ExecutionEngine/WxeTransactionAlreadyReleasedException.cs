using System;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Thrown when <see cref="WxeTransactionBase{TTransaction}.Execute(WxeContext)"/> is called after the transaction has already been released.
  /// This can happen, for example, if a transacted function's <see cref="WxeTransactedFunctionBase{TTransaction}.Execute(WxeContext)"/> method
  /// is called after execution has already finished once, or after the function has thrown an exception.
  /// </summary>
  [Serializable]
  public class WxeTransactionAlreadyReleasedException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="WxeTransactionAlreadyReleasedException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public WxeTransactionAlreadyReleasedException (string message)
        : base (message)
    {
    }
  }
}