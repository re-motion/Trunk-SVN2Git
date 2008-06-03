/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
namespace Remotion.Security
{
  /// <summary>Defines an adapter between the security layer and the business object implementation.</summary>
  /// <remarks>
  /// It is registered in the <see cref="AdapterRegistry"/> and is used for security checks 
  /// in implementations of <see cref="I:Remotion.ObjectBinding.IBusinessObjectProperty"/>.
  /// <note type="implementnotes">
  /// A typical implementation uses a <see cref="T:Remotion.Security.SecurityClient"/> that further dispatches to an 
  /// <see cref="IObjectSecurityStrategy"/> retrieved from the <see cref="ISecurableObject"/>.
  /// </note>
  /// </remarks>
  public interface IObjectSecurityAdapter : ISecurityAdapter
  {
    /// <summary>Determines whether read access to a property of <paramref name="securableObject"/> is granted.</summary>
    /// <param name="securableObject">The <see cref="ISecurableObject"/> whose permissions are checked.</param>
    /// <param name="propertyName">The property for which the permissions are checked.</param>
    /// <returns><see langword="true"/> when the property value can be retrieved.</returns>
    /// <remarks>If access is denied, the property is hidden in the UI.</remarks>
    bool HasAccessOnGetAccessor (ISecurableObject securableObject, string propertyName);

    /// <summary>Determines whether write access to a property of <paramref name="securableObject"/> is granted.</summary>
    /// <param name="securableObject">The <see cref="ISecurableObject"/> whose permissions are checked.</param>
    /// <param name="propertyName">The property for which the permissions are checked.</param>
    /// <returns><see langword="true"/> when the property can be changed.</returns>
    /// <remarks>If access is denied, the property is displayed as read-only in the UI.</remarks>
    bool HasAccessOnSetAccessor (ISecurableObject securableObject, string propertyName);
  }
}
