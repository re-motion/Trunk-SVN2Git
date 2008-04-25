using System;

namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.Interfaces
  //TODO: Type property for concrete numeric type
  /// <summary>The <see cref="IBusinessObjectNumericProperty"/> interface provides additional meta data for numeric values.</summary>
  /// <remarks>
  /// <note type="inotes">
  /// The objects returned for this property must implement the <see cref="IFormattable"/> interface in order to be displayed by the 
  /// <see cref="IBusinessObject.GetPropertyString"/> methods.
  /// </note>
  /// </remarks>
  public interface IBusinessObjectNumericProperty : IBusinessObjectProperty
  {
    /// <summary> Gets a flag specifying whether negative numbers are valid for the property. </summary>
    /// <value> <see langword="true"/> if this property can be assigned a negative value. </value>
    bool AllowNegative { get; }

    /// <summary>Gets the numeric type associated with this <see cref="IBusinessObjectNumericProperty"/>.</summary>
    Type Type { get; }
  }
}