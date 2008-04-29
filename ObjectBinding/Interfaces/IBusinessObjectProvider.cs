using System;

namespace Remotion.ObjectBinding
{
  /// <summary> Provides functionality for business object providers. </summary>
  /// <remarks>
  ///   A business object provider is able to retrieve services (e.g. the
  ///   <see cref="T:Remotion.ObjectBinding.Web.IBusinessObjectWebUIService"/>) from the object model, as well as provide
  ///   functionality required by more than one of the business object components (<b>Class</b>, <b>Property</b>, and
  ///   <b>Object</b>).
  ///   <note type="inotes">
  ///     <para>
  ///       If this interface is implemented using singletons, the singleton must be thread save.
  ///     </para><para>
  ///       You can use the <see langword="abstract"/> default implemtation (<see cref="BusinessObjectProvider"/>) as a 
  ///       base for implementing the business object provider for your object model.
  ///     </para>
  ///   </note>
  /// </remarks>
  public interface IBusinessObjectProvider
  {
    /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. </summary>
    /// <param name="serviceType">The type of <see cref="IBusinessObjectService"/> to get from the object model. Must not be <see langword="null" />.</param>
    /// <returns> 
    ///   An instance if the <see cref="IBusinessObjectService"/> type or <see langword="null"/> if the sevice could not be found or instantiated.
    ///  </returns>
    ///  <remarks>
    ///    <note type="inotes">
    ///     If your object model does not support services, this method may always return null.
    ///    </note>
    ///  </remarks>
    IBusinessObjectService GetService (Type serviceType);

    /// <summary>Returns the <see cref="Char"/> to be used as a serparator when formatting the property path's identifier.</summary>
    /// <returns> A <see cref="Char"/> that is not used by the property's identifier. </returns>
    char GetPropertyPathSeparator ();

    /// <summary>Creates a <see cref="IBusinessObjectPropertyPath"/> from the passed <see cref="IBusinessObjectProperty"/> list.</summary>
    /// <param name="properties"> An array of <see cref="IBusinessObjectProperty"/> instances. </param>
    /// <returns> A new instance of the <see cref="IBusinessObjectPropertyPath"/> type. </returns>
    IBusinessObjectPropertyPath CreatePropertyPath (IBusinessObjectProperty[] properties);

    /// <summary> Returns a <see cref="String"/> to be used instead of the actual value if the property is not accessible. </summary>
    /// <returns> A <see cref="String"/> that can be easily distinguished from typical property values. </returns>
    string GetNotAccessiblePropertyStringPlaceHolder ();
  }
}