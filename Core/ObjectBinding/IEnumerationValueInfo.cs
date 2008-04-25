using System;

namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.Interfaces
  /// <summary>
  ///   The <b>IEnumerationValueInfo"</b> interface provides fucntionality for encapsulating a native enumeration value 
  ///   for use with an <see cref="IBusinessObjectEnumerationProperty"/>.
  /// </summary>
  /// <remarks> 
  ///   For enumerations of the <see cref="Enum"/> type, the generic <see cref="EnumerationValueInfo"/> class can be 
  ///   used.
  ///  </remarks>
  public interface IEnumerationValueInfo
  {
    /// <summary> Gets the object representing the original value, e.g. a System.Enum type. </summary>
    /// <value> The encapsulated enumeration value. </value>
    object Value { get; }

    /// <summary> Gets the string identifier representing the value. </summary>
    /// <value> The encapsulated enumeration value's string representation. </value>
    string Identifier { get; }

    /// <summary> Gets the string presented to the user. </summary>
    /// <value> The human readable value of the encapsulated enumeration value. </value>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    string DisplayName { get; }

    /// <summary>
    ///   Gets a flag indicating whether this value should be presented as an option to the user. 
    ///   (If not, existing objects might still use this value.)
    /// </summary>
    /// <value> <see langword="true"/> if this enumeration value sould be presented as an option to the user. </value>
    bool IsEnabled { get; }
  }
}