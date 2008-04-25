using System;

namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.Interfaces
  /// <summary> The <b>IBusinessObjectDateTimeProperty</b> interface is used for accessing <see cref="DateTime"/> values. </summary>
  /// <remarks>
  /// <note type="inotes">
  /// The objects returned for this property must implement the <see cref="IFormattable"/> interface in order to be displayed by the 
  /// <see cref="IBusinessObject.GetPropertyString"/> methods.
  /// </note>
  /// </remarks>
  public interface IBusinessObjectDateTimeProperty : IBusinessObjectProperty
  {
    DateTimeType Type { get; }
  }

  /// <summary>
  /// The <see cref="DateTimeType"/> enum defines the list of possible data types supported by the <see cref="IBusinessObjectDateTimeProperty"/>.
  /// </summary>
  public enum DateTimeType
  {
    DateTime,
    Date
  }
}