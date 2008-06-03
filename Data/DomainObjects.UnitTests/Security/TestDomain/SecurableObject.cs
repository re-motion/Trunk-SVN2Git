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
using Remotion.Development.UnitTesting;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.UnitTests.Security.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class SecurableObject : DomainObject, ISecurableObject, ISecurityContextFactory
  {
    public static SecurableObject NewObject (ClientTransaction clientTransaction, IObjectSecurityStrategy securityStrategy)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return NewObject<SecurableObject>().With (securityStrategy);
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

    public DataContainer GetDataContainer ()
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (this, typeof (DomainObject), "GetDataContainerForTransaction", ClientTransaction);
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

    ISecurityContext ISecurityContextFactory.CreateSecurityContext ()
    {
      return new SecurityContext (GetPublicDomainObjectType());
    }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}
