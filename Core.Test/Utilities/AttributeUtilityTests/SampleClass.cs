using System;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [Inherited]
  public class SampleClass
  {
    [Inherited]
    public virtual string PropertyWithSingleAttribute
    {
      get { return null; }
    }

    [Inherited]
    protected virtual string ProtectedPropertyWithAttribute
    {
      get { return null; }
    }

    [Multiple]
    public virtual string PropertyWithMultipleAttribute
    {
      get { return null; }
    }
  }
}