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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class User_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<User>
  {
    public override void ToText (User u, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<User> ().e ("user", u.UserName).e ("title", u.Title).e ("first", u.FirstName).e ("last", u.LastName);
      toTextBuilder.e ("display", u.DisplayName).e ("owning group", u.OwningGroup).e ("tenat", u.Tenant);
      toTextBuilder.nl ().e ("roles", u.Roles);
      toTextBuilder.ie ();
    }
  }
}