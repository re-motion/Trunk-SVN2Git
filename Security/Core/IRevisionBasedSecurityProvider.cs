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
  /// <summary>
  /// Extends the <see cref="ISecurityProvider"/> interface to provide functionality for querying the revision of the 
  /// <see cref="IServiceProvider"/>'s data.
  /// </summary>
  /// <remarks>
  /// The revision feature is used in conjunction with the <see cref="RevisionBasedAccessTypeCacheProvider"/> to provide a second-level cache
  /// for access types which will be invalidated when the revision of the <see cref="IServiceProvider"/>'s data increments, indicating an update
  /// to the configured permissions.
  /// </remarks>
  public interface IRevisionBasedSecurityProvider : ISecurityProvider
  {
    /// <summary>Gets the current revison number.</summary>
    /// <returns>The current revison number.</returns>
    /// <remarks>
    /// The revison number is incremented when any permission-related information becomes outdated. 
    /// An incremented revision number indicates that the cache must be discared.
    /// </remarks>
    int GetRevision ();
  }
}