using System;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public abstract class DateTimePropertyBase : PropertyBase, IBusinessObjectDateTimeProperty
  {
    protected DateTimePropertyBase (Parameters parameters)
        : base (parameters)
    {
    }

    public abstract DateTimeType Type { get; }
  }
}