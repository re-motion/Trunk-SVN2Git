namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.Interfaces
  /// <summary> 
  ///   The <b>IBusinessObjectStringProperty</b> provides additional meta data for <see cref="string"/> values.
  /// </summary>
  public interface IBusinessObjectStringProperty: IBusinessObjectProperty
  {
    /// <summary>
    ///   Getsthe the maximum length of a string assigned to the property, or <see langword="null"/> if no maximum length is defined.
    /// </summary>
    int? MaxLength { get; }
  }
}