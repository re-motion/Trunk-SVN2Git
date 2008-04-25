using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Web.Test
{
[Serializable]
public class TestFailureException : ApplicationException
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public TestFailureException () : this ("A test failed.") 
  {
  }

  public TestFailureException (string message) : base (message) 
  {
  }
  
  public TestFailureException (string message, Exception inner) : base (message, inner) 
  {
  }

  protected TestFailureException (SerializationInfo info, StreamingContext context) : base (info, context) 
  {
  }

  // methods and properties

}
}
