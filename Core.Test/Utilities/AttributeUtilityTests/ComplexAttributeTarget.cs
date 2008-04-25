using System;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  public class ComplexAttributeTarget
  {
    [Complex ()]
    public void DefaultCtor ()
    {
    }

    [Complex (S = "foo")]
    public void DefaultCtorWithProperty ()
    {
    }

    [Complex (T = typeof (object))]
    public void DefaultCtorWithField ()
    {
    }

    [Complex (typeof (void), S = "string")]
    public void CtorWithTypeAndProperty ()
    {
    }

    [Complex ("s", 1, 2, 3, "4")]
    public void CtorWithStringAndParamsArray ()
    {
    }

    [Complex (typeof (double), typeof (int), typeof (string))]
    public void CtorWithStringAndTypeParamsArray ()
    {
    }
  }
}