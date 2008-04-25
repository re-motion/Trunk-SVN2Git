using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement
{
[Serializable]
public class ClientTransactionsDifferException : DataManagementException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ClientTransactionsDifferException () : this ("ClientTransactions differ.") {}
  public ClientTransactionsDifferException (string message) : base (message) {}
  public ClientTransactionsDifferException (string message, Exception inner) : base (message, inner) {}
  protected ClientTransactionsDifferException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
