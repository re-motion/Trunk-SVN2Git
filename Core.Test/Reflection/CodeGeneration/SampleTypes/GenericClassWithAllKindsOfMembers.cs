using System;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class GenericClassWithAllKindsOfMembers<T>
  {
    public virtual string Method (T t)
    {
      if (Event != null)
        Event ();
      return t.ToString ();
    }

    public virtual T Property
    {
      get { return default (T); }
    }

    public virtual event Func<T> Event;
  }
}