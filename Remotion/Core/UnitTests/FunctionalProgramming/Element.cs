using System;

namespace Remotion.UnitTests.FunctionalProgramming
{
  internal class Element
  {
    private readonly int _value;
    public readonly Element Parent;

    public Element (int value, Element parent)
    {
      _value = value;
      Parent = parent;
    }

    public override string ToString ()
    {
      return _value.ToString();
    }
  }
}