using System;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class ByteProperty : NumericPropertyBase
  {
    public ByteProperty (Parameters parameters)
        : base (parameters)
    {
    }

    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    public override bool AllowNegative
    {
      get { return false; }
    }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    public override Type Type
    {
      get { return typeof (Byte); }
    }
  }
}