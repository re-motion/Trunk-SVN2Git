/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.ObjectBinding
{
  /// <summary> 
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
