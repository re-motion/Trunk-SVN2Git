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
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class Role_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<Role>
  {
    public override void ToText (Role role, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<Role> ("");
      if (toTextBuilder.OutputComplexity >= ToTextBuilderBase.ToTextBuilderOutputComplexityLevel.Complex)
      {
        //toTextBuilder.e ("user", x.User.UserName).e ("group", x.Group.Name).e ("pos", x.Position.Name);
        toTextBuilder.e ("user", role.User.UserName).e ("group", role.Group.DisplayName).e ("pos", role.Position.DisplayName);
      }
      else
      {
        //toTextBuilder.e (x.User.UserName).e (x.Group.Name).e (x.Position.Name);
        toTextBuilder.e (role.User.UserName).e (role.Group.DisplayName).e (role.Position.DisplayName);
      }
      toTextBuilder.ie ();
    }
  }
}