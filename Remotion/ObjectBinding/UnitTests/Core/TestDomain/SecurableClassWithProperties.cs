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
using Remotion.Mixins;
using Remotion.Security;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  [BindableObject]
  [Uses (typeof (MixinSecurableClassWithProperties))]
  public class SecurableClassWithProperties : ISecurableObject
  {
    private readonly IObjectSecurityStrategy _securityStrategy;
    private string _readOnylProperty = string.Empty;
    private string _accessibleProperty = string.Empty;

    public SecurableClassWithProperties (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    public string PropertyWithDefaultPermission { get; set; }

    [DemandPropertyReadPermission (TestAccessTypes.First)]
    public string PropertyWithReadPermission
    {
      get { return _readOnylProperty; }
      set { _readOnylProperty = value; }
    }

    [DemandPropertyWritePermission (TestAccessTypes.Second)]
    public string PropertyWithWritePermission
    {
      get { return _accessibleProperty; }
      set { _accessibleProperty = value; }
    }

    [DemandPropertyReadPermission (TestAccessTypes.First)]
    public virtual string  PropertyToOverrideWithReadPermission
    {
      get { return _accessibleProperty; }
      set { _accessibleProperty = value; }
    }

    [DemandPropertyWritePermission (TestAccessTypes.First)]
    public virtual string PropertyToOverrideWithWritePermission
    {
      get { return _accessibleProperty; }
      set { _accessibleProperty = value; }
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetType();
    }
  }
}