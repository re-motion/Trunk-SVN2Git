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
using System.Web;
using Remotion.Configuration;
using Remotion.Security;

namespace Remotion.Web.Security
{
  public class HttpContextUserProvider : ExtendedProviderBase, IUserProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public HttpContextUserProvider()
        : this ("HttpContext", new NameValueCollection())
    {
    }

    public HttpContextUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }
    
     // methods and properties

    public IPrincipal GetUser()
    {
      if (HttpContext.Current == null)
        return null;
      else
        return HttpContext.Current.User;
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
