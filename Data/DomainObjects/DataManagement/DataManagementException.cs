using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement
{
[Serializable]
public class DataManagementException : DomainObjectException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DataManagementException () : this ("A data management exception occurred.") {}
  public DataManagementException (string message) : base (message) {}
  public DataManagementException (string message, Exception inner) : base (message, inner) {}
  protected DataManagementException (SerializationInfo info, StreamingContext context) : base (info, context) {}

  // methods and properties

}
}
