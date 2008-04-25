using System;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public class ClassWithAllKindsOfMembers
  {
    public virtual void Method ()
    {
      if (Event != null)
        Event (null, null);
    }

    public virtual void MethodWithOutRef (out string outP, ref int refP)
    {
      outP = refP.ToString ();
      ++refP;
    }

    public virtual int Property
    {
      get { return 0; }
      set { Dev.Null = value; }
    }

    public virtual string this[int i]
    {
      get { return i.ToString(); }
      set { Dev.Null = value; }
    }

    public virtual event EventHandler Event;
  }
}