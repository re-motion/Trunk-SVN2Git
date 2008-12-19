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
using Remotion.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IPrincipalProvider"/> according to the "Null Object Pattern".
  /// </summary>
  public class PrincipalUserProvider : ExtendedProviderBase, IPrincipalProvider
  {
    private readonly NullSecurityPrincipal _securityPrincipal = new NullSecurityPrincipal();

    public PrincipalUserProvider ()
        : this ("Null", new NameValueCollection())
    {
    }

    public PrincipalUserProvider (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public ISecurityPrincipal GetPrincipal ()
    {
      return _securityPrincipal;
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }
  }
}