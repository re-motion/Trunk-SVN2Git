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
using System.Security.Principal;
using Remotion.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IUserProvider"/> according to the "Null Object Pattern".
  /// </summary>
  public class NullUserProvider: ExtendedProviderBase, IUserProvider
  {
    private NullPrincipal _principal = new NullPrincipal();

    public NullUserProvider()
        : this ("Null", new NameValueCollection())
    {
    }

    public NullUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public IPrincipal GetUser()
    {
      return _principal;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}
