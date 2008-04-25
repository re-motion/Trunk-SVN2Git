using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class AttributeWithParameters : Attribute
  {
    public int Field;

    private int _property;
    private int _ctor;

    public AttributeWithParameters (int ctor)
    {
      _ctor = ctor;
    }

    public AttributeWithParameters (int ctor, string dummy) : this (ctor)
    {
    }

    public int Property
    {
      get { return _property; }
      set { _property = value; }
    }
  }
}