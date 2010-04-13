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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  [BindableDomainObject]
  public abstract class ObjectWithSecurity : DomainObject, ISecurableObject, IDomainObjectSecurityContextFactory
  {
    public static ObjectWithSecurity NewObject ()
    {
      return NewObject<ObjectWithSecurity>();
    }

    protected ObjectWithSecurity ()
    {
    }

    public abstract string TheProperty { get; set; }

    IObjectSecurityStrategy ISecurableObject.GetSecurityStrategy ()
    {
      return new DomainObjectSecurityStrategy (RequiredSecurityForStates.NewAndDeleted, this);
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      return SecurityContext.Create (GetPublicDomainObjectType (), null, null, null, new Dictionary<string, EnumWrapper> (), new EnumWrapper[0]);
    }

    public bool IsNew
    {
      get { return State == StateType.New; }
    }

    public bool IsDeleted
    {
      get { return State == StateType.Deleted; }
    }
  }
}