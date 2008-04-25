using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (DuckFaceMixin))]
  public class BaseTypeWithDuckFaceMixin
  {
    public virtual string MethodImplementedOnBase ()
    {
      return "BaseTypeWithDuckFaceMixin.MethodImplementedOnBase-" + ProtectedMethodImplementedOnBase ();
    }

    protected virtual string ProtectedMethodImplementedOnBase ()
    {
      return "BaseTypeWithDuckFaceMixin.ProtectedMethodImplementedOnBase";
    }
  }
}
