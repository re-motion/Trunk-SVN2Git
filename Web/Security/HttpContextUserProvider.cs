// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using Remotion.Configuration;
using Remotion.Security;

namespace Remotion.Web.Security
{
  public class HttpContextUserProvider : ExtendedProviderBase, IUserProvider
  {
    public HttpContextUserProvider ()
        : this ("HttpContext", new NameValueCollection())
    {
    }

    public HttpContextUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public ISecurityPrincipal GetUser ()
    {
      if (HttpContext.Current == null)
        return new NullSecurityPrincipal();

      IIdentity identity = HttpContext.Current.User.Identity;
      if (!identity.IsAuthenticated)
        return new NullSecurityPrincipal ();

      return new SecurityPrincipal (identity.Name, null, null, null);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}