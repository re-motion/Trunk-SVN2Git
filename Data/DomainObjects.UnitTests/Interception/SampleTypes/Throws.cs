using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public class Throws : DomainObject
  {
    public static Throws NewObject ()
    {
      return NewObject<Throws> ().With();
    }

    public Throws ()
      : base (ThrowException (), new StreamingContext())
    {
    }

    private static SerializationInfo ThrowException ()
    {
      throw new Exception ("Thrown in ThrowException()");
    }
  }
}