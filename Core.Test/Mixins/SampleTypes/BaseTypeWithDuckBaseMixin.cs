using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses (typeof (DuckBaseMixin))]
  public class BaseTypeWithDuckBaseMixin
  {
    public virtual string MethodImplementedOnBase ()
    {
      return "BaseTypeWithDuckBaseMixin.MethodImplementedOnBase-" + ProtectedMethodImplementedOnBase();
    }

    protected virtual string ProtectedMethodImplementedOnBase ()
    {
      return "BaseTypeWithDuckBaseMixin.ProtectedMethodImplementedOnBase";
    }
  }
}
