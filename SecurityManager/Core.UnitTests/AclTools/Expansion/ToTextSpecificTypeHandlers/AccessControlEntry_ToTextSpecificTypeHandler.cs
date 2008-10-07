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

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class AccessControlEntry_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AccessControlEntry>
  {
    public override void ToText (AccessControlEntry x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<AccessControlEntry> ("").e (x.Permissions).e ("SelUser", x.UserSelection).e ("SelGroup", x.GroupSelection).e ("SelTenant", x.TenantSelection).eIfNotNull ("user", x.SpecificUser).eIfNotNull ("position", x.SpecificPosition).eIfNotNull ("group", x.SpecificGroup).eIfNotNull ("tenant", x.SpecificTenant).ie ();
    }
  }
}