// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
  public interface IBusinessObjectPropertyPath
  {
    /// <summary> Gets the list of properties in this path. </summary>
    IBusinessObjectProperty[] Properties { get; }

    /// <summary> Gets the value of this property path for the specified object. </summary>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="throwExceptionIfNotReachable"> 
    ///   If <see langword="true"/>, an <see cref="InvalidOperationException"/> is thrown if any but the last property 
    ///   in the path is <see langword="null"/>. If <see langword="false"/>, <see langword="null"/> is returned instead. 
    /// </param>
    /// <param name="getFirstListEntry">
    ///   If <see langword="true"/>, the first value of each list property is processed.
    ///   If <see langword="false"/>, evaluation of list properties causes an <see cref="InvalidOperationException"/>.
    ///   (This does not apply to the last property in the path. If the last property is a list property, the return value is always a list.)
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if any but the last property in the path is <see langword="null"/>, or is not a single-value reference property. 
    /// </exception>
    object GetValue (IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry);

    /// <summary> Gets the string representation of the value of this property path for the specified object. </summary>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="format"> The format string passed to <see cref="IBusinessObject.GetPropertyString">IBusinessObject.GetPropertyString</see>. </param>
    string GetString (IBusinessObject obj, string format);

    /// <summary> Gets the <see cref="IBusinessObject"/> that is used to retrieve the value of the property path. </summary>
    /// <param name="obj"> The object that has the first property in the path. Must not be <see langword="null"/>. </param>
    /// <param name="throwExceptionIfNotReachable"> 
    ///   If <see langword="true"/>, an <see cref="InvalidOperationException"/> is thrown if the <see cref="IBusinessObject"/> cannot be reached 
    ///   because one of the properties in the path is <see langword="null"/>. If <see langword="false"/>, <see langword="null"/> is returned instead. 
    /// </param>
    /// <param name="getFirstListEntry">
    ///   If <see langword="true"/>, the first value of each list property is processed.
    ///   If <see langword="false"/>, evaluation of list properties causes an <see cref="InvalidOperationException"/>.
    /// </param>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if any but the last property in the path is <see langword="null"/>, or is not a single-value reference property. 
    /// </exception>
    IBusinessObject GetBusinessObject (IBusinessObject obj, bool throwExceptionIfNotReachable, bool getFirstListEntry);

    /// <summary> Sets the value of this property path for the specified object. </summary>
    /// <param name="obj">
    ///   The object that has the first property in the path. Must not be <see langword="null"/>. 
    /// </param>
    /// <param name="value"> The value to be assiged to the property. </param>
    /// <remarks> <b>SetValue</b> is not implemented in the current version. </remarks>
    /// <exception cref="NotImplementedException"> This method is not implemented. </exception>
    void SetValue (IBusinessObject obj, object value);

    /// <summary> Gets the string representation of this property path. </summary>
    string Identifier { get; }
  }
}
