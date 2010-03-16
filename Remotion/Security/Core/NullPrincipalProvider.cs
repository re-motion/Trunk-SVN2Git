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
using Remotion.Configuration;

namespace Remotion.Security
{
  /// <summary>
  /// Represents a nullable <see cref="IPrincipalProvider"/> according to the "Null Object Pattern".
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class NullPrincipalProvider : ExtendedProviderBase, IPrincipalProvider
  {
    private readonly NullSecurityPrincipal _securityPrincipal = new NullSecurityPrincipal();

    public NullPrincipalProvider ()
        : this ("Null", new NameValueCollection())
    {
    }

    public NullPrincipalProvider (string name, NameValueCollection config)
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
