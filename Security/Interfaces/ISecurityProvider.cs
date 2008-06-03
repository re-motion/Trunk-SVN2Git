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
using System.Security.Principal;

namespace Remotion.Security
{
  /// <summary>Provides access to the permission management functionality.</summary>
  /// <remarks>This service interface enables a plugable security system architecture, acting as single point of access to the permission management functionality.</remarks>
  public interface ISecurityProvider : INullObject
  {
    /// <summary>Determines permission for a user.</summary>
    /// <param name="context">The <see cref="ISecurityContext"/> gouping all object-specific security information of the current permission check.</param>
    /// <param name="user">The <see cref="IPrincipal"/> on whose behalf the permissions are evaluated.</param>
    /// <returns></returns>
    AccessType[] GetAccess (ISecurityContext context, IPrincipal user);

    /// <summary>Gets the current revison number.</summary>
    /// <returns>The current revison number.</returns>
    /// <remarks>The revison number is incremented when any permission-related information becomes outdated; an incremented revision number indicates that the cache must be discared.</remarks>
    int GetRevision ();
  }
}
