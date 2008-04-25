using System;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class DateProperty : DateTimePropertyBase
  {
    public DateProperty (Parameters parameters)
        : base (parameters)
    {
    }

    public override DateTimeType Type
    {
      get { return DateTimeType.Date; }
    }
  }
}