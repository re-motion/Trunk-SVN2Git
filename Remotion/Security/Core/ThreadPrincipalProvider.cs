// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading;
using Remotion.Configuration;

namespace Remotion.Security
{
  public class ThreadPrincipalProvider : ExtendedProviderBase, IPrincipalProvider
  {
    public ThreadPrincipalProvider ()
        : this ("Thread", new NameValueCollection())
    {
    }

    public ThreadPrincipalProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public ISecurityPrincipal GetPrincipal ()
    {
      IIdentity identity = Thread.CurrentPrincipal.Identity;
      if (!identity.IsAuthenticated)
        return new NullSecurityPrincipal();

      return new SecurityPrincipal (identity.Name, null, null, null);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
