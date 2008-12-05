// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
