// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
  ///       You can use the <see langword="abstract"/> default implemtation (<see cref="T:Remotion.ObjectBinding.BusinessObjectProvider"/>) as a 
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

    /// <summary> Retrieves the requested <see cref="IBusinessObjectService"/>. </summary>
    T GetService<T> () where T: IBusinessObjectService;

    /// <summary> Adds the requested <see cref="IBusinessObjectService"/>. </summary>
    /// <param name="serviceType">The <see cref="Type"/> of <see cref="IBusinessObjectService"/> to get from the object model. Must not be <see langword="null" />.</param>
    /// <param name="service">An instance of the <see cref="IBusinessObjectService"/> specified by <paramref name="serviceType"/>. Must not be <see langword="null" />.</param>
    /// <remarks>
    ///   <note type="inotes">
    ///     If your object model does not support services, this method should throw a <see cref="NotSupportedException"/>.
    ///   </note>
    /// </remarks>
    void AddService (Type serviceType, IBusinessObjectService service);

    /// <summary> Registers a new <see cref="IBusinessObjectService"/> with this <see cref="IBusinessObjectProvider"/>. </summary>
    /// <param name="service"> The <see cref="IBusinessObjectService"/> to register. Must not be <see langword="null" />.</param>
    /// <typeparam name="T">The <see cref="Type"/> of the <paramref name="service"/> to be registered.</typeparam>
    /// <remarks>
    ///   <note type="inotes">
    ///     If your object model does not support services, this method should throw a <see cref="NotSupportedException"/>.
    ///   </note>
    /// </remarks>
    void AddService<T> (T service) where T : IBusinessObjectService;

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
