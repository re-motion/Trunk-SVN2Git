using System;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class DateTimeProperty : DateTimePropertyBase
  {
    public DateTimeProperty (Parameters parameters)
        : base (parameters)
    {
    }

    public override DateTimeType Type
    {
      get { return DateTimeType.DateTime; }
    }
  }
}