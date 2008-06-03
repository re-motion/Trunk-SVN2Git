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
