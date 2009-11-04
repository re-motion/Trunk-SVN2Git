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
namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   The <b>IBusinessObjectClassWithIdentity</b> interface provides functionality for defining the <b>Class</b> of an 
  ///   <see cref="IBusinessObjectWithIdentity"/>. 
  /// </summary>
  /// <remarks>
  ///   The <b>IBusinessObjectClassWithIdentity</b> interface provides additional funcitonality utilizing the
  ///  <see cref="IBusinessObjectWithIdentity"/>' <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/>.
  /// </remarks>
  public interface IBusinessObjectClassWithIdentity: IBusinessObjectClass
  {
    /// <summary> 
    ///   Looks up and returns the <see cref="IBusinessObjectWithIdentity"/> identified by the 
    ///   <paramref name="uniqueIdentifier"/>.
    /// </summary>
    /// <param name="uniqueIdentifier"> 
    ///   A <see cref="string"/> representing the <b>ID</b> of an <see cref="IBusinessObjectWithIdentity"/>.
    /// </param>
    /// <returns> 
    ///   An <see cref="IBusinessObjectWithIdentity"/> or <see langword="null"/> if the specified object was not found. 
    /// </returns>
    IBusinessObjectWithIdentity GetObject (string uniqueIdentifier);
  }
}
