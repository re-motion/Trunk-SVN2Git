// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class SecurityToken_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<SecurityToken>
  {
    public override void ToText (SecurityToken securityToken, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("securityToken", securityToken);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<SecurityToken> ("").e ("principal", securityToken.User).eIfNotNull (securityToken.OwningTenant).eIfNotNull (securityToken.OwningGroups).eIfNotNull (securityToken.OwningGroupRoles).eIfNotNull (securityToken.AbstractRoles).ie ();
    }
  }
}