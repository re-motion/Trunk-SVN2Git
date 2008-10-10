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
  public class User_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<User>
  {
    public override void ToText (User user, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      if (toTextBuilder.OutputComplexity >= ToTextBuilderBase.ToTextBuilderOutputComplexityLevel.Complex)
      {
        toTextBuilder.ib<User> ().e ("user", user.UserName).e ("title", user.Title).e ("first", user.FirstName).e ("last", user.LastName);
        toTextBuilder.e ("display", user.DisplayName).e ("group", user.OwningGroup).e ("tenant", user.Tenant);
        toTextBuilder.nl ().e ("roles", user.Roles);
        toTextBuilder.ie ();
      }
      else
      {
        toTextBuilder.ib<User> ("").e (user.UserName).ie();
      }
    }

  }




}