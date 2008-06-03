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
  /// <summary>The base interface for all business objects that need security features.</summary>
  public interface ISecurableObject
  {
    /// <summary>Gets the <see cref="IObjectSecurityStrategy"/> used by that business object.</summary>
    /// <remarks>Primarily used by a <see cref="T:Remotion.Security.SecurityClient"/> to dispatch security checks.</remarks>
    /// <returns>Returns the <see cref="IObjectSecurityStrategy"/>.</returns>
    IObjectSecurityStrategy GetSecurityStrategy ();

    /// <summary>Gets the <see cref="Type"/> representing the <see cref="ISecurableObject"/> in the security infrastructure.</summary>
    /// <returns>Return a <see cref="Type"/> object.</returns>
    Type GetSecurableType ();
  }
}
