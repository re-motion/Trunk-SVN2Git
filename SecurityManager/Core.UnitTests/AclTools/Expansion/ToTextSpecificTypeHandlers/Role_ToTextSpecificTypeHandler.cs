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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class Role_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<Role>
  {
    public override void ToText (Role x, IToTextBuilder toTextBuilder)
    {
      toTextBuilder.ib<Role> ("");
      toTextBuilder.e ("user", x.User.UserName).e ("group", x.Group.Name).e ("pos", x.Position.Name);
      toTextBuilder.ie ();
    }
  }
}