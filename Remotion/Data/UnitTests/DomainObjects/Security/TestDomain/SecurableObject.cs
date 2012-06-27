// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.TestDomain
{
  [Instantiable]
  [DBTable]
  [Uses (typeof (SecurableObjectMixin))]
  public abstract class SecurableObject : DomainObject, ISecurableObject, ISecurityContextFactory
  {
    public static SecurableObject NewObject (ClientTransaction clientTransaction, IObjectSecurityStrategy securityStrategy)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return NewObject<SecurableObject>(ParamList.Create (securityStrategy));
      }
    }

    private IObjectSecurityStrategy _securityStrategy;

    protected SecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      _securityStrategy = new ObjectSecurityStrategy (this);
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    Type ISecurableObject.GetSecurableType ()
    {
      return GetPublicDomainObjectType();
    }

    public abstract string StringProperty { get; set; }

    public abstract string OtherStringProperty { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract SecurableObject Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<SecurableObject> Children { get; }

    [DBBidirectionalRelation ("OtherChildren")]
    public abstract SecurableObject OtherParent { get; set; }

    [DBBidirectionalRelation ("OtherParent")]
    public abstract ObjectList<SecurableObject> OtherChildren { get; }

    public abstract string PropertyWithDefaultPermission { get; set; }

    public abstract string PropertyWithCustomPermission { [DemandPermission (TestAccessTypes.First)]get; set; }

    protected abstract string NonPublicPropertyWithCustomPermission { [DemandPermission (TestAccessTypes.First)] get;  [DemandPermission (TestAccessTypes.Second)] set; }

    protected abstract SecurableObject NonPublicRelationPropertyWithCustomPermission { [DemandPermission (TestAccessTypes.First)] get; [DemandPermission (TestAccessTypes.Second)] set; }

    public abstract string PropertyWithMissingGetAccessor { set; }

    public abstract string PropertyWithMissingSetAccessor { get; }

    public abstract SecurableObject RelationPropertyWithMissingGetAccessor { set; }

    public abstract SecurableObject RelationPropertyWithMissingSetAccessor { get; }

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      return SecurityContext.CreateStateless(GetPublicDomainObjectType());
    }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
