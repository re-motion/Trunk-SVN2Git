// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
  /// <summary> 
  ///   The <b>IBusinessObjectReferenceProperty</b> interface is used for accessing references to other 
  ///   <see cref="IBusinessObject"/> instances.
  /// </summary>
  public interface IBusinessObjectReferenceProperty : IBusinessObjectProperty
  {
    /// <summary> Gets the class information for elements of this property. </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectClass"/> of the <see cref="IBusinessObject"/> accessed through this property.
    /// </value>
    IBusinessObjectClass ReferenceClass { get; }

    /// <summary>Gets a flag indicating whether it is possible to get a list of the objects that can be assigned to this property.</summary>
    /// <returns> <see langword="true"/> if it is possible to get the available objects from the object model. </returns>
    /// <remarks>Use the <see cref="SearchAvailableObjects"/> method (or an object model specific overload) to get the list of objects.</remarks>
    bool SupportsSearchAvailableObjects { get; }

    /// <summary>Searches the object model for the <see cref="IBusinessObject"/> instances that can be assigned to this property.</summary>
    /// <param name="referencingObject"> The business object for which to search for the possible objects to be referenced. </param>
    /// <param name="searchArguments">A parameter-object containing additional information for executing the search. Can be <see langword="null"/>.</param>
    /// <returns>A list of the <see cref="IBusinessObject"/> instances available. Must not return <see langword="null"/>.</returns>
    /// <exception cref="NotSupportedException">
    ///   Thrown if <see cref="SupportsSearchAvailableObjects"/> evaluated <see langword="false"/> but this method
    ///   has been called anyways.
    /// </exception>
    /// <remarks> 
    ///   This method is used if the seach statement is entered via the Visual Studio .NET designer, for instance in
    ///   the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValue"/> control.
    ///   <note type="inotes">
    ///     If your object model cannot evaluate a search string, but allows search through a less generic method,
    ///     provide an overload, and document that getting the list of available objects is only possible during runtime.
    ///   </note>
    /// </remarks>
    IBusinessObject[] SearchAvailableObjects (IBusinessObject referencingObject, ISearchAvailableObjectsArguments searchArguments);

    /// <summary>
    ///   Gets a flag indicating if <see cref="Create"/> may be called to implicitly create a new business object 
    ///   for editing in case the object reference is null.
    /// </summary>
    // SupportsDefaultValue
    bool CreateIfNull { get; }

    /// <summary>
    ///   If <see cref="CreateIfNull"/> is <see langword="true"/>, this method can be used to create a new business 
    ///   object.
    /// </summary>
    /// <param name="referencingObject"> 
    ///   The business object containing the reference property whose value will be assigned the newly created object. 
    /// </param>
    /// <exception cref="NotSupportedException"> 
    ///   Thrown if this method is called although <see cref="CreateIfNull"/> evaluated <see langword="false"/>. 
    /// </exception>
    /// <remarks>
    ///   A use case for the <b>Create</b> method is the instantiation of an business object without a unique identifier,
    ///   usually an <b>Aggregate</b>. The aggregate reference can be <see langword="null"/> until one of its values
    ///   is set in the user interface.
    /// </remarks>
    // CreateDefaultValue
    IBusinessObject Create (IBusinessObject referencingObject);

    //bool SupportsDelete

    // void Delete (IBusinessObject referencingObject, IBusinessObject value);
  }
}
