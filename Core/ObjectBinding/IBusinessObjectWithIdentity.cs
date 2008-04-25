namespace Remotion.ObjectBinding
{
  // TODO FS: Move to OB.Interfaces
  /// <summary> 
  /// <summary>
  ///   The <see cref="IBusinessObjectWithIdentity"/> interface provides functionality to uniquely identify a business object 
  ///   within its business object domain.
  /// </summary>
  /// <remarks>
  ///   With the help of the <see cref="IBusinessObjectWithIdentity"/> interface it is possible to persist and later restore 
  ///   a reference to the business object. 
  /// </remarks>
  public interface IBusinessObjectWithIdentity : IBusinessObject
  {
    /// <summary> Gets the programmatic <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/> </summary>
    /// <value> A <see cref="string"/> uniquely identifying this object. </value>
    /// <remarks> This value must be be unqiue within its business object domain. </remarks>
    string UniqueIdentifier { get; }
  }
}