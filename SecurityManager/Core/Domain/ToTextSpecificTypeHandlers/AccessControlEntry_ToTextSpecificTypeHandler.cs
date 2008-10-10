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
  public class AccessControlEntry_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AccessControlEntry>
  {
    public override void ToText (AccessControlEntry accessControlEntry, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("accessControlEntry", accessControlEntry);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<AccessControlEntry> ("").e (accessControlEntry.Permissions).e ("SelUser", accessControlEntry.UserSelection).e ("SelGroup", accessControlEntry.GroupSelection).e ("SelTenant", accessControlEntry.TenantSelection).eIfNotNull ("user", accessControlEntry.SpecificUser).eIfNotNull ("position", accessControlEntry.SpecificPosition).eIfNotNull ("group", accessControlEntry.SpecificGroup).eIfNotNull ("tenant", accessControlEntry.SpecificTenant).ie ();
    }
  }
}