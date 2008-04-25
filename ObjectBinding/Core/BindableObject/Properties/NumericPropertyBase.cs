using System;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public abstract class NumericPropertyBase : PropertyBase, IBusinessObjectNumericProperty
  {
    protected NumericPropertyBase (Parameters parameters)
        : base (parameters)
    {
    }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    public abstract Type Type { get; }

    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    public abstract bool AllowNegative { get; }
  }
}