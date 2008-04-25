using System;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class StringProperty : PropertyBase, IBusinessObjectStringProperty
  {
    private readonly int? _maxLength;


    public StringProperty (Parameters parameters, int? maxLength)
        : base (parameters)
    {
      _maxLength = maxLength;
    }

    /// <summary>
    ///   Getsthe the maximum length of a string assigned to the property, or <see langword="null"/> if no maximum length is defined.
    /// </summary>
    public int? MaxLength
    {
      get { return _maxLength; }
    }
  }
}