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
  /// <summary>Defines the signature for a factory method to create a <see cref="ISecurityContext"/> for a buiness object.</summary>
  /// <remarks><note type="implementnotes">Typically implemented by business objects (acting as their own <see cref="ISecurityContextFactory"/>).</note></remarks>
  public interface ISecurityContextFactory
  {
    /// <summary>Gets the <see cref="ISecurityContext"/> for a business object.</summary>
    /// <returns>The <see cref="ISecurityContext"/> for a business object.</returns>
    ISecurityContext CreateSecurityContext ();
  }
}
