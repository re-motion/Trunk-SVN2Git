using System;

namespace Remotion.Data
{
  /// <summary>
  /// The <see cref="ITransactionFactory"/> interface defines a factory method for creating root transactions in 
  /// user interface application such as a web application using the Execution Engine flow control infrastructure.
  /// </summary>
  public interface ITransactionFactory
  {
    /// <summary>
    /// Creates a new root transaction instance. This instance is not yet managed by a scope.
    /// </summary>
    /// <returns>A new root transaction.</returns>
    ITransaction CreateRootTransaction ();
  }
}