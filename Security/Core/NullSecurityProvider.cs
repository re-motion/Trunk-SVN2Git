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
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Security.Principal;
using Remotion.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Provides an implementation of a nullable object according to the "Null Object Pattern", 
  /// extending <see cref="ProviderBase"/> and implementing <see cref="ISecurityProvider"/>.
  /// </summary>
  public class NullSecurityProvider : ExtendedProviderBase, ISecurityProvider
  {
    public NullSecurityProvider ()
      : this ("Null", new NameValueCollection ())
    {
    }

    public NullSecurityProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    /// <summary>
    /// The "Null Object" implementation always returns an empty array.
    /// </summary>
    /// <returns>Always returns an empty array.</returns>
    public AccessType[] GetAccess (ISecurityContext context, IPrincipal user)
    {
      return new AccessType[0];
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
