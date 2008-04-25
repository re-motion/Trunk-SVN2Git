using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
  public class AttributeWithParameters2 : Attribute
  {
    public int Field;

    private int _property;
    private int _ctor;

    public AttributeWithParameters2 (int ctor)
    {
      _ctor = ctor;
    }

    public AttributeWithParameters2 (int ctor, string dummy)
      : this (ctor)
    {
    }

    public int Property
    {
      get { return _property; }
      set { _property = value; }
    }
  }
}